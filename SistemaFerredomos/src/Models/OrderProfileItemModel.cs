using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class OrderProfileItemModel
    {
        public ProfileModel Profile { get; set; }
        public decimal Quantity { get; set; } = 1;
        public string Color { get; set; }

        // Helpers para la vista
        public string ProfileName => Profile?.Name ?? "";
        public string ProfileCode => Profile?.Code ?? "";
        public double ProfileSize => Profile?.Size ?? 0;
    }
}