using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class ProductsRepository
    {
        private readonly IDatabaseService _databaseService;

        public ProductsRepository()
        {
            _databaseService = new DatabaseService();
        }

        // Obtener todos los productos
        public List<ProductsModel> GetAll()
        {
            var products = new List<ProductsModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetAllProducts", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                products.Add(MapReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener productos: " + ex.Message);
            }
            return products;
        }

        // Obtener producto por ID
        public ProductsModel GetById(int id)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetProductById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener producto por ID: " + ex.Message);
            }
            return null;
        }

        // Agregar producto
        public bool Add(ProductsModel product)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("AddProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_name", product.Name);
                        cmd.Parameters.AddWithValue("@p_stock", product.Stock);
                        cmd.Parameters.AddWithValue("@p_purchase_price", product.PurchasePrice);
                        cmd.Parameters.AddWithValue("@p_sale_price", product.SalePrice);
                        cmd.Parameters.AddWithValue("@p_supplier_id", product.SupplierId);
                        cmd.Parameters.AddWithValue("@p_image", product.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_mx", product.Mx ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_code", product.Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_size", product.Size);
                        cmd.Parameters.AddWithValue("@p_color_code", product.ColorCode ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar producto: " + ex.Message);
                return false;
            }
        }

        // Actualizar producto
        public bool Update(ProductsModel product)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UpdateProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", product.Id);
                        cmd.Parameters.AddWithValue("@p_name", product.Name);
                        cmd.Parameters.AddWithValue("@p_stock", product.Stock);
                        cmd.Parameters.AddWithValue("@p_purchase_price", product.PurchasePrice);
                        cmd.Parameters.AddWithValue("@p_sale_price", product.SalePrice);
                        cmd.Parameters.AddWithValue("@p_supplier_id", product.SupplierId);
                        cmd.Parameters.AddWithValue("@p_image", product.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_mx", product.Mx ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_code", product.Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_size", product.Size);
                        cmd.Parameters.AddWithValue("@p_color_code", product.ColorCode ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar producto: " + ex.Message);
                return false;
            }
        }

        // Eliminar producto
        public bool Delete(int id)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DeleteProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar producto: " + ex.Message);
                return false;
            }
        }

        // Obtener proveedores
        public List<SupplierModel> GetSuppliers()
        {
            var suppliers = new List<SupplierModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetAllSuppliers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                suppliers.Add(new SupplierModel
                                {
                                    Id = reader.GetInt32("id"),
                                    Name = reader.GetString("name"),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                                    Address = reader.IsDBNull(reader.GetOrdinal("address")) ? "" : reader.GetString("address")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener proveedores: " + ex.Message);
            }
            return suppliers;
        }

        // Actualizar stock (restar)
        public void UpdateStock(int productId, int quantity)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE products SET stock = stock - @quantity WHERE id = @productId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar stock: " + ex.Message);
            }
        }

        // Aumentar stock
        public void IncreaseStock(int productId, int quantity)
        {
            using var conn = _databaseService.GetConnection();
            conn.Open();
            string query = "UPDATE products SET stock = stock + @qty WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@qty", quantity);
            cmd.Parameters.AddWithValue("@id", productId);
            cmd.ExecuteNonQuery();
        }

        // ── Método auxiliar para mapear el reader ──
        private ProductsModel MapReader(MySqlDataReader reader)
        {
            return new ProductsModel
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Stock = reader.GetInt32("stock"),
                PurchasePrice = reader.GetDecimal("purchase_price"),
                SalePrice = reader.GetDecimal("sale_price"),
                Image = reader.IsDBNull(reader.GetOrdinal("image")) ? null : reader.GetString("image"),
                SupplierId = reader.IsDBNull(reader.GetOrdinal("supplier_id")) ? 0 : reader.GetInt32("supplier_id"),
                // ★ Campos nuevos
                Mx = reader.IsDBNull(reader.GetOrdinal("mx")) ? null : reader.GetString("mx"),
                Code = reader.IsDBNull(reader.GetOrdinal("code")) ? null : reader.GetString("code"),
                Size = reader.IsDBNull(reader.GetOrdinal("size")) ? 0 : reader.GetDouble("size"),
                ColorCode = reader.IsDBNull(reader.GetOrdinal("color_code")) ? null : reader.GetString("color_code"),
                Supplier = new SupplierModel
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("supplier_id")) ? 0 : reader.GetInt32("supplier_id"),
                    Name = reader.IsDBNull(reader.GetOrdinal("supplier_name")) ? "" : reader.GetString("supplier_name"),
                    Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                    Address = reader.IsDBNull(reader.GetOrdinal("address")) ? "" : reader.GetString("address")
                }
            };
        }
    }
}