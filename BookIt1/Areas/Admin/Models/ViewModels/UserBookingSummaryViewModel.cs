namespace BookIt1.Areas.Admin.Models.ViewModels
{
    public class UserBookingSummaryViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int TotalBookings { get; set; }
        public int TotalTickets { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
