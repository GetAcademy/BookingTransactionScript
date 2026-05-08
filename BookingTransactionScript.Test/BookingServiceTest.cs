using BookingTransactionScript.Core;

namespace BookingTransactionScript.Test
{
    public class BookingServiceTests
    {
        private static readonly DateTime Monday = new(2026, 1, 5);

        [Test]
        public async Task BookAsync_WhenBookingIsValid_ReturnsBookingCreated()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(10);
            var end = Monday.AddHours(11);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Booking created."));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsValid_AddsOneBookingToRepository()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(10);
            var end = Monday.AddHours(11);

            // Act
            await service.BookAsync(start, end);

            // Assert
            Assert.That(repository.AddedBookings, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsValid_StoresCorrectStartAndEnd()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(10);
            var end = Monday.AddHours(12);

            // Act
            await service.BookAsync(start, end);

            // Assert
            var booking = repository.AddedBookings.Single();

            Assert.That(booking.Start, Is.EqualTo(start));
            Assert.That(booking.End, Is.EqualTo(end));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsValid_CreatesBookingWithId()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            // Act
            await service.BookAsync(
                Monday.AddHours(10),
                Monday.AddHours(11));

            // Assert
            var booking = repository.AddedBookings.Single();

            Assert.That(booking.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsValid_CreatesBookingThatIsNotCancelled()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            // Act
            await service.BookAsync(
                Monday.AddHours(10),
                Monday.AddHours(11));

            // Assert
            var booking = repository.AddedBookings.Single();

            Assert.That(booking.IsCancelled, Is.False);
        }

        [Test]
        public async Task BookAsync_WhenStartEqualsEnd_ReturnsErrorMessage()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(10);
            var end = Monday.AddHours(10);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Start must be before end."));
        }

        [Test]
        public async Task BookAsync_WhenStartIsAfterEnd_ReturnsErrorMessage()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(11);
            var end = Monday.AddHours(10);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Start must be before end."));
        }

        [Test]
        public async Task BookAsync_WhenStartIsAfterEnd_DoesNotAddBooking()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            // Act
            await service.BookAsync(
                Monday.AddHours(11),
                Monday.AddHours(10));

