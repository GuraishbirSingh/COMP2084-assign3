using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public partial class Donations
    {
        public Donations()
        {
            DonationDetails = new HashSet<DonationDetails>();
        }

        [Key]
        public int DonationId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DonationDate { get; set; }
        [Required]
        [StringLength(100)]
        public string UserId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Total { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        [Required]
        [StringLength(10)]
        public string Province { get; set; }
        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }
        [Required]
        [StringLength(15)]
        public string Phone { get; set; }

        [InverseProperty("Donations")]
        public virtual ICollection<DonationDetails> DonationDetails { get; set; }
    }
}
