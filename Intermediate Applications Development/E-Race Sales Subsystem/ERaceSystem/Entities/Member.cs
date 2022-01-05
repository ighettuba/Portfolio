namespace ERaceSystem.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    internal partial class Member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Member()
        {
            Cars = new HashSet<Car>();
            RaceDetails = new HashSet<RaceDetail>();
        }

        public int MemberID { get; set; }

        [Required(ErrorMessage ="Last Name is required")]
        [StringLength(30,ErrorMessage ="Lastname is limited to 30 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="First name is required")]
        [StringLength(30,ErrorMessage ="First name is limited to 30 characters")]
        public string FirstName { get; set; }

        [StringLength(10,ErrorMessage ="Phone is limited to 10 digits")]
        public string Phone { get; set; }

        [Required(ErrorMessage ="Address is required")]
        [StringLength(30,ErrorMessage ="Address is limited to 30 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage ="City is required")]
        [StringLength(30,ErrorMessage ="City is limited to 30 characters")]
        public string City { get; set; }

        [Required(ErrorMessage ="Postal code is required")]
        [StringLength(6,ErrorMessage ="Postal code is limited to 6 characters")]
        public string PostalCode { get; set; }

        [StringLength(30,ErrorMessage ="email address is limited to 30 characters")]
        public string EmailAddress { get; set; }

        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage ="Certification level is required")]
        [StringLength(1,ErrorMessage ="Certification level is 1 character")]
        public string CertificationLevel { get; set; }

        [StringLength(1,ErrorMessage ="Gender should be one character")]
        public string Gender { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Car> Cars { get; set; }

        public virtual Certification Certification { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RaceDetail> RaceDetails { get; set; }
    }
}
