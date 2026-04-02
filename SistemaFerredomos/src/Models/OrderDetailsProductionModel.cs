using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class OrderDetailsProductionModel
    {
        public int OrderId { get; set; }
        public int ProductionId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public OrdersModel Orders { get; set; }
        public ProductionModel Production { get; set; }

        public decimal Subtotal
        {
            get { return Quantity * UnitPrice; }
        }
    }
}