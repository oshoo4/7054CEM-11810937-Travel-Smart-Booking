using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TravelSmartBooking.Api.Controllers;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;
using TravelSmartBooking.Api.Repositories;
using TravelSmartBooking.Tests.Mocks;
using Xunit;

namespace TravelSmartBooking.Tests;

public class PackagesControllerTests
{
    private readonly IPackageRepository _mockPackageRepository;
    private readonly PackagesController _packagesController;
    private readonly IPackageFactory _packageFactory;
    private readonly IBookingFacade _mockBookingFacade;
    private readonly IBookingRepository _mockBookingRepository;
    private readonly IConfirmationSenderStrategyFactory _confirmationSenderFactory;

    public PackagesControllerTests()
    {
        var services = new ServiceCollection();
        services.AddTransient<IConfirmationSenderStrategy, EmailConfirmationSenderStrategy>();
        services.AddTransient<IConfirmationSenderStrategy, SmsConfirmationSenderStrategy>();
        services.AddTransient<
            IConfirmationSenderStrategyFactory,
            ConfirmationSenderStrategyFactory
        >();
        var serviceProvider = services.BuildServiceProvider();

        _packageFactory = new PackageFactory();
        _mockPackageRepository = new MockPackageRepository(_packageFactory);
        _mockBookingRepository = new MockBookingRepository();
        _confirmationSenderFactory = new ConfirmationSenderStrategyFactory(serviceProvider);
        _mockBookingFacade = new MockBookingFacade(
            _mockPackageRepository,
            _mockBookingRepository,
            _confirmationSenderFactory.GetConfirmationSender("email")
        );

        _packagesController = new PackagesController(_mockPackageRepository, _mockBookingFacade);
    }

    [Fact]
    public async Task GetPackages_ReturnsAllPackages()
    {
        var result = await _packagesController.GetPackages();

        var okResult = Assert.IsType<ActionResult<IEnumerable<Package>>>(result);
        var resultValue = result.Value;
        if (resultValue != null)
        {
            Assert.Equal(2, resultValue.Count());
        }
    }


    [Fact]
    public async Task GetPackage_ValidId_ReturnsPackage()
    {
        var result = await _packagesController.GetPackage(1);

        var okResult = Assert.IsType<ActionResult<Package>>(result);
        var package = Assert.IsType<Package>(okResult.Value);
        Assert.Equal(1, package.Id);
    }

    [Fact]
    public async Task GetPackage_InvalidId_ReturnsNotFound()
    {
        var result = await _packagesController.GetPackage(999);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostPackage_ValidInput_CreatesPackage()
    {
        var newPackage = new Package
        {
            Name = "New Package",
            Description = "Description",
            Price = 100,
            Availability = 50,
            Prerequisites = "Passport",
        };

        var result = await _packagesController.PostPackage(newPackage);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdPackage = Assert.IsType<Package>(createdAtActionResult.Value);
        Assert.Equal("New Package", createdPackage.Name);
        Assert.NotEqual(0, createdPackage.Id);

        var retrievedPackage = await _mockPackageRepository.GetByIdAsync(createdPackage.Id);
        Assert.NotNull(retrievedPackage);
    }

    [Fact]
    public async Task PostPackage_InvalidInput_ReturnsBadRequest()
    {
        var newPackage = new Package
        {
            Name = "",
            Description = "New Description",
            Price = 150,
            Availability = 20,
            Prerequisites = "None",
        };
        _packagesController.ModelState.AddModelError("Name", "The Name field is required.");

        var result = await _packagesController.PostPackage(newPackage);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task PutPackage_ValidInput_UpdatesPackage()
    {
        var packageId = 1;
        var updatedPackage = new Package
        {
            Id = packageId,
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 200,
            Availability = 25,
            Prerequisites = "Visa",
        };

        var result = await _packagesController.PutPackage(packageId, updatedPackage);
        Assert.IsType<NoContentResult>(result);

        var retrievedPackage = await _mockPackageRepository.GetByIdAsync(packageId);
        Assert.Equal("Updated Name", retrievedPackage.Name);
    }

    [Fact]
    public async Task PutPackage_MismatchedId_ReturnsBadRequest()
    {
        var result = await _packagesController.PutPackage(1, new Package { Id = 2 });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutPackage_InvalidInput_ReturnsBadRequest()
    {
        // Given
        var packageId = 1;
        var updatedPackage = new Package
        {
            Id = packageId,
            Name = "",
            Description = "Updated Description",
            Price = 120,
            Availability = 15,
            Prerequisites = "Updated Prerequisites",
        };
        _packagesController.ModelState.AddModelError("Name", "The Name field is required.");

        // When
        var result = await _packagesController.PutPackage(packageId, updatedPackage);
        // Then
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PutPackage_PackageNotFound_ReturnsNotFound()
    {
        var result = await _packagesController.PutPackage(999, new Package { Id = 999 });
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeletePackage_ValidId_DeletesPackage()
    {
        var result = await _packagesController.DeletePackage(1);
        Assert.IsType<NoContentResult>(result);

        var deletedPackage = await _mockPackageRepository.GetByIdAsync(1);
        Assert.Null(deletedPackage);
    }

    [Fact]
    public async Task DeletePackage_InvalidId_ReturnsNotFound()
    {
        var result = await _packagesController.DeletePackage(999);
        Assert.IsType<NotFoundResult>(result);
    }

    // [Fact]
    // public async Task BookPackage_ValidInput_ReturnsOk()
    // {
    //     var packageId = 1;
    //     var result = await _packagesController.BookPackage(packageId, "Customer Details");
    //     Assert.IsType<OkResult>(result);

    //     var mockFacade = (MockBookingFacade)_mockBookingFacade;
    //     Assert.True(mockFacade.BookPackageAsyncWasCalled);
    //     Assert.Equal(packageId, mockFacade.LastBookedPackageId);
    //     Assert.Equal("Customer Details", mockFacade.LastCustomerDetails);
    // }

    // [Fact]
    // public async Task BookPackage_BookingFails_ReturnsBadRequest()
    // {
    //     // Given
    //     var packageId = 1;
    //     var customerDetails = "Test Customer Details";

    //     var mockFacade = (MockBookingFacade)_mockBookingFacade;
    //     mockFacade.BookPackageAsyncResult = false;

    //     // When
    //     var result = await _packagesController.BookPackage(packageId, customerDetails);

    //     // Then
    //     Assert.IsType<BadRequestObjectResult>(result); // Expecting BadRequestObjectResult, not BadRequestResult
    // }
}
