using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class SupplierOrderMaterialModel
    {
        public int SupplierOrderId { get; set; }
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public SupplierOrderModel SupplierOrder { get; set; }
        public MaterialModel Material { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
