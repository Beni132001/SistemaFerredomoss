using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class MaterialModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Stock{ get; set; }
        public decimal PurchasePrice{ get; set; }
        public decimal SalePrice { get; set; }
        public int SupplierId { get; set; }

        public SupplierModel Supplier { get; set; }
    }
}
