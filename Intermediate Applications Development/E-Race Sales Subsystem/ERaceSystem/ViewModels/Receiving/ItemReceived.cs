using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.ViewModels.Receiving
{
    public class ItemReceived
    {
        public int OrderDetailID { get; set; }
        public int QtyOrdered { get; set; }
        public int QtyOutstanding { get; set; }
        public int UnitReceived { get; set; }
        public int QtySalvaged { get; set; }
        public int UnitRejected { get; set; }

    }
}
