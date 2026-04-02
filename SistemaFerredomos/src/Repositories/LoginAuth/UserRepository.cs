using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;

namespace SistemaFerredomos.src.Repositories.LoginAuth
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseService _databaseService;

        public UserRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public UserModel Authenticate(string username, string password)
        {
            try
            {
                using (var conn = _databaseService.GetConnection())
                {
                    conn.Open();

                    var cmd = new MySqlCommand(
                        "SELECT id, name, username, password, type FROM users WHERE username = @username",
                        conn);

                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        var storedPassword = reader.GetString("password");

                        bool isValid = false;

                        // 🔥 CASO 1: ya es hash BCrypt
                        if (storedPassword.StartsWith("$2"))
                        {
                            isValid = BCrypt.Net.BCrypt.Verify(password, storedPassword);
                        }
                        else
                        {
                            // CASO 2: contraseña en texto plano (MIGRACIÓN)
                            if (password == storedPassword)
                            {
                                isValid = true;

                                // generar nuevo hash
                                string newHash = BCrypt.Net.BCrypt.HashPassword(password);

                                // actualizar en BD automáticamente
                                UpdatePassword(reader.GetInt32("id"), newHash);
                            }
                        }

                        if (!isValid)
                            return null;

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
            catch (Exception ex)
            {
                throw new Exception("Error de conexión a la base de datos", ex);
            }
        }

        private void UpdatePassword(int userId, string newHash)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                var cmd = new MySqlCommand(
                    "UPDATE users SET password = @password WHERE id = @id",
                    conn);

                cmd.Parameters.AddWithValue("@password", newHash);
                cmd.Parameters.AddWithValue("@id", userId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}