namespace ERaceSystem.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    internal partial class RaceFee
    {
        public int RaceFeeID { get; set; }

        [Required(ErrorMessage ="Description is required")]
        [StringLength(50,ErrorMessage ="Description is limited to 50 characters")]
        public string Description { get; set; }

        [Column(TypeName = "money")]
        public decimal Fee { get; set; }
    }
}
