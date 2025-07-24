using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    class SupplierOrderProductsModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitaryPrice { get; set; }
        public SupplierOrderModel SupplierOrder { get; set; }
        public ProductsModel Products { get; set; } 
    }
}
