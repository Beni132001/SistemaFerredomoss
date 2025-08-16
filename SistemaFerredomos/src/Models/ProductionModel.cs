using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    class ProductionModel
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Type { get; set; } //DOMO-PORTERO-VENTANAS
        public int DesindgId { get; set; }
        public decimal Price { get; set; }
        public decimal? Height { get; set; }
        public decimal? lenght { get; set; }
        public decimal? Width { get; set; }

        public DesingsModel Desings { get; set; }   
        public List<ProductionMaterialModel> Materials { get; set; } = new List<ProductionMaterialModel>();
    }
}
