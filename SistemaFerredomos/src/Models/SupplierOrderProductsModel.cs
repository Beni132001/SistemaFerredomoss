using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class SupplierOrderProductsModel
    {
        public int SupplierOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public SupplierOrderModel SupplierOrder { get; set; }
        public ProductsModel Products { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
