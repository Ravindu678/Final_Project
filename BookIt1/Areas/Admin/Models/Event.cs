namespace BookIt1.Areas.Admin.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        //public int Capacity { get; set; }
        public string Location { get; set; }
        public int TotalTickets { get; set; }       // Total capacity
        public int AvailableTickets { get; set; }   // Seats remaining for booking
        public decimal Price { get; set; }
        public string Category { get; set; }      // For filtering & recommendations
        public string? ImagePath { get; set; }
        //public DateTime Date { get; internal set; }
    }
}
