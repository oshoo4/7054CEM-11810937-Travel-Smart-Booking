using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TravelSmartBooking.Api.Controllers;
using TravelSmartBooking.Api.Dtos;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;
using TravelSmartBooking.Api.Repositories;
using TravelSmartBooking.Tests.Mocks;
using Xunit;

namespace TravelSmartBooking.Tests;

public class BookingControllerTests
{
    private readonly BookingsController _controller;
    private readonly IBookingRepository _mockBookingRepository;
    private readonly IBookingFacade _mockBookingFacade;
    private readonly IPackageRepository _mockPackageRepository;
    private readonly IConfirmationSenderStrategyFactory _confirmationSenderFactory;

    public BookingControllerTests()
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
        _mockBookingFacade = new BookingFacade(
            _mockPackageRepository,
            _mockBookingRepository,
            _confirmationSenderFactory.GetConfirmationSender("email")
        );
        _controller = new BookingsController(
            _mockBookingRepository,
            _mockBookingFacade,
            _confirmationSenderFactory
        );
    }

    [Fact]
    public async Task GetBookings_ReturnsAllBookings()
    {
        var result = await _controller.GetBookings();
        var okResult = Assert.IsType<ActionResult<IEnumerable<Booking>>>(result);
        var resultValue = result.Value;
        if (resultValue != null)
        {
            Assert.Equal(2, resultValue.Count());
        }
    }

    [Fact]
    public async Task BookPackage_CallsBookingFacadeAndConfirmationSender()
    {
        // Given
        var packageId = 1;
        var bookingRequest = new BookingRequestDto
        {
            CustomerName = "Test User",
            CustomerEmail = "test@example.com",
            CustomerDetails = "Some details",
            ConfirmationMethod = "email",
        };

        // When
        var result = await _controller.BookPackage(packageId, bookingRequest);

        // Then
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task BookPackage_InvalidConfirmationMethod_ReturnsBadRequest()
    {
        // Given
        var packageId = 1;
        var bookingRequest = new BookingRequestDto
        {
            CustomerName = "Test Name",
            CustomerEmail = "test@example.com",
            CustomerDetails = "Test Details",
            ConfirmationMethod =
                "invalid"
            ,
        };

        // When
        var result = await _controller.BookPackage(packageId, bookingRequest);

        // Then
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task BookPackage_InvalidInput_ReturnsBadRequest()
    {
        // Given
        var packageId = 1;
        var bookingRequest = new BookingRequestDto
        {
            CustomerName = "",
            CustomerEmail = "test@example.com",
            CustomerDetails = "Test Details",
            ConfirmationMethod =
                "email"
            ,
        };

        _controller.ModelState.AddModelError("CustomerName", "The CustomerName field is required.");

        // When
        var result = await _controller.BookPackage(packageId, bookingRequest);
        // Then
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
