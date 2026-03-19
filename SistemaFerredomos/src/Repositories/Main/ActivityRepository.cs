using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class ActivityRepository
    {
        private readonly IDatabaseService _databaseService;

        public ActivityRepository()
        {
            _databaseService = new DatabaseService();
        }

        public List<ActivityModel> GetAll()
        {
            var list = new List<ActivityModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT a.id, a.user_id, u.name AS user_name,
                                            a.activity, a.reference_id, a.date_time
                                     FROM activity_logs a
                                     LEFT JOIN users u ON a.user_id = u.id
                                     ORDER BY a.date_time DESC
                                     LIMIT 200";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(MapReader(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener actividad: " + ex.Message);
            }
            return list;
        }

        public List<ActivityModel> GetByUser(string userName)
        {
            var list = new List<ActivityModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT a.id, a.user_id, u.name AS user_name,
                                            a.activity, a.reference_id, a.date_time
                                     FROM activity_logs a
                                     LEFT JOIN users u ON a.user_id = u.id
                                     WHERE u.name LIKE @search OR u.username LIKE @search
                                     ORDER BY a.date_time DESC
                                     LIMIT 200";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", $"%{userName}%");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                list.Add(MapReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al filtrar actividad: " + ex.Message);
            }
            return list;
        }

        public void Log(int userId, string activity, int? referenceId = null)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO activity_logs (user_id, activity, reference_id) VALUES (@uid, @act, @ref)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@act", activity);
                        cmd.Parameters.AddWithValue("@ref", referenceId.HasValue ? (object)referenceId.Value : DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al registrar actividad: " + ex.Message);
            }
        }

        private ActivityModel MapReader(MySqlDataReader reader)
        {
            return new ActivityModel
            {
                Id = reader.GetInt32("id"),
                UserId = reader.GetInt32("user_id"),
                UserName = reader.IsDBNull(reader.GetOrdinal("user_name")) ? "" : reader.GetString("user_name"),
                Activity = reader.GetString("activity"),
                ReferenceId = reader.IsDBNull(reader.GetOrdinal("reference_id")) ? null : reader.GetInt32("reference_id"),
                DateTime = reader.GetDateTime("date_time")
            };
        }
    }
}