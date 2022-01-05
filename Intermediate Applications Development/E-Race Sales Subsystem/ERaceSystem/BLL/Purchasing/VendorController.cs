using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional namespaces
using ERaceSystem.DAL;
using ERaceSystem.Entities;
using System.ComponentModel;
using ERaceSystem.ViewModels;
using ERaceSystem.ViewModels.Purchasing;
using DMIT2018Common.UserControls;
#endregion

namespace ERaceSystem.BLL.Purchasing
{
    [DataObject]
    public class VendorController
    {
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SelectionList> List_VendorNames()
        {
            using (var context = new ERaceSystemContext())
            {

                var results = from x in context.Vendors
                              orderby x.Name
                              select new SelectionList
                              {
                                  ValueId = x.VendorID,
                                  DisplayText = x.Name
                              };
                return results.ToList();
            }
        }



        //public List<VendorInfo> List_VendorInfo(int Vendorid)
        //{
        //    using (var context = new ERaceSystemContext())
        //    {
        //        var result = from x in context.Vendors
        //                     where x.VendorID == Vendorid
        //                     select new VendorInfo
        //                     {
        //                         VendorID = x.VendorID,
        //                         Name = x.Name,
        //                         Address = x.Address + " " + x.City,
        //                         Phone = x.Phone,
        //                         Contact = x.Contact
        //                     };
        //        return result.FirstOrDefault();
        //    }
        //}
        public VendorInfo List_VendorInfo(int Vendorid)
        {
            using (var context = new ERaceSystemContext())
            {
                var result = from x in context.Vendors
                             where x.VendorID == Vendorid
                             select new VendorInfo
                             {
                                 VendorID = x.VendorID,
                                 Name = x.Name,
                                 Address = x.Address + " " + x.City,
                                 Phone = x.Phone,
                                 Contact = x.Contact
                             };
                return result.FirstOrDefault();
            }


        }
    }
}
