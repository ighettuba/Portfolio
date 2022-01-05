using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ERaceSystem.ViewModels.Purchasing
{
    public class InventoryList
    {
        public string Description { get; set; }
        public List<ProductList> ProductList { get; set; }
    }
}
