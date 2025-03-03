using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Interfaces;

// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
// DEPENDENCY INVERSION PRINCIPLE(SOLID Principles)
public interface IPackageRepository
{
    Task<IEnumerable<Package>> GetAllAsync();
    Task<Package?> GetByIdAsync(int id);
    Task<Package> AddAsync(Package package);
    Task UpdateAsync(Package package);
    Task DeleteAsync(int id);
}
