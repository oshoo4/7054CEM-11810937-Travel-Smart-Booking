using Microsoft.EntityFrameworkCore;
using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Data;

public class TravelSmartContext : DbContext
{
    public TravelSmartContext(DbContextOptions<TravelSmartContext> options)
        : base(options) { }

    public DbSet<Package> Packages { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Package>()
            .HasData(
                new Package
                {
                    Id = 1,
                    Name = "Paris Getaway",
                    Description = "3 days in Paris",
                    Price = 500,
                    Availability = 10,
                    Prerequisites = "Passport",
                },
                new Package
                {
                    Id = 2,
                    Name = "Rome Adventure",
                    Description = "5 days in Rome",
                    Price = 800,
                    Availability = 5,
                    Prerequisites = "Passport",
                }
            );
    }
}
