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

        // 🟢 Obtener todos los materiales
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
                            {
                                materials.Add(new MaterialModel
                                {
                                    Id = reader.GetInt32("id"),
                                    Name = reader.GetString("name"),
                                    Stock = reader.GetDecimal("stock"),
                                    PurchasePrice = reader.GetDecimal("purchase_price"),
                                    SalePrice = reader.GetDecimal("sale_price"),
                                    Image = reader.IsDBNull(reader.GetOrdinal("image")) ? null : reader.GetString("image"),
                                    SupplierId = reader.IsDBNull(reader.GetOrdinal("supplier_id")) ? 0 : reader.GetInt32("supplier_id"),
                                    Supplier = new SupplierModel
                                    {
                                        Id = reader.IsDBNull(reader.GetOrdinal("supplier_id")) ? 0 : reader.GetInt32("supplier_id"),
                                        Name = reader.IsDBNull(reader.GetOrdinal("supplier_name")) ? "" : reader.GetString("supplier_name"),
                                        Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                                        Address = reader.IsDBNull(reader.GetOrdinal("address")) ? "" : reader.GetString("address")
                                    }
                                });
                            }
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

        // 🟢 Obtener material por ID
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener material por ID: " + ex.Message);
            }

            return null;
        }

        // 🟢 Agregar material
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

        // Método para obtener todos los proveedores
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

        public void UpdateStock(int materialId, decimal quantity)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE materials
                         SET stock = stock - @quantity
                         WHERE id = @id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@id", materialId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetStock(int materialId)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = "SELECT stock FROM materials WHERE id=@id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", materialId);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                        return Convert.ToDecimal(result);

                    return 0;
                }
            }
        }

        public void UpdateStock(string materialName, decimal quantity)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"
        UPDATE materials
        SET stock = stock - @quantity
        WHERE name = @name";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@name", materialName);

                    command.ExecuteNonQuery();
                }
            }
        }

        //contar materiales bajos
        public int GetLowStockCount()
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query =
                "SELECT COUNT(*) FROM materials WHERE stock < 5";

                using (var command = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        //metodo para contar aumentar en stock
        public void IncreaseStock(int materialId, decimal quantity)
        {
            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = "UPDATE materials SET stock = stock + @qty WHERE id = @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@qty", quantity);
                    command.Parameters.AddWithValue("@id", materialId);

                    command.ExecuteNonQuery();
                }
            }
        }

        //para ver materiales con stock bajo
        public List<MaterialModel> GetLowStockMaterials(decimal minStock = 5)
        {
            List<MaterialModel> materials = new List<MaterialModel>();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM materials WHERE stock <= @minStock";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@minStock", minStock);

                    using (var reader = command.ExecuteReader())
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
    }
}