namespace BookingTransactionScript.Core
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<string> BookAsync(DateTime start, DateTime end)
        {
            var existingBookings = await _bookingRepository.GetAllAsync();

            if (start >= end)
            {
                return "Start must be before end.";
            }

            if (start.Minute != 0 || end.Minute != 0)
            {
                return "Only whole hours can be booked.";
            }

            if (start.Hour < 8 || end.Hour > 16)
            {
                return "Booking must be within opening hours.";
            }

            foreach (var existing in existingBookings)
            {
                var overlaps = start < existing.End && end > existing.Start;

                if (overlaps)
                {
                    return "Booking overlaps with an existing booking.";
                }
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                Start = start,
                End = end,
                IsCancelled = false
            };

            await _bookingRepository.AddAsync(booking);

            return "Booking created.";
        }
    }

    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task AddAsync(Booking booking);
    }

    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsCancelled { get; set; }
    }
}
