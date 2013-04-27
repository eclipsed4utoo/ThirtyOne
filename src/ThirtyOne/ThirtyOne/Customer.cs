using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThirtyOne
{
    public partial class Customer
    {
        private string testing = string.Empty;
        public override string ToString()
        {
            return this.CustomerName;
        }
    }
}
