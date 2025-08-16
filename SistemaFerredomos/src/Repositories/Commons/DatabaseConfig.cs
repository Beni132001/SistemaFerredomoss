using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Repositories.Commons
{
    public static class DatabaseConfig
    {
        // Configura tus credenciales aquí
        public const string Server = "localhost";
        public const string Database = "BDSISTEMAFERREDOMOS";
        public const string UserId = "root";
        public const string Password = "password";

        public static string ConnectionString =>
            $"Server={Server};Database={Database};Uid={UserId};Pwd={Password};";
    }
}
