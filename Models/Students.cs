using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public partial class Students
    {
        public Students()
        {
            Carts = new HashSet<Carts>();
            DonationDetails = new HashSet<DonationDetails>();
        }
        [Key]
        public int StudentId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        [StringLength(50)]
        public string Nationality { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 999999, ErrorMessage = "Price must be between 0 and 999,999")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Price { get; set; }
        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        [InverseProperty(nameof(Courses.Students))]
        public virtual Courses Course { get; set; }
        [InverseProperty("Students")]
        public virtual ICollection<Carts> Carts { get; set; }
        [InverseProperty("Students")]
        public virtual ICollection<DonationDetails> DonationDetails { get; set; }
    }
}
