using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class DashboardStatsModel
    {
        public int OrdersToday { get; set; }
        public int PendingOrders { get; set; }
        public int LowMaterials { get; set; }
        public decimal TodaySales { get; set; }
    }
}
