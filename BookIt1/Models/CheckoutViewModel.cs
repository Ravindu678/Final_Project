namespace BookIt1.Models
{
    public class CheckoutViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; }
        public decimal Total { get; set; }
    }
}
