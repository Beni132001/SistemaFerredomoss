using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class BreakdownModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string ProfileCode { get; set; }
        public string ProfileName { get; set; }
        public double Size { get; set; }
        public string Color { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
