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
using ERaceSystem.ViewModels.Sales;

#endregion
namespace ERaceSystem.BLL
{
    [DataObject]
    public class ProductController
    {
        #region Queries
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SelectionList> ProductsByCategory(int categoryid)
        {
            using (var context = new ERaceSystemContext())
            {
                var results = (from prod in context.Products
                               where prod.CategoryID == categoryid
                               select new SelectionList
                               {
                                   ValueId = prod.ProductID,
                                   DisplayText = prod.ItemName,                                  
                               });
                return results.ToList();
            }
        }
        public ProductItem Product_FindById(int productid)
        {
            using (var context = new ERaceSystemContext())
            {
                var result = (from prod in context.Products
                               where prod.ProductID == productid
                               select new ProductItem
                               {
                                   ProductID = prod.ProductID,
                                   ItemName = prod.ItemName,
                                   ItemPrice = prod.ItemPrice,
                                   QuantityOnHand = prod.QuantityOnHand,
                                   ReStockCharge = prod.ReStockCharge                                  
                               }).FirstOrDefault();
                return result;
            }
        }

        #endregion
    }
}
