using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class SupplierRepository
    {
        private readonly IDatabaseService _databaseService;

        public SupplierRepository()
        {
            _databaseService = new DatabaseService();
        }

        public List<SupplierModel> GetAll()
        {
            var suppliers = new List<SupplierModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, name, phone, address FROM suppliers ORDER BY name";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            suppliers.Add(MapReader(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener proveedores: " + ex.Message);
            }
            return suppliers;
        }

        public bool Add(SupplierModel supplier)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO suppliers (name, phone, address) VALUES (@name, @phone, @address)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", supplier.Name);
                        cmd.Parameters.AddWithValue("@phone", supplier.Phone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@address", supplier.Address ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar proveedor: " + ex.Message);
                return false;
            }
        }

        public bool Update(SupplierModel supplier)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE suppliers SET name=@name, phone=@phone, address=@address WHERE id=@id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", supplier.Id);
                        cmd.Parameters.AddWithValue("@name", supplier.Name);
                        cmd.Parameters.AddWithValue("@phone", supplier.Phone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@address", supplier.Address ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar proveedor: " + ex.Message);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM suppliers WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar proveedor: " + ex.Message);
                return false;
            }
        }

        public bool HasDependencies(int supplierId)
        {
            using var conn = _databaseService.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM materials WHERE supplier_id = @id",
                conn);

            cmd.Parameters.AddWithValue("@id", supplierId);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private SupplierModel MapReader(MySqlDataReader reader)
        {
            return new SupplierModel
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
                Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString("address")
            };
        }
    }
}