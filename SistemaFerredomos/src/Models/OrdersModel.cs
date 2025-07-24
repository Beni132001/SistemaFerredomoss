using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    class OrdersModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; } 
        public string ClientName { get; set; }
        public string ClientNumber { get; set; }
        public string State { get; set; }
        public decimal TotalPrice { get; set; }
        public UserModel User { get; set; }
        public List<OrderDetailsProductionModel> Productions { get; set; } = new List<OrderDetailsProductionModel>();       
        public List<OrderDetailsProductsModel> Products { get; set; }   = new List<OrderDetailsProductsModel> { };  
    }
}
