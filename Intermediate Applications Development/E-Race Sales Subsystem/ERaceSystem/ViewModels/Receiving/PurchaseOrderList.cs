using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.ViewModels.Receiving
{
    class PurchaseOrderList
    {
        public int OrderID { get; set; }
        public int? OrderNumber { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public bool Closed { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
