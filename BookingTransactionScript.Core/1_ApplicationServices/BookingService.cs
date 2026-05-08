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
            var durationResult = Duration.Create(start, end);
            if (!durationResult.IsSuccess)
            {
                return Result<Booking>.Fail(durationResult.ErrorMessage!);
            }
            var existingBookings = await _bookingRepository.GetAllAsync();

            foreach (var existing in existingBookings)
            {
                var overlaps = start < existing.End && end > existing.Start;

                if (overlaps)
                {
                    return Result<Booking>.Fail("Booking overlaps with an existing booking.");
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

            return Result<Booking>.Success(booking);
        }
    }
}
