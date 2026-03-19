using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class ProductsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int SupplierId { get; set; }
        public string Image { get; set; }
        public string Mx { get; set; }
        public string Code { get; set; }
        public double Size { get; set; }
        public string ColorCode { get; set; }
        public SupplierModel Supplier { get; set; }
    }
}
