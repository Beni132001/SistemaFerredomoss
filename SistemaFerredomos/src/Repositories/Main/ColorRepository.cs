using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class ColorRepository
    {
        private readonly IDatabaseService _databaseService;

        public ColorRepository()
        {
            _databaseService = new DatabaseService();
        }

        // Obtener todos los colores
        public List<ColorModel> GetAll()
        {
            var colors = new List<ColorModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetAllColors", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                colors.Add(new ColorModel
                                {
                                    Code = reader.GetString("code"),
                                    Name = reader.GetString("name")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener colores: " + ex.Message);
            }
            return colors;
        }

        // Agregar color
        public bool Add(ColorModel color)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("AddColor", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", color.Code);
                        cmd.Parameters.AddWithValue("@p_name", color.Name);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar color: " + ex.Message);
                return false;
            }
        }

        // Actualizar color
        public bool Update(ColorModel color)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UpdateColor", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", color.Code);
                        cmd.Parameters.AddWithValue("@p_name", color.Name);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar color: " + ex.Message);
                return false;
            }
        }

        // Eliminar color
        public bool Delete(string code)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DeleteColor", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", code);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar color: " + ex.Message);
                return false;
            }
        }
    }
}