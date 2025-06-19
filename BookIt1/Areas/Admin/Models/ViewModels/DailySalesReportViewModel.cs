namespace BookIt1.Areas.Admin.Models.ViewModels
{
    public class DailySalesReportViewModel
    {
        public DateTime Date { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
