using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class SupplierOrderModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalPrice { get; set; }
        public UserModel User { get; set; }
        public SupplierModel Supplier { get; set; }
        public List<SupplierOrderMaterialModel> Materials { get; set; } = new List<SupplierOrderMaterialModel>();
        public List<SupplierOrderProductsModel> Products { get; set; } = new List<SupplierOrderProductsModel> ();
    }
}
