using Microsoft.EntityFrameworkCore;
using TravelSmartBooking.Api.Data;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Repositories;

// FACADE (GoF Patterns)
// SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
public class BookingFacade : IBookingFacade
{
    private readonly IPackageRepository _packageRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly TravelSmartContext _context;
    private readonly IConfirmationSenderStrategy _confirmationSender;

    // OPEN-CLOSED PRINCIPLE (SOLID Principles)
    public BookingFacade(
        IPackageRepository packageRepository,
        IBookingRepository bookingRepository,
        IConfirmationSenderStrategy confirmationSender
    )
    {
        _packageRepository = packageRepository;
        _confirmationSender = confirmationSender;
        _bookingRepository = bookingRepository;
    }

    public IPackageRepository GetPackageRepository()
    {
        return _packageRepository;
    }

    public async Task<bool> BookPackageAsync(int packageId, string customerDetails)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null || string.IsNullOrEmpty(customerDetails))
        {
            return false;
        }

        var booking = new Booking
        {
            PackageId = packageId,
            BookingDate = DateTime.Now,
            CustomerDetails = customerDetails
        };
        await _bookingRepository.AddAsync(booking);
        return true;
    }
}
