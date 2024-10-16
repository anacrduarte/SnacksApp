using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnacksApp.Models
{
    public class OrdersPerUser
    {
        public int Id { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
