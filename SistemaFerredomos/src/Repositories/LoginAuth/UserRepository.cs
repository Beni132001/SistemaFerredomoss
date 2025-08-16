using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        "SELECT id, name, username, type FROM users " +
                        "WHERE username = @username AND password = @password",
                        conn);

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                UserName = reader.GetString("username"),
                                Type = Enum.Parse<UserType>(reader.GetString("type"), true)
                            };
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores (deberías implementar logging)
                throw new Exception("Error de conexión a la base de datos", ex);
            }
        }
    }
}
