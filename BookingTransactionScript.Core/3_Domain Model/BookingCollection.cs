namespace BookingTransactionScript.Core._3_Domain_Model
{
    internal class BookingCollection
    {
        private readonly IEnumerable<Booking> _bookings;

        public BookingCollection(IEnumerable<Booking> bookings)
        {
            _bookings = bookings;
        }

        public bool IsOverlapping(BookingPeriod bookingPeriod)
        {
            return _bookings.Any(bookingPeriod.IsOverlapping);

            /*
               foreach (var booking in _bookings)
               {
                   var overlaps = bookingPeriod.IsOverlapping(booking);
               
                   if (overlaps)
                   {
                       return true;
                   }
               }
               
               return false;             
             */
        }
    }
}
