using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class SupplierOrdersRepository
    {
        private readonly IDatabaseService _databaseService;

        public SupplierOrdersRepository()
        {
            _databaseService = new DatabaseService();
        }

        // CREAR PEDIDO
        public int CreateSupplierOrder(SupplierOrderModel order)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"INSERT INTO supplier_orders
                                (user_id, supplier_id, total_price)
                                VALUES
                                (@user_id, @supplier_id, @total_price);
                                SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", order.UserId);
                    command.Parameters.AddWithValue("@supplier_id", order.SupplierId);
                    command.Parameters.AddWithValue("@total_price", order.TotalPrice);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // GUARDAR MATERIALES
        public void SaveOrderMaterials(int orderId, List<SupplierOrderMaterialModel> materials)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in materials)
                        {
                            string query = @"INSERT INTO supplier_order_materials
                                    (supplier_order_id, material_id, quantity, unit_price)
                                    VALUES
                                    (@order_id, @material_id, @quantity, @price)";

                            using (var command = new MySqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@order_id", orderId);
                                command.Parameters.AddWithValue("@material_id", item.MaterialId);
                                command.Parameters.AddWithValue("@quantity", item.Quantity);
                                command.Parameters.AddWithValue("@price", item.UnitPrice);

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // GUARDAR PRODUCTOS
        public void SaveOrderProducts(int orderId, List<SupplierOrderProductsModel> products)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach(var item in products)
                        {
                            string query = @"INSERT INTO supplier_order_hardware_products
                                    (supplier_order_id, products_id, quantity, unit_price)
                                    VALUES
                                    (@order_id, @product_id, @quantity, @price)";

                            using (var command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@order_id", orderId);
                                command.Parameters.AddWithValue("@product_id", item.ProductId);
                                command.Parameters.AddWithValue("@quantity", item.Quantity);
                                command.Parameters.AddWithValue("@price", item.UnitPrice);

                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // OBTENER PEDIDOS
        public List<SupplierOrderModel> GetOrders()
        {
            List<SupplierOrderModel> orders = new List<SupplierOrderModel>();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"SELECT so.id,
                                        so.date,
                                        so.total_price,
                                        s.id AS supplier_id,
                                        s.name AS supplier_name
                                 FROM supplier_orders so
                                 JOIN suppliers s ON so.supplier_id = s.id
                                 ORDER BY so.date DESC";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new SupplierOrderModel
                        {
                            Id = reader.GetInt32("id"),
                            Date = reader.GetDateTime("date"),
                            SupplierId = reader.GetInt32("supplier_id"),
                            TotalPrice = reader.GetDecimal("total_price"),

                            Supplier = new SupplierModel
                            {
                                Id = reader.GetInt32("supplier_id"),
                                Name = reader.GetString("supplier_name")
                            }
                        });
                    }
                }
            }

            return orders;
        }

        // ELIMINAR PEDIDO
        public void DeleteOrder(int orderId)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query1 = "DELETE FROM supplier_order_materials WHERE supplier_order_id=@id";
                string query2 = "DELETE FROM supplier_order_hardware_products WHERE supplier_order_id=@id";
                string query3 = "DELETE FROM supplier_orders WHERE id=@id";

                using (var command = new MySqlCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }

                using (var command = new MySqlCommand(query2, connection))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }

                using (var command = new MySqlCommand(query3, connection))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }
            }
        }

        //obtener materiales del pedido
        public List<SupplierOrderMaterialModel> GetOrderMaterials(int orderId)
        {
            List<SupplierOrderMaterialModel> materials = new();

            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "SELECT * FROM supplier_order_materials WHERE supplier_order_id=@id";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", orderId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                materials.Add(new SupplierOrderMaterialModel
                {
                    MaterialId = reader.GetInt32("material_id"),
                    Quantity = reader.GetDecimal("quantity")
                });
            }

            return materials;
        }

        //obtener productos del pedido
        public List<SupplierOrderProductsModel> GetOrderProducts(int orderId)
        {
            List<SupplierOrderProductsModel> products = new();

            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "SELECT * FROM supplier_order_hardware_products WHERE supplier_order_id=@id";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", orderId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new SupplierOrderProductsModel
                {
                    ProductId = reader.GetInt32("products_id"),
                    Quantity = reader.GetInt32("quantity")
                });
            }

            return products;
        }

        //actualiza pecio total
        public void UpdateTotalPrice(int orderId)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"
        UPDATE supplier_orders
        SET total_price = (
            SELECT IFNULL(SUM(quantity * unit_price), 0)
            FROM supplier_order_materials
            WHERE supplier_order_id = @id
        )
        WHERE id = @id;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}