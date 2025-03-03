using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Interfaces;

// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
// DEPENDENCY INVERSION PRINCIPLE(SOLID Principles)
public interface IPackageFactory
{
    Package CreatePackage(
        string name,
        string description,
        decimal price,
        int availability,
        string prerequisites
    );
}
