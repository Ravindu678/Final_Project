namespace BookIt1.Models.ViewModels
{
    public class BookingSummary
    {
        public string Email { get; set; }
        public string EventTitle { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerTicket { get; set; }
        public decimal TotalPrice => Quantity * PricePerTicket;
    }
}
