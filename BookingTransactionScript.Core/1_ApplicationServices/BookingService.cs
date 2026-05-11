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

        public async Task<Result<Booking>> BookAsync(DateTime start, DateTime end)
        {
            var bookingPeriodResult = BookingPeriod.Create(start, end);
            if (!bookingPeriodResult.IsSuccess)
            {
                return Result<Booking>.Fail(bookingPeriodResult.ErrorMessage!);
            }

            var bookingPeriod = bookingPeriodResult.Value!;
            var existingBookings = await _bookingRepository.GetAllAsync();

            foreach (var booking in existingBookings)
            {
                var overlaps = bookingPeriod.IsOverlapping(booking);
                //var overlap = existing.IsOverlapping(start, end);
                //var overlaps = start < existing.End && end > existing.Start;

                if (overlaps)
                {
                    return Result<Booking>.Fail("Booking overlaps with an existing booking.");
                }
            }

            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                Start = start,
                End = end,
                IsCancelled = false
            };

            await _bookingRepository.AddAsync(newBooking);

            return Result<Booking>.Success(newBooking);
        }
    }
}
