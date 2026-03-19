using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaFerredomos.src.Repositories.Main
{
    public class UsersRepository
    {
        private readonly IDatabaseService _databaseService;

        public UsersRepository()
        {
            _databaseService = new DatabaseService();
        }

        public List<UserModel> GetAll()
        {
            var users = new List<UserModel>();
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, name, username, type FROM users ORDER BY name";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            users.Add(MapReader(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener usuarios: " + ex.Message);
            }
            return users;
        }

        public bool Add(UserModel user)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO users (name, username, password, type) VALUES (@name, @username, @password, @type)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", user.Name);
                        cmd.Parameters.AddWithValue("@username", user.UserName);
                        cmd.Parameters.AddWithValue("@password", user.Password);
                        cmd.Parameters.AddWithValue("@type", user.Type.ToString());
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al agregar usuario: " + ex.Message);
                return false;
            }
        }

        public bool Update(UserModel user)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();
                    // Si no se ingresa nueva contraseña, no la cambia
                    string query = string.IsNullOrWhiteSpace(user.Password)
                        ? "UPDATE users SET name=@name, username=@username, type=@type WHERE id=@id"
                        : "UPDATE users SET name=@name, username=@username, password=@password, type=@type WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        cmd.Parameters.AddWithValue("@name", user.Name);
                        cmd.Parameters.AddWithValue("@username", user.UserName);
                        cmd.Parameters.AddWithValue("@type", user.Type.ToString());
                        if (!string.IsNullOrWhiteSpace(user.Password))
                            cmd.Parameters.AddWithValue("@password", user.Password);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al actualizar usuario: " + ex.Message);
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
                    string query = "DELETE FROM users WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar usuario: " + ex.Message);
                return false;
            }
        }

        private UserModel MapReader(MySqlDataReader reader)
        {
            return new UserModel
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                UserName = reader.GetString("username"),
                Type = Enum.Parse<UserType>(reader.GetString("type"), true)
            };
        }
    }
}