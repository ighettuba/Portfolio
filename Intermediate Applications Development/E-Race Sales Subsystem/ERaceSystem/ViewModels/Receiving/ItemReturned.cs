using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.ViewModels.Receiving
{
    public class ItemReturned
    {
        public int? OrderDetailID { get; set; }
        public string UnOrderedItem { get; set; }
        public int? ItemQuantity { get; set; }
        public int? ItemUnit { get; set; }
        public int? QtySalvaged { get; set; }
        public string Comment { get; set; }
        public string VendorProductID { get; set; }

    }
}
