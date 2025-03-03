namespace TravelSmartBooking.Api.Interfaces;

// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
// DEPENDENCY INVERSION PRINCIPLE(SOLID Principles)
public interface IConfirmationSenderStrategy
{
    Task SendConfirmationAsync(string customerDetails, string message);
}
