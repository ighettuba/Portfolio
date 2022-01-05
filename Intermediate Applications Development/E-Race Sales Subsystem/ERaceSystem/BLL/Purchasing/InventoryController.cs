using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespace
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
    public class InventoryController
    {
        public List<InventoryList> GetVendorInventory(int vendorid)
        {
            using (var context = new ERaceSystemContext())
            {
                var results = from x in context.Categories
                              select new InventoryList
                              {
                                  Description = x.Description,
                                  ProductList = (from y in context.VendorCatalogs
                                                 where y.VendorID == vendorid && y.Product.Category.Description == x.Description
                                                 select new ProductList
                                                 {
                                                     ProductID = y.ProductID,
                                                     ItemName = y.Product.ItemName,
                                                     ReOrderLevel = y.Product.ReOrderLevel,
                                                     QuantityOnHand = y.Product.QuantityOnHand,
                                                     QuantityOnOrder = y.Product.QuantityOnOrder,
                                                     UnitSize = y.OrderUnitSize,
                                                     UnitCost = y.OrderUnitCost,
                                                     Size = y.OrderUnitType + " (" + y.OrderUnitSize + ")"
                                                 }).ToList()
                              };

                return results.ToList();
            }
        }
    }
}
