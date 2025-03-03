using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Repositories;

// FACTORY (GoF Patterns)
// SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
public class PackageFactory : IPackageFactory
{
    public Package CreatePackage(
        string name,
        string description,
        decimal price,
        int availability,
        string prerequisites
    )
    {
        return new Package
        {
            Name = name,
            Description = description,
            Price = price,
            Availability = availability,
            Prerequisites = prerequisites,
        };
    }
}
