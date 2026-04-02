
using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Windows.Input;

namespace SistemaFerredomos.src.Repositories.Main
{
    class OrdersRepository
    {
        private readonly IDatabaseService _databaseService;
        public OrdersRepository()
        {
            _databaseService = new DatabaseService();
        }

        public int CreateOrder(OrdersModel order)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"INSERT INTO orders
                                (user_id, date, customer_name, customer_phone, status, total_price)
                                VALUES
                                (@user_id, @date, @customer_name, @customer_phone, @status, @total_price);
                                SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", order.UserId);
                    command.Parameters.AddWithValue("@date", order.Date);
                    command.Parameters.AddWithValue("@customer_name", order.CustomerName);
                    command.Parameters.AddWithValue("@customer_phone", order.CustomerPhone);
                    command.Parameters.AddWithValue("@status", order.Status);
                    command.Parameters.AddWithValue("@total_price", order.TotalPrice);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public List<OrdersModel> GetOrders()
        {
            List<OrdersModel> orders = new List<OrdersModel>();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM orders ORDER BY date DESC";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new OrdersModel
                        {
                            Id = reader.GetInt32("id"),

                            UserId = reader.IsDBNull(reader.GetOrdinal("user_id"))
        ? 0
        : reader.GetInt32("user_id"),

                            Date = reader.IsDBNull(reader.GetOrdinal("date"))
        ? DateTime.Now
        : reader.GetDateTime("date"),

                            CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name"))
        ? ""
        : reader.GetString("customer_name"),

                            CustomerPhone = reader.IsDBNull(reader.GetOrdinal("customer_phone"))
        ? ""
        : reader.GetString("customer_phone"),

                            Status = reader.IsDBNull(reader.GetOrdinal("status"))
        ? "pendiente"
        : reader.GetString("status"),

                            TotalPrice = reader.IsDBNull(reader.GetOrdinal("total_price"))
        ? 0
        : reader.GetDecimal("total_price")
                        });
                    }
                }
            }

            return orders;
        }

        public void UpdateStatus(int orderId, string status)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = "UPDATE orders SET status=@status WHERE id=@id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@id", orderId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveOrderProducts(int orderId, List<OrderDetailsProductsModel> products)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in products)
                        {
                            string query = @"INSERT INTO order_products
                                    (order_id, product_id, quantity, unit_price)
                                    VALUES
                                    (@order_id, @product_id, @quantity, @price)";

                            using (var command = new MySqlCommand(query, connection, transaction))
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

        public void SaveOrderProductions(int orderId, List<OrderDetailsProductionModel> productions)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in productions)
                        {
                            string query = @"INSERT INTO order_production
                                    (order_id, production_id, quantity, unit_price)
                                    VALUES
                                    (@order_id, @production_id, @quantity, @price)";

                            using (var command = new MySqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@order_id", orderId);
                                command.Parameters.AddWithValue("@production_id", item.ProductionId);
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

        public void DeleteOrder(int orderId)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Eliminar productos de la orden
                        string deleteProducts = "DELETE FROM order_products WHERE order_id = @id";
                        using (var cmd = new MySqlCommand(deleteProducts, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        // Eliminar producciones de la orden
                        string deleteProductions = "DELETE FROM order_production WHERE order_id = @id";
                        using (var cmd = new MySqlCommand(deleteProductions, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        // Eliminar la orden
                        string deleteOrder = "DELETE FROM orders WHERE id = @id";
                        using (var cmd = new MySqlCommand(deleteOrder, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            cmd.ExecuteNonQuery();
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

        //para ver detalles de la orden
        public List<OrderDetailsProductsModel> GetOrderProducts(int orderId)
        {
            List<OrderDetailsProductsModel> products = new List<OrderDetailsProductsModel>();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"SELECT op.order_id, op.product_id, op.quantity, op.unit_price,
                         p.name
                         FROM order_products op
                         JOIN products p ON op.product_id = p.id
                         WHERE op.order_id = @orderId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderId", orderId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new OrderDetailsProductsModel
                            {
                                OrderId = reader.GetInt32("order_id"),
                                ProductId = reader.GetInt32("product_id"),
                                Quantity = reader.GetInt32("quantity"),
                                UnitPrice = reader.GetDecimal("unit_price"),
                                Products = new ProductsModel
                                {
                                    Name = reader.GetString("name")
                                }
                            });
                        }
                    }
                }
            }

            return products;
        }

        public List<OrderDetailsProductionModel> GetOrderProductions(int orderId)
        {
            var productions = new List<OrderDetailsProductionModel>();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"SELECT op.order_id, op.production_id, op.quantity, op.unit_price,
                                p.name, p.type
                         FROM order_production op
                         JOIN production p ON op.production_id = p.id
                         WHERE op.order_id = @orderId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderId", orderId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productions.Add(new OrderDetailsProductionModel
                            {
                                OrderId = reader.GetInt32("order_id"),
                                ProductionId = reader.GetInt32("production_id"),
                                Quantity = reader.GetInt32("quantity"),
                                UnitPrice = reader.GetDecimal("unit_price"),
                                Production = new ProductionModel
                                {
                                    Name = reader.GetString("name"),
                                    Type = reader.GetString("type")
                                }
                            });
                        }
                    }
                }
            }

            return productions;
        }

        //para analisis 
        public DashboardStatsModel GetDashboardStats()
        {
            var stats = new DashboardStatsModel();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string queryOrdersToday =
                "SELECT COUNT(*) FROM orders WHERE DATE(date)=CURDATE()";

                string queryPending =
                "SELECT COUNT(*) FROM orders WHERE status='pendiente'";

                string querySales =
                "SELECT IFNULL(SUM(total_price),0) FROM orders WHERE DATE(date)=CURDATE()";

                using (var cmd = new MySqlCommand(queryOrdersToday, connection))
                    stats.OrdersToday = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new MySqlCommand(queryPending, connection))
                    stats.PendingOrders = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new MySqlCommand(querySales, connection))
                    stats.TodaySales = Convert.ToDecimal(cmd.ExecuteScalar());
            }

            return stats;
        }

        //solo ordenes pendientes
        public List<OrdersModel> GetPendingOrders()
        {
            var orders = new List<OrdersModel>();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"SELECT id, customer_name, date 
                         FROM orders 
                         WHERE status = 'pendiente' 
                         ORDER BY date DESC";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new OrdersModel
                        {
                            Id = reader.GetInt32("id"),
                            CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name"))
                                ? "" : reader.GetString("customer_name"),
                            Date = reader.GetDateTime("date")
                        });
                    }
                }
            }

            return orders;
        }
    }
}