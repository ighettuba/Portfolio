using ERaceSystem.DAL;
using ERaceSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.BLL
{
    [DataObject]
    public class EmployeeController
    {
        public EmployeeItem Employee_FindByID(int employeeid)
        {
            using (var context = new ERaceSystemContext())
            {
                var results = context.Employees
                                .Where(x => x.EmployeeID == employeeid)
                                .Select(x => x)
                                .FirstOrDefault();
                EmployeeItem item = new EmployeeItem
                {                    
                    EmployeeID = results.EmployeeID,
                    LastName = results.LastName,
                    FirstName = results.FirstName,
                    Address = results.Address,
                    City = results.City,                   
                    PostalCode = results.PostalCode,
                    Phone = results.Phone,
                    BirthDate = results.BirthDate,
                    PositionID = results.PositionID,
                    LoginId = results.LoginId,
                    SocialInsuranceNumber = results.SocialInsuranceNumber
                };
                return item;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<EmployeeList> Employee_ListNames()
        {
            using (var context = new ERaceSystemContext())
            {
                var employeelist = from x in context.Employees
                                   orderby  x.FirstName, x.LastName
                                   select new EmployeeList
                                   {
                                       DisplayText = x.FirstName + ", " + x.LastName,
                                       ValueId = x.EmployeeID,
                                       Position = x.PositionID
                                   };
                return employeelist.ToList();
            }
        }


    }
}
