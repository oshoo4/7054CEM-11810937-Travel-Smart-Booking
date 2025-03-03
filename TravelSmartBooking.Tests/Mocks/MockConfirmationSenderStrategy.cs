using TravelSmartBooking.Api.Interfaces;

namespace TravelSmartBooking.Tests.Mocks;

public class MockConfirmationSenderStrategy : IConfirmationSenderStrategy
{
    public bool SendConfirmationAsyncWasCalled { get; private set; } = false;
    public string LastCustomerDetails { get; private set; } = string.Empty;
    public string LastMessage { get; private set; } = string.Empty;

    public async Task SendConfirmationAsync(string customerDetails, string message)
    {
        SendConfirmationAsyncWasCalled = true;
        LastCustomerDetails = customerDetails;
        LastMessage = message;
        await Task.CompletedTask;
    }
}
