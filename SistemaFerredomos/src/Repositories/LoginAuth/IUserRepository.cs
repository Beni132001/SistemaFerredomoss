using SistemaFerredomos.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Repositories.LoginAuth
{
    public interface IUserRepository
    {
        UserModel Authenticate(string username, string password);
    }
}
