using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Tests.Mocks;

public class MockBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings;
    private int _nextId = 1;

    public MockBookingRepository()
    {
        _bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                PackageId = 1,
                BookingDate = DateTime.UtcNow.AddDays(-2),
                CustomerDetails = "Customer 1",
            },
            new Booking
            {
                Id = 2,
                PackageId = 2,
                BookingDate = DateTime.UtcNow.AddDays(-1),
                CustomerDetails = "Customer 2",
            },
        };
        _nextId = 3;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await Task.FromResult(_bookings);
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_bookings.FirstOrDefault(b => b.Id == id));
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        booking.Id = _nextId++;
        _bookings.Add(booking);
        return await Task.FromResult(booking);
    }
}
