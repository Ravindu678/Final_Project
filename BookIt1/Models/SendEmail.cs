using System.ComponentModel.DataAnnotations;

namespace BookIt1.Models
{
    public class SendEmail
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int Mobile  { get; set; }
        [Required]
        public string Message { get; set; }

    }
}
