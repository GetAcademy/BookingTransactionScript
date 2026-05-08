using BookingTransactionScript.Core._2_DomainServices;
using BookingTransactionScript.Core._3_Domain_Model;

namespace BookingTransactionScript.Core._1_ApplicationServices
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result> BookAsync(DateTime start, DateTime end)
        {
            if (start >= end)
            {
                return Result.Fail("Start must be before end.");
            }

            if (start.Minute != 0 || end.Minute != 0)
            {
                return Result.Fail("Only whole hours can be booked.");
            }

            if (start.Hour < 8 || end.Hour > 16)
            {
                return Result.Fail("Booking must be within opening hours.");
            }

            var existingBookings = await _bookingRepository.GetAllAsync();

            foreach (var existing in existingBookings)
            {
                var overlaps = start < existing.End && end > existing.Start;

                if (overlaps)
                {
                    return Result.Fail("Booking overlaps with an existing booking.");
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

            return Result.Success();
        }
    }
}
