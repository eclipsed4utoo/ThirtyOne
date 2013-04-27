using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThirtyOne
{
    public class UnpaidOrder
    {
        private string testing = string.Empty;
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public double OrderTotal { get; set; }
        public bool IsPaid { get; set; }
    }
}
