namespace ERaceSystem.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    internal partial class UnOrderedItem
    {
        [Key]
        public int ItemID { get; set; }

        public int OrderID { get; set; }

        [Required(ErrorMessage ="Item name is required")]
        [StringLength(50,ErrorMessage ="Item name is limited to 50 characters")]
        public string ItemName { get; set; }

        [Required(ErrorMessage ="Vendor product ID is required")]
        [StringLength(25,ErrorMessage ="Vendor product ID is limited to 25 characters")]
        public string VendorProductID { get; set; }

        public int Quantity { get; set; }
    }
}
