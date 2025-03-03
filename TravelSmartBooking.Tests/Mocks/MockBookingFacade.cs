using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;
using TravelSmartBooking.Api.Repositories;

namespace TravelSmartBooking.Tests.Mocks;

public class MockBookingFacade : IBookingFacade
{
    public bool BookPackageAsyncWasCalled { get; private set; } = false;
    public int LastBookedPackageId { get; private set; } = -1;
    public string LastCustomerDetails { get; private set; } = string.Empty;

    public bool BookPackageAsyncResult { get; set; } = true;

    private readonly IPackageRepository _packageRepository;

    public MockBookingFacade(
        IPackageRepository packageRepository,
        IBookingRepository bookingRepository,
        IConfirmationSenderStrategy confirmationSender
    )
    {
        _packageRepository = packageRepository;
    }

    public async Task<bool> BookPackageAsync(int packageId, string customerDetails)
    {
        BookPackageAsyncWasCalled = true;
        LastBookedPackageId = packageId;
        LastCustomerDetails = customerDetails;

        return await Task.FromResult(BookPackageAsyncResult);
    }

    public IPackageRepository GetPackageRepository()
    {
        return _packageRepository;
    }
}
