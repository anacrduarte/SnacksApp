using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnacksApp.Models
{
    public class Order
    {
        public string? Adress { get; set; }

        public decimal TotalValue { get; set; }
        public int UserId { get; set; }
    }
}
