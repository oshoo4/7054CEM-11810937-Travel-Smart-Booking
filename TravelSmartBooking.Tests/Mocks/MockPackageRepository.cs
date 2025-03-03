using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Tests.Mocks;

public class MockPackageRepository : IPackageRepository
{
    private readonly List<Package> _packages;
    private readonly IPackageFactory _packageFactory;

    public MockPackageRepository(IPackageFactory packageFactory)
    {
        _packageFactory = packageFactory;
        _packages = new List<Package>()
        {
            _packageFactory.CreatePackage("Test Package 1", "Description 1", 100, 10, "None"),
            _packageFactory.CreatePackage("Test Package 2", "Description 2", 200, 5, "Passport"),
        };

        int id = 1;
        foreach (var package in _packages)
        {
            package.Id = id++;
        }
    }

    public async Task<IEnumerable<Package>> GetAllAsync()
    {
        return await Task.FromResult(_packages);
    }

    public async Task<Package?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_packages.FirstOrDefault(p => p.Id == id));
    }

    public async Task<Package> AddAsync(Package package)
    {
        var newPackage = _packageFactory.CreatePackage(
            package.Name,
            package.Description,
            package.Price,
            package.Availability,
            package.Prerequisites
        );
        newPackage.Id = _packages.Count > 0 ? _packages.Max(p => p.Id) + 1 : 1;
        _packages.Add(newPackage);
        return await Task.FromResult(newPackage);
    }

    public async Task UpdateAsync(Package package)
    {
        var existingPackage = _packages.FirstOrDefault(p => p.Id == package.Id);
        if (existingPackage == null)
        {
            throw new KeyNotFoundException($"Package with ID {package.Id} not found.");
        }

        existingPackage.Name = package.Name;
        existingPackage.Description = package.Description;
        existingPackage.Price = package.Price;
        existingPackage.Availability = package.Availability;
        existingPackage.Prerequisites = package.Prerequisites;

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var package = _packages.FirstOrDefault(p => p.Id == id);
        if (package != null)
        {
            _packages.Remove(package);
        }
        await Task.CompletedTask;
    }
}
