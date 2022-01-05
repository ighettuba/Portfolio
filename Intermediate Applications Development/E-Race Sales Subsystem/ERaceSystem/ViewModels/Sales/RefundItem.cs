using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.ViewModels.Sales
{
    public class RefundItem
    {
        public int RefundID { get; set; }
        public int InvoiceID { get; set; }
        public int ProductID { get; set; }
        public string ItemName { get; set; }
        public decimal RestockCharge { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
    }
}
