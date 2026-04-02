using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class ProfileRepository
    {
        private readonly IDatabaseService _databaseService;

        public ProfileRepository()
        {
            _databaseService = new DatabaseService();
        }

        // Obtener todos los perfiles
        public List<ProfileModel> GetAll()
        {
            var profiles = new List<ProfileModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("GetAllProfiles", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                profiles.Add(new ProfileModel
                                {
                                    Code = reader.GetString("code"),
                                    Name = reader.GetString("name"),
                                    Size = reader.IsDBNull(reader.GetOrdinal("size")) ? 0 : reader.GetDouble("size")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener perfiles: " + ex.Message);
            }
            return profiles;
        }

        // Agregar perfil
        public bool Add(ProfileModel profile)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("AddProfile", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", profile.Code);
                        cmd.Parameters.AddWithValue("@p_name", profile.Name);
                        cmd.Parameters.AddWithValue("@p_size", profile.Size);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar perfil: " + ex.Message);
                return false;
            }
        }

        // Actualizar perfil
        public bool Update(ProfileModel profile)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UpdateProfile", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", profile.Code);
                        cmd.Parameters.AddWithValue("@p_name", profile.Name);
                        cmd.Parameters.AddWithValue("@p_size", profile.Size);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar perfil: " + ex.Message);
                return false;
            }
        }

        // Eliminar perfil
        public bool Delete(string code)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DeleteProfile", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_code", code);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar perfil: " + ex.Message);
                return false;
            }
        }

        //para ver si el codigo ya existe
        public bool ExistsCode(string code)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM profiles WHERE code = @code";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al verificar código: " + ex.Message);
                return false;
            }
        }
    }
}