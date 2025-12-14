namespace Domain
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int? UserId { get; set; }
        public string CardLastFour { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Navigation properties
        public virtual User? User { get; set; }
    }
}