namespace BookIt1.Models
{
    public class PaymentView
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }

        
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }

        
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public string CVV { get; set; }
    }
}
