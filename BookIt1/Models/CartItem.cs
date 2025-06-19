using BookIt1.Areas.Admin.Models;
using BookIt1.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookIt1.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("EventId")]
        public Event Event { get; set; }

        //public string Id { get; set; }  // Foreign key to Identity User
        //public BookIt1User { get; set; } // Navigation property
    }

}
