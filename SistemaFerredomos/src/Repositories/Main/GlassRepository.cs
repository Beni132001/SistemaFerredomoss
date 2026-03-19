using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class GlassRepository
    {
        private readonly IDatabaseService _databaseService;

        public GlassRepository()
        {
            _databaseService = new DatabaseService();
        }

        // Obtener todos los vidrios
        public List<GlassModel> GetAll()
        {
            var glassList = new List<GlassModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetAllGlass", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                glassList.Add(new GlassModel
                                {
                                    Code = reader.GetString("code"),
                                    Name = reader.GetString("name"),
                                    Width = reader.IsDBNull(reader.GetOrdinal("width")) ? 0 : reader.GetDouble("width"),
                                    Height = reader.IsDBNull(reader.GetOrdinal("height")) ? 0 : reader.GetDouble("height"),
                                    Thickness = reader.IsDBNull(reader.GetOrdinal("thickness")) ? 0 : reader.GetDouble("thickness")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener vidrios: " + ex.Message);
            }
            return glassList;
        }

        // Buscar vidrio por código o nombre
        public List<GlassModel> Search(string search)
        {
            var glassList = new List<GlassModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetGlassByCode", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_search", search);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                glassList.Add(new GlassModel
                                {
                                    Code = reader.GetString("code"),
                                    Name = reader.GetString("name"),
                                    Width = reader.IsDBNull(reader.GetOrdinal("width")) ? 0 : reader.GetDouble("width"),
                                    Height = reader.IsDBNull(reader.GetOrdinal("height")) ? 0 : reader.GetDouble("height"),
                                    Thickness = reader.IsDBNull(reader.GetOrdinal("thickness")) ? 0 : reader.GetDouble("thickness")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al buscar vidrio: " + ex.Message);
            }
            return glassList;
        }

        // Agregar vidrio
        public bool Add(GlassModel glass)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("AddGlass", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", glass.Code);
                        cmd.Parameters.AddWithValue("@p_name", glass.Name);
                        cmd.Parameters.AddWithValue("@p_width", glass.Width);
                        cmd.Parameters.AddWithValue("@p_height", glass.Height);
                        cmd.Parameters.AddWithValue("@p_thickness", glass.Thickness);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar vidrio: " + ex.Message);
                return false;
            }
        }

        // Actualizar vidrio
        public bool Update(GlassModel glass)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UpdateGlass", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", glass.Code);
                        cmd.Parameters.AddWithValue("@p_name", glass.Name);
                        cmd.Parameters.AddWithValue("@p_width", glass.Width);
                        cmd.Parameters.AddWithValue("@p_height", glass.Height);
                        cmd.Parameters.AddWithValue("@p_thickness", glass.Thickness);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar vidrio: " + ex.Message);
                return false;
            }
        }

        // Eliminar vidrio
        public bool Delete(string code)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DeleteGlass", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", code);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar vidrio: " + ex.Message);
                return false;
            }
        }
    }
}