using BookIt1.Areas.Admin.Models;

namespace BookIt1.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; }  
        public int EventId { get; set; }
        public int TicketsBooked { get; set; }
        public DateTime BookingDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        public Event Event { get; set; }
    }
}
