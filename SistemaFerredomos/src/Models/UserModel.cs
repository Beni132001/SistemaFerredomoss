using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string UserName { get; set; }   
        public string Password { get; set; }    
        public string Type { get; set; }  //administrador/usurario  


    }
}
