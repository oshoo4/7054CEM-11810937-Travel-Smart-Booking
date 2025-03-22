using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelSmartBooking.Api.Data;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;
using TravelSmartBooking.Api.Repositories;
using TravelSmartBooking.Tests.Mocks;
using Xunit;

namespace TravelSmartBooking.Tests;

public class BookingFacadeTests
{
    private readonly BookingFacade _bookingFacade;
    private readonly IPackageRepository _mockPackageRepository;
    private readonly IBookingRepository _mockBookingRepository;
    private readonly IConfirmationSenderStrategyFactory _confirmationSenderFactory;

    public BookingFacadeTests()
    {
        var services = new ServiceCollection();
        services.AddTransient<IConfirmationSenderStrategy, EmailConfirmationSenderStrategy>();
        services.AddTransient<IConfirmationSenderStrategy, SmsConfirmationSenderStrategy>();
        services.AddTransient<
            IConfirmationSenderStrategyFactory,
            ConfirmationSenderStrategyFactory
        >();
        var serviceProvider = services.BuildServiceProvider();

        _mockPackageRepository = new MockPackageRepository(new PackageFactory());
        _mockBookingRepository = new MockBookingRepository();
        _confirmationSenderFactory = new ConfirmationSenderStrategyFactory(serviceProvider);
        _bookingFacade = new BookingFacade(
            _mockPackageRepository,
            _mockBookingRepository,
            _confirmationSenderFactory.GetConfirmationSender("email")
        );
    }

    [Fact]
    public async Task BookPackageAsync_SuccessfulBooking_ReturnsTrue()
    {
        // Given
        var packageId = 1;
        var customerDetails = "Test Customer";

        // When
        var result = await _bookingFacade.BookPackageAsync(packageId, customerDetails);

        // Then
        Assert.True(result);

        var package = await _mockPackageRepository.GetByIdAsync(packageId);
        Assert.Equal(9, package.Availability);

        var bookings = await _mockBookingRepository.GetAllAsync();
        var booking = bookings.Last();
        Assert.Equal(packageId, booking.PackageId);
        Assert.Equal(customerDetails, booking.CustomerDetails);
    }

    [Fact]
    public async Task BookPackageAsync_PackageNotFound_ReturnsFalse()
    {
        // Given
        var packageId = 999;
        var customerDetails = "Test Customer";

        // When
        var result = await _bookingFacade.BookPackageAsync(packageId, customerDetails);

        // Then
        Assert.False(result);

        var bookings = await _mockBookingRepository.GetAllAsync();
        Assert.Equal(2, bookings.Count());
    }

    [Fact]
    public async Task BookPackageAsync_PackageOutOfStock_ReturnsFalse()
    {
        // Given
        var packageId = 1;
        var customerDetails = "Test Customer";

        var package = await _mockPackageRepository.GetByIdAsync(packageId);
        package.Availability = 0;
        await _mockPackageRepository.UpdateAsync(package);

        // When
        var result = await _bookingFacade.BookPackageAsync(packageId, customerDetails);

        // Then
        Assert.False(result);

        var bookings = await _mockBookingRepository.GetAllAsync();
        Assert.Equal(2, bookings.Count());
    }

    [Fact]
    public async Task BookPackageAsync_NullCustomerDetails_ReturnsFalse()
    {
        // Given
        var packageId = 1;
        string customerDetails = null;

        // When
        var result = await _bookingFacade.BookPackageAsync(packageId, customerDetails);

        // Then
        Assert.False(result);

        var bookings = await _mockBookingRepository.GetAllAsync();
        Assert.Equal(2, bookings.Count());
    }

    [Fact]
    public async Task BookPackageAsync_EmptyCustomerDetails_ReturnsFalse()
    {
        // Given
        var packageId = 1;
        var customerDetails = "";

        // When
        var result = await _bookingFacade.BookPackageAsync(packageId, customerDetails);

        // Then
        Assert.False(result);

        var bookings = await _mockBookingRepository.GetAllAsync();
        Assert.Equal(2, bookings.Count());
    }
}
