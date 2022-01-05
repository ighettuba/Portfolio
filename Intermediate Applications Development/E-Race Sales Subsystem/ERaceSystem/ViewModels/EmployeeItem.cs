using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.ViewModels
{
    public class EmployeeItem
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public int PositionID { get; set; }
        public string LoginId { get; set; }
        public DateTime BirthDate { get; set; }
        public string SocialInsuranceNumber { get; set; }
    }
    public class EmployeeList
    {
        public int ValueId { get; set; }
        public string DisplayText { get; set; }
        public int Position { get; set; }

    }
}
