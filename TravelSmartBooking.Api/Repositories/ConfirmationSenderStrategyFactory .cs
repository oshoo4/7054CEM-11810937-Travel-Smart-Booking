using System;
using Microsoft.Extensions.DependencyInjection;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Repositories;

namespace TravelSmartBooking.Api.Repositories;

// STRATEGY (GoF Patterns)
// FACTORY (GoF Patterns)
// SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
public class ConfirmationSenderStrategyFactory : IConfirmationSenderStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    // DEPENDENCY INVERSION PRINCIPLE (SOLID Principles)
    public ConfirmationSenderStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IConfirmationSenderStrategy GetConfirmationSender(string method)
    {
        // OPEN-CLOSED PRINCIPLE (SOLID Principles)
        switch (method.ToLowerInvariant())
        {
            case "email":
                return new EmailConfirmationSenderStrategy();
            case "sms":
                return new SmsConfirmationSenderStrategy();
            default:
                throw new ArgumentException($"Invalid confirmation method: {method}");
        }
    }
}
