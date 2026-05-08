namespace BookingTransactionScript.Core._3_Domain_Model
{
    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsCancelled { get; set; }
    }
}
