namespace BookIt1.Areas.Admin.Models.ViewModels
{
    public class EventBookingReportViewModel
    {
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UniqueBookers { get; set; }
    }
}

