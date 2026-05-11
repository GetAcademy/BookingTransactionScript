namespace BookingTransactionScript.Core._3_Domain_Model
{
    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsCancelled { get; set; }

        public bool IsOverlapping(BookingPeriod bookingPeriod)
        {
            //return bookingPeriod.Start < End && bookingPeriod.End > Start;
            return IsOverlapping(bookingPeriod.Start, bookingPeriod.End);
        }

        public bool IsOverlapping(DateTime start, DateTime end)
        {
            return start < End && end > Start;
        }
    }
}
