using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.ViewModels.Sales
{
    public class InvoiceItem
    {
        public int InvoiceDetailID { get; set; }

        public int InvoiceID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal? Price { get; set; }
        public string ItemName { get; set; }
        public decimal? Amount { get; set; }

    }
}
