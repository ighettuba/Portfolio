using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.ViewModels.Receiving
{
    public class UnorderedItem
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string VendorProductID { get; set; }
        public int Quantity { get; set; }
        public int OrderID { get; set; }

    }
}
