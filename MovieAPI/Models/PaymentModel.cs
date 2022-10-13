namespace MovieAPI.Models
{
    public class PaymentModel
    {
        public Guid UserID { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
            
    }
}
