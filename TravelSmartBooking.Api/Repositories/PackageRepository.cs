using Microsoft.EntityFrameworkCore;
using TravelSmartBooking.Api.Data;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly TravelSmartContext _context;
    private readonly IPackageFactory _packageFactory;

    public PackageRepository(TravelSmartContext context, IPackageFactory packageFactory)
    {
        _context = context;
        _packageFactory = packageFactory;
    }

    public async Task<IEnumerable<Package>> GetAllAsync()
    {
        return await _context.Packages.ToListAsync();
    }

    public async Task<Package?> GetByIdAsync(int id)
    {
        return await _context.Packages.FindAsync(id);
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
        _context.Packages.Add(newPackage);
        await _context.SaveChangesAsync();
        return newPackage;
    }

    public async Task UpdateAsync(Package package)
    {
        _context.Entry(package).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package != null)
        {
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
        }
    }
}
