using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    class OrderDetailsProductsModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitaryPrice { get; set; }
        public OrdersModel Orders { get; set; }
        public ProductsModel Products { get; set; }
    }
}
