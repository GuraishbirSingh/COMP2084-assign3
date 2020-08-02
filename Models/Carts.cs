using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public partial class Carts
    {
        [Key]
        public int CartId { get; set; }
        public int StudentId { get; set; }
        public int Credits { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [ForeignKey(nameof(StudentId))]
        [InverseProperty("Carts")]
        public virtual  Students Students { get; set; }
    }
}
