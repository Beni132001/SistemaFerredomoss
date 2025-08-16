using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Repositories.Commons
{
        public interface IDatabaseService
        {
            MySqlConnection GetConnection();
        }

    public class DatabaseService : IDatabaseService
    {
        public MySqlConnection GetConnection() =>
            new MySqlConnection(DatabaseConfig.ConnectionString);
    }
}
