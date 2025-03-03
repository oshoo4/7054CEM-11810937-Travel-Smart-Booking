using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Interfaces;

// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
// DEPENDENCY INVERSION PRINCIPLE(SOLID Principles)
public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> AddAsync(Booking booking);
}
