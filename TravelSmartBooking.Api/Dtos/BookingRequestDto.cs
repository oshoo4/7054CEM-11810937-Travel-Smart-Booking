namespace TravelSmartBooking.Api.Dtos;

public class BookingRequestDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerDetails { get; set; } = string.Empty;
    public string ConfirmationMethod { get; set; } = string.Empty;
}
