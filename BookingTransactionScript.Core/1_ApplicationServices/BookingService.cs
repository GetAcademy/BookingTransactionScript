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
            var validationResult = await ValidateBooking(start, end);
            if (!validationResult.IsSuccess) return validationResult;

            var newBooking = new Booking(start, end);
            await _bookingRepository.AddAsync(newBooking);
            return Result<Booking>.Success(newBooking);
        }

        private async Task<Result<Booking>> ValidateBooking(DateTime start, DateTime end)
        {
            var bookingPeriodResult = BookingPeriod.Create(start, end);
            if (!bookingPeriodResult.IsSuccess)
            {
                return Result<Booking>.Fail(bookingPeriodResult.ErrorMessage!);
            }

            var bookingPeriod = bookingPeriodResult.Value!;
            var existingBookings = await _bookingRepository.GetAllAsync();
            var bookingCollection = new BookingCollection(existingBookings);
            if (bookingCollection.IsOverlapping(bookingPeriod))
            {
                return Result<Booking>.Fail("Booking overlaps with an existing booking.");
            }
        }
    }
}
