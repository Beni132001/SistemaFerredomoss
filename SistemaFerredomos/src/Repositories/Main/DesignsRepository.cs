using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class DesignsRepository
    {
        private readonly IDatabaseService _databaseService;

        public DesignsRepository()
        {
            _databaseService = new DatabaseService();
        }

        public List<DesignsModel> GetAll()
        {
            var designs = new List<DesignsModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, name, description, color, image FROM designs ORDER BY name";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            designs.Add(MapReader(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener diseños: " + ex.Message);
            }
            return designs;
        }

        public bool Add(DesignsModel design)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO designs (name, description, color, image)
                                     VALUES (@name, @description, @color, @image)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", design.Name);
                        cmd.Parameters.AddWithValue("@description", design.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@color", design.Color ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@image", design.Image ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar diseño: " + ex.Message);
                return false;
            }
        }

        public bool Update(DesignsModel design)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE designs
                                     SET name=@name, description=@description, color=@color, image=@image
                                     WHERE id=@id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", design.Id);
                        cmd.Parameters.AddWithValue("@name", design.Name);
                        cmd.Parameters.AddWithValue("@description", design.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@color", design.Color ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@image", design.Image ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar diseño: " + ex.Message);
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
                    string query = "DELETE FROM designs WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar diseño: " + ex.Message);
                return false;
            }
        }

        private DesignsModel MapReader(MySqlDataReader reader)
        {
            return new DesignsModel
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                Color = reader.IsDBNull(reader.GetOrdinal("color")) ? null : reader.GetString("color"),
                Image = reader.IsDBNull(reader.GetOrdinal("image")) ? null : reader.GetString("image")
            };
        }
    }
}