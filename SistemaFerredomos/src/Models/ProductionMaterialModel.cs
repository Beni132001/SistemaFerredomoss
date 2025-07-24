using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    class ProductionMaterialModel
    {
        public int ProductionId { get; set; }
        public int MaterialId { get; set; }
        public decimal Quantity{ get; set; }

        public ProductionModel Production { get; set; }
        public MaterialModel Material { get; set; } 
    }
}
