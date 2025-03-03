namespace TravelSmartBooking.Api.Interfaces;

// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
// DEPENDENCY INVERSION PRINCIPLE(SOLID Principles)
public interface IBookingFacade
{
    Task<bool> BookPackageAsync(int packageId, string customerDetails);
    IPackageRepository GetPackageRepository();
}
