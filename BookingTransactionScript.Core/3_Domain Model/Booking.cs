namespace BookingTransactionScript.Core._3_Domain_Model
{
    public class Booking
    {
        public Guid Id { get;  }
        public DateTime Start { get; }
        public DateTime End { get; }
        public bool IsCancelled { get; private set; }

        public Booking(DateTime start, DateTime end)
        : this(Guid.NewGuid(), start, end)
        {
        }

        public Booking(Guid id, DateTime start, DateTime end, bool isCancelled = false)
        {
            Id = id;
            Start = start;
            End = end;
            IsCancelled = isCancelled;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
