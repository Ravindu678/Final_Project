namespace BookIt1.Models
{
    public class BookingViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public int AvailableTickets { get; set; }
        public int TicketsToBook { get; set; }

        // Additional user input fields
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
    }

}
