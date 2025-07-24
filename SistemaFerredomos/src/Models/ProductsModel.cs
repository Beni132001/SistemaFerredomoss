using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    class ProductsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal stock { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int SupplierId { get; set; }
        public string Image { get; set; }

        public SupplierModel Supplier { get; set; }
    }
}
