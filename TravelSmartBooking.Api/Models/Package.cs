namespace TravelSmartBooking.Api.Models;

public class Package
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public int Availability { get; set; }
    public string Prerequisites { get; set; } = String.Empty;
}
