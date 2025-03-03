namespace TravelSmartBooking.Api.Models;

public class Booking
{
    public int Id { get; set; }
    public int PackageId { get; set; }
    public DateTime BookingDate { get; set; }
    public string CustomerDetails { get; set; }
}
