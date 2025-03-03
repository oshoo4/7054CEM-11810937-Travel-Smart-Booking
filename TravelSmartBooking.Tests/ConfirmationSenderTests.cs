using System;
using System.IO;
using TravelSmartBooking.Api.Repositories;
using Xunit;

namespace TravelSmartBooking.Tests;

public class ConfirmationSenderTests
{
    [Fact]
    public async Task EmailConfirmationSender_SendConfirmationAsync_WritesToConsole()
    {
        // Given
        var sender = new EmailConfirmationSenderStrategy();
        var customerDetails = "test@example.com";
        var message = "Your booking is confirmed!";

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // When
        await sender.SendConfirmationAsync(customerDetails, message);

        // Then
        var output = consoleOutput.ToString();
        Assert.Contains($"Sending email to: {customerDetails}", output);
        Assert.Contains($"Message: {message}", output);
    }

    [Fact]
    public async Task SmsConfirmationSender_SendConfirmationAsync_WritesToConsole()
    {
        // Given
        var sender = new SmsConfirmationSenderStrategy();
        var customerDetails = "123-456-7890";
        var message = "Your booking is confirmed!";

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // When
        await sender.SendConfirmationAsync(customerDetails, message);

        // Then
        var output = consoleOutput.ToString();
        Assert.Contains($"Sending SMS to: {customerDetails}", output);
        Assert.Contains($"Message: {message}", output);
    }
}
