using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public class DonationDetails
    {
        [Key]
        public int DonationDetailId { get; set; }
        public int DonationId { get; set; }
        public int StudentId { get; set; }
        public int Credits { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(DonationId))]
        [InverseProperty("DonationDetails")]
        public virtual Donations Donations { get; set; }
        [ForeignKey(nameof(StudentId))]
        [InverseProperty("DonationDetails")]
        public virtual Students Students { get; set; }
    }
}
