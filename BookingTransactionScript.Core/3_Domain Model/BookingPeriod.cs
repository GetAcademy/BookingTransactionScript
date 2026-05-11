namespace BookingTransactionScript.Core._3_Domain_Model
{
    public class BookingPeriod
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        private BookingPeriod(DateTime start, DateTime end)
        {
            End = end;
            Start = start;
        }

        public static Result<BookingPeriod> Create(DateTime start, DateTime end)
        {
            if (start >= end)
            {
                return Result<BookingPeriod>.Fail("Start must be before end.");
            }

            if (start.Minute != 0 || end.Minute != 0)
            {
                return Result<BookingPeriod>.Fail("Only whole hours can be booked.");
            }

            if (start.Hour < 8 || end.Hour > 16)
            {
                return Result<BookingPeriod>.Fail("Booking must be within opening hours.");
            }

            var duration = new BookingPeriod(start, end);
            return Result<BookingPeriod>.Success(duration);
        }

        public bool IsOverlapping(Booking booking)
        {
            return IsOverlapping(booking.Start, booking.End);
        }

        public bool IsOverlapping(DateTime start, DateTime end)
        {
            return start < End && end > Start;
        }
    }
}
 