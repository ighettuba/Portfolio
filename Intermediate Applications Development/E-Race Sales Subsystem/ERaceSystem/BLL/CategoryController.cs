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
    public class CategoryController
    {
        #region Queries
        [DataObjectMethod(DataObjectMethodType.Select,false)]
        public List<SelectionList> ListCategories()
        {
            using (var context = new ERaceSystemContext())
            {
                var result = context.Categories.Select(x => new SelectionList { ValueId = x.CategoryID, DisplayText = x.Description });
                return result.ToList();
            }
        }

        #endregion
    }
}