            // Assert
            Assert.That(repository.AddedBookings, Is.Empty);
        }

        [Test]
        public async Task BookAsync_WhenStartHasMinutes_ReturnsWholeHoursErrorMessage()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(10).AddMinutes(30);
            var end = Monday.AddHours(11);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Only whole hours can be booked."));
        }

        [Test]
        public async Task BookAsync_WhenEndHasMinutes_ReturnsWholeHoursErrorMessage()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(10);
            var end = Monday.AddHours(11).AddMinutes(30);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Only whole hours can be booked."));
        }

        [Test]
        public async Task BookAsync_WhenStartAndEndHaveMinutes_ReturnsWholeHoursErrorMessage()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(10).AddMinutes(15);
            var end = Monday.AddHours(11).AddMinutes(45);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Only whole hours can be booked."));
        }

        [Test]
        public async Task BookAsync_WhenStartHasMinutes_DoesNotAddBooking()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            // Act
            await service.BookAsync(
                Monday.AddHours(10).AddMinutes(30),
                Monday.AddHours(11));

            // Assert
            Assert.That(repository.AddedBookings, Is.Empty);
        }

        [Test]
        public async Task BookAsync_WhenStartIsBeforeOpeningHours_ReturnsOpeningHoursErrorMessage()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(7);
            var end = Monday.AddHours(8);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Booking must be within opening hours."));
        }

        [Test]
        public async Task BookAsync_WhenEndIsAfterOpeningHours_ReturnsOpeningHoursErrorMessage()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(15);
            var end = Monday.AddHours(17);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Booking must be within opening hours."));
        }

        [Test]
        public async Task BookAsync_WhenBookingStartsExactlyAtOpeningTime_IsAllowed()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(8);
            var end = Monday.AddHours(9);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Booking created."));
            Assert.That(repository.AddedBookings, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingEndsExactlyAtClosingTime_IsAllowed()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(15);
            var end = Monday.AddHours(16);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Booking created."));
            Assert.That(repository.AddedBookings, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingCoversWholeOpeningDay_IsAllowed()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(8);
            var end = Monday.AddHours(16);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Booking created."));
            Assert.That(repository.AddedBookings, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingStartsBeforeExistingBookingButEndsInsideExistingBooking_ReturnsOverlapError()
        {
            // Existing: 10:00 - 12:00
            // New:       09:00 - 11:00

            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(10),
                    End = Monday.AddHours(12),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(9),
                Monday.AddHours(11));

            // Assert
            Assert.That(result, Is.EqualTo("Booking overlaps with an existing booking."));
        }

        [Test]
        public async Task BookAsync_WhenBookingStartsInsideExistingBookingButEndsAfterExistingBooking_ReturnsOverlapError()
        {
            // Existing: 10:00 - 12:00
            // New:       11:00 - 13:00

            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(10),
                    End = Monday.AddHours(12),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(11),
                Monday.AddHours(13));

            // Assert
            Assert.That(result, Is.EqualTo("Booking overlaps with an existing booking."));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsCompletelyInsideExistingBooking_ReturnsOverlapError()
        {
            // Existing: 09:00 - 13:00
            // New:       10:00 - 11:00

            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(9),
                    End = Monday.AddHours(13),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(10),
                Monday.AddHours(11));

            // Assert
            Assert.That(result, Is.EqualTo("Booking overlaps with an existing booking."));
        }

        [Test]
        public async Task BookAsync_WhenBookingCompletelyCoversExistingBooking_ReturnsOverlapError()
        {
            // Existing: 10:00 - 11:00
            // New:       09:00 - 12:00

            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(10),
                    End = Monday.AddHours(11),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(9),
                Monday.AddHours(12));

            // Assert
            Assert.That(result, Is.EqualTo("Booking overlaps with an existing booking."));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsExactlySameAsExistingBooking_ReturnsOverlapError()
        {
            // Existing: 10:00 - 11:00
            // New:       10:00 - 11:00

            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(10),
                    End = Monday.AddHours(11),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(10),
                Monday.AddHours(11));

            // Assert
            Assert.That(result, Is.EqualTo("Booking overlaps with an existing booking."));
        }

        [Test]
        public async Task BookAsync_WhenBookingEndsExactlyWhenExistingBookingStarts_IsAllowed()
        {
            // Existing: 10:00 - 11:00
            // New:       09:00 - 10:00

            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(10),
                    End = Monday.AddHours(11),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(9),
                Monday.AddHours(10));

            // Assert
            Assert.That(result, Is.EqualTo("Booking created."));
            Assert.That(repository.AddedBookings, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingStartsExactlyWhenExistingBookingEnds_IsAllowed()
        {
            // Existing: 10:00 - 11:00
            // New:       11:00 - 12:00

            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(10),
                    End = Monday.AddHours(11),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(11),
                Monday.AddHours(12));

            // Assert
            Assert.That(result, Is.EqualTo("Booking created."));
            Assert.That(repository.AddedBookings, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingOverlapsOneOfSeveralExistingBookings_ReturnsOverlapError()
        {
            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(8),
                    End = Monday.AddHours(9),
                    IsCancelled = false
                },
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(12),
                    End = Monday.AddHours(14),
                    IsCancelled = false
                },
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(15),
                    End = Monday.AddHours(16),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(13),
                Monday.AddHours(15));

            // Assert
            Assert.That(result, Is.EqualTo("Booking overlaps with an existing booking."));
        }

        [Test]
        public async Task BookAsync_WhenBookingFitsBetweenExistingBookings_IsAllowed()
        {
            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(8),
                    End = Monday.AddHours(10),
                    IsCancelled = false
                },
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(12),
                    End = Monday.AddHours(14),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(10),
                Monday.AddHours(12));

            // Assert
            Assert.That(result, Is.EqualTo("Booking created."));
            Assert.That(repository.AddedBookings, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsInvalid_DoesNotCallAddAsync()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            // Act
            await service.BookAsync(
                Monday.AddHours(7),
                Monday.AddHours(8));

            // Assert
            Assert.That(repository.AddAsyncCallCount, Is.EqualTo(0));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsValid_CallsGetAllAsyncOnce()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            // Act
            await service.BookAsync(
                Monday.AddHours(10),
                Monday.AddHours(11));

            // Assert
            Assert.That(repository.GetAllAsyncCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsValid_CallsAddAsyncOnce()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            // Act
            await service.BookAsync(
                Monday.AddHours(10),
                Monday.AddHours(11));

            // Assert
            Assert.That(repository.AddAsyncCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task BookAsync_WhenStartIsAfterEndAndAlsoOutsideOpeningHours_ReturnsStartBeforeEndErrorFirst()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(17);
            var end = Monday.AddHours(7);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Start must be before end."));
        }

        [Test]
        public async Task BookAsync_WhenStartHasMinutesAndAlsoOutsideOpeningHours_ReturnsWholeHoursErrorFirst()
        {
            // Arrange
            var repository = new FakeBookingRepository();
            var service = new BookingService(repository);

            var start = Monday.AddHours(7).AddMinutes(30);
            var end = Monday.AddHours(8);

            // Act
            var result = await service.BookAsync(start, end);

            // Assert
            Assert.That(result, Is.EqualTo("Only whole hours can be booked."));
        }

        [Test]
        public async Task BookAsync_WhenBookingIsOutsideOpeningHoursAndOverlapsExistingBooking_ReturnsOpeningHoursErrorFirst()
        {
            // Arrange
            var repository = new FakeBookingRepository
            {
                ExistingBookings =
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    Start = Monday.AddHours(7),
                    End = Monday.AddHours(9),
                    IsCancelled = false
                }
            }
            };

            var service = new BookingService(repository);

            // Act
            var result = await service.BookAsync(
                Monday.AddHours(7),
                Monday.AddHours(9));

            // Assert
            Assert.That(result, Is.EqualTo("Booking must be within opening hours."));
        }

        private class FakeBookingRepository : IBookingRepository
        {
            public List<Booking> ExistingBookings { get; } = new();
            public List<Booking> AddedBookings { get; } = new();

            public int GetAllAsyncCallCount { get; private set; }
            public int AddAsyncCallCount { get; private set; }

            public Task<List<Booking>> GetAllAsync()
            {
                GetAllAsyncCallCount++;

                var allBookings = ExistingBookings
                    .Concat(AddedBookings)
                    .ToList();

                return Task.FromResult(allBookings);
            }

            public Task AddAsync(Booking booking)
            {
                AddAsyncCallCount++;

                AddedBookings.Add(booking);

                return Task.CompletedTask;
            }
        }
    }
}
