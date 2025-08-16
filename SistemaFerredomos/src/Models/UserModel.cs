using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public enum UserType { admin, user }
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string UserName { get; set; }   
        public string Password { get; set; }    
        public UserType Type { get; set; }  //administrador/usurario  

        public bool IsAdmin => Type == UserType.admin; 

       
    } 


}
