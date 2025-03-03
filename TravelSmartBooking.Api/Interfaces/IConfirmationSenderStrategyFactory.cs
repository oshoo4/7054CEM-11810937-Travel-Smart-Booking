namespace TravelSmartBooking.Api.Interfaces;

// INTERFACE SEGREGATION PRINCIPLE (SOLID Principles)
// DEPENDENCY INVERSION PRINCIPLE(SOLID Principles)
public interface IConfirmationSenderStrategyFactory
{
    IConfirmationSenderStrategy GetConfirmationSender(string method);
}
