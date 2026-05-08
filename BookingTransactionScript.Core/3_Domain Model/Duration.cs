namespace BookingTransactionScript.Core._3_Domain_Model
{
    internal class Duration
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        private Duration(DateTime start, DateTime end)
        {
            End = end;
            Start = start;
        }

        public static Result<Duration> Create(DateTime start, DateTime end)
        {
            if (start >= end)
            {
                return Result<Duration>.Fail("Start must be before end.");
            }

            if (start.Minute != 0 || end.Minute != 0)
            {
                return Result<Duration>.Fail("Only whole hours can be booked.");
            }

            if (start.Hour < 8 || end.Hour > 16)
            {
                return Result<Duration>.Fail("Booking must be within opening hours.");
            }

            var duration = new Duration(start, end);
            return Result<Duration>.Success(duration);
        }
    }
}
