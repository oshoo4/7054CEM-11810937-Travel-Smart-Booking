using TravelSmartBooking.Api.Interfaces;

namespace TravelSmartBooking.Api.Repositories;

// STRATEGY (GoF Patterns)
// SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
// LISKOV SUBSTITUTION PRINCIPLE (SOLID Principles)
// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
// OPEN-CLOSED PRINCIPLE(SOLID Principles)
public class EmailConfirmationSenderStrategy : IConfirmationSenderStrategy
{
    public async Task SendConfirmationAsync(string customerDetails, string message)
    {
        Console.WriteLine($"Sending email to: {customerDetails}");
        Console.WriteLine($"Message: {message}");
        await Task.Delay(100);
    }
}
