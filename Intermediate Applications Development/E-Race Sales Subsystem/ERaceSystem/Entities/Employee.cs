namespace ERaceSystem.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    internal partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            Invoices = new HashSet<Invoice>();
            Orders = new HashSet<Order>();
            ReceiveOrders = new HashSet<ReceiveOrder>();
        }

        public int EmployeeID { get; set; }

        [Required(ErrorMessage ="Last Name is required")]
        [StringLength(30,ErrorMessage ="Lastname is limited to 30 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="First name is required")]
        [StringLength(30,ErrorMessage ="First Name is limited to 30 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage ="Address is required")]
        [StringLength(30,ErrorMessage ="Address is limited to 30 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage ="City is required")]
        [StringLength(30,ErrorMessage ="City is limited to 30 characters")]
        public string City { get; set; }

        [Required(ErrorMessage ="Postal code is required")]
        [StringLength(6,ErrorMessage ="Postal code is limited to 6 characters")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage ="Phone is required")]
        [StringLength(10,ErrorMessage ="Phone is limited to 10 digits")]
        public string Phone { get; set; }

        public int PositionID { get; set; }

        [StringLength(50,ErrorMessage ="Login ID is limited to 50 characters")]
        public string LoginId { get; set; }

        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage ="SIN is required")]
        [StringLength(9,ErrorMessage ="SIN is limited to 9 digits")]
        public string SocialInsuranceNumber { get; set; }

        public virtual Position Position { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReceiveOrder> ReceiveOrders { get; set; }
    }
}
