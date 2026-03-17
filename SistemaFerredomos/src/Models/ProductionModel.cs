using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class ProductionModel
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Type { get; set; } //DOMO-PORTERO-VENTANAS
        public int DesignId { get; set; }
        public decimal Price { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        public DesignsModel Designs { get; set; }   
        public List<ProductionMaterialModel> Materials { get; set; } = new List<ProductionMaterialModel>();
    }
}
