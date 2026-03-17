using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.Models
{
    public class ActivityModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Activity { get; set; } //login logout order, supplier order
        public int? ReferenceId { get; set; } //id ordens
        public DateTime CreatedDate { get; set; }
        public UserModel User { get; set; } 
    }
}
