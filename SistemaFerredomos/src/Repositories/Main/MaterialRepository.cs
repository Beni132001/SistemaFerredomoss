using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class MaterialRepository
    {
        private readonly IDatabaseService _databaseService;

        public MaterialRepository()
        {
            _databaseService = new DatabaseService();
        }

        // Obtener todos los materiales
        public List<MaterialModel> GetAll()
        {
            var materials = new List<MaterialModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetAllMaterials", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                materials.Add(MapReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener materiales: " + ex.Message);
            }
            return materials;
        }

        // Obtener material por ID
        public MaterialModel GetById(int id)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetMaterialById", conn))
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
                Console.WriteLine("❌ Error al obtener material por ID: " + ex.Message);
            }
            return null;
        }

        // Agregar material
        public bool Add(MaterialModel material)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("AddMaterial", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_name", material.Name);
                        cmd.Parameters.AddWithValue("@p_stock", material.Stock);
                        cmd.Parameters.AddWithValue("@p_purchase_price", material.PurchasePrice);
                        cmd.Parameters.AddWithValue("@p_sale_price", material.SalePrice);
                        cmd.Parameters.AddWithValue("@p_supplier_id", material.SupplierId);
                        cmd.Parameters.AddWithValue("@p_image", material.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_code", material.Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_size", material.Size);
                        cmd.Parameters.AddWithValue("@p_shelf", material.Shelf ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_color_code", material.ColorCode ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar material: " + ex.Message);
                return false;
            }
        }

        // Actualizar material
        public bool Update(MaterialModel material)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UpdateMaterial", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", material.Id);
                        cmd.Parameters.AddWithValue("@p_name", material.Name);
                        cmd.Parameters.AddWithValue("@p_stock", material.Stock);
                        cmd.Parameters.AddWithValue("@p_purchase_price", material.PurchasePrice);
                        cmd.Parameters.AddWithValue("@p_sale_price", material.SalePrice);
                        cmd.Parameters.AddWithValue("@p_supplier_id", material.SupplierId);
                        cmd.Parameters.AddWithValue("@p_image", material.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_code", material.Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_size", material.Size);
                        cmd.Parameters.AddWithValue("@p_shelf", material.Shelf ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_color_code", material.ColorCode ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar material: " + ex.Message);
                return false;
            }
        }

        // Eliminar material
        public bool Delete(int id)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DeleteMaterial", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar material: " + ex.Message);
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
        public void UpdateStock(int materialId, decimal quantity)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();
                string query = "UPDATE materials SET stock = stock - @quantity WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@id", materialId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Actualizar stock por nombre (restar)
        public void UpdateStock(string materialName, decimal quantity)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();
                string query = "UPDATE materials SET stock = stock - @quantity WHERE name = @name";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@name", materialName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Aumentar stock
        public void IncreaseStock(int materialId, decimal quantity)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();
                string query = "UPDATE materials SET stock = stock + @qty WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@qty", quantity);
                    cmd.Parameters.AddWithValue("@id", materialId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Obtener stock actual
        public decimal GetStock(int materialId)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();
                string query = "SELECT stock FROM materials WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", materialId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        // Contar materiales con stock bajo
        public int GetLowStockCount()
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM materials WHERE stock < 5";
                using (var cmd = new MySqlCommand(query, conn))
                    return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Obtener materiales con stock bajo
        public List<MaterialModel> GetLowStockMaterials(decimal minStock = 5)
        {
            var materials = new List<MaterialModel>();
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM materials WHERE stock <= @minStock";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@minStock", minStock);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            materials.Add(new MaterialModel
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Stock = reader.GetDecimal("stock"),
                                PurchasePrice = reader.GetDecimal("purchase_price"),
                                SalePrice = reader.GetDecimal("sale_price")
                            });
                        }
                    }
                }
            }
            return materials;
        }

        // Obtener precio de compra
        public decimal GetPurchasePrice(int materialId)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();
                string query = "SELECT purchase_price FROM materials WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", materialId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        // ── Método auxiliar para mapear el reader ──
        private MaterialModel MapReader(MySqlDataReader reader)
        {
            return new MaterialModel
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Stock = reader.GetDecimal("stock"),
                PurchasePrice = reader.GetDecimal("purchase_price"),
                SalePrice = reader.GetDecimal("sale_price"),
                Image = reader.IsDBNull(reader.GetOrdinal("image")) ? null : reader.GetString("image"),
                SupplierId = reader.IsDBNull(reader.GetOrdinal("supplier_id")) ? 0 : reader.GetInt32("supplier_id"),
                // ★ Campos nuevos
                Code = reader.IsDBNull(reader.GetOrdinal("code")) ? null : reader.GetString("code"),
                Size = reader.IsDBNull(reader.GetOrdinal("size")) ? 0 : reader.GetDouble("size"),
                Shelf = reader.IsDBNull(reader.GetOrdinal("shelf")) ? null : reader.GetString("shelf"),
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