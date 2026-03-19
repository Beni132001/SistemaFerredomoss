using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class BreakdownRepository
    {
        private readonly IDatabaseService _databaseService;

        public BreakdownRepository()
        {
            _databaseService = new DatabaseService();
        }

        // Obtener todos los desgloses
        public List<BreakdownModel> GetAll()
        {
            var breakdowns = new List<BreakdownModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetAllBreakdowns", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                breakdowns.Add(MapReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener desgloses: " + ex.Message);
            }
            return breakdowns;
        }

        // Obtener desglose por número de orden
        public List<BreakdownModel> GetByOrderNumber(string orderNumber)
        {
            var breakdowns = new List<BreakdownModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetBreakdownByOrder", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_order_number", orderNumber);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                breakdowns.Add(MapReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener desglose por orden: " + ex.Message);
            }
            return breakdowns;
        }

        // Agregar desglose
        public bool Add(BreakdownModel breakdown)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("AddBreakdown", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_order_number", breakdown.OrderNumber);
                        cmd.Parameters.AddWithValue("@p_profile_code", breakdown.ProfileCode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_profile_name", breakdown.ProfileName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_size", breakdown.Size);
                        cmd.Parameters.AddWithValue("@p_color", breakdown.Color ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_quantity", breakdown.Quantity);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar desglose: " + ex.Message);
                return false;
            }
        }

        // Eliminar desglose por ID
        public bool Delete(int id)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DeleteBreakdown", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", id);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar desglose: " + ex.Message);
                return false;
            }
        }

        // Método auxiliar para mapear el reader
        private BreakdownModel MapReader(MySqlDataReader reader)
        {
            return new BreakdownModel
            {
                Id = reader.GetInt32("id"),
                OrderNumber = reader.GetString("order_number"),
                ProfileCode = reader.IsDBNull(reader.GetOrdinal("profile_code")) ? null : reader.GetString("profile_code"),
                ProfileName = reader.IsDBNull(reader.GetOrdinal("profile_name")) ? null : reader.GetString("profile_name"),
                Size = reader.IsDBNull(reader.GetOrdinal("size")) ? 0 : reader.GetDouble("size"),
                Color = reader.IsDBNull(reader.GetOrdinal("color")) ? null : reader.GetString("color"),
                Quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? 1 : reader.GetDecimal("quantity"),
                CreatedAt = reader.GetDateTime("created_at")
            };
        }
    }
}