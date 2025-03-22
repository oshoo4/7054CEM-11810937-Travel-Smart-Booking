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
        // Given
        var result = await _packagesController.GetPackages();

        // When
        var resultValue = result.Value;

        // Then
        if (resultValue != null)
        {
            Assert.Equal(2, resultValue.Count());
        }
    }

    [Fact]
    public async Task GetPackage_ValidId_ReturnsPackage()
    {
        // Given
        var result = await _packagesController.GetPackage(1);

        // Then
        var okResult = Assert.IsType<ActionResult<Package>>(result);
        var package = Assert.IsType<Package>(okResult.Value);
        Assert.Equal(1, package.Id);
    }

    [Fact]
    public async Task GetPackage_InvalidId_ReturnsNotFound()
    {
        // Given
        int invalidID = 999;

        // When
        var result = await _packagesController.GetPackage(invalidID);

        // Then
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostPackage_ValidInput_CreatesPackage()
    {
        // Given
        var newPackage = new Package
        {
            Name = "New Package",
            Description = "Description",
            Price = 100,
            Availability = 50,
            Prerequisites = "Passport",
        };

        // When
        var result = await _packagesController.PostPackage(newPackage);

        // Then
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
        // Given
        var newPackage = new Package
        {
            Name = "",
            Description = "New Description",
            Price = 150,
            Availability = 20,
            Prerequisites = "None",
        };
        _packagesController.ModelState.AddModelError("Name", "The Name field is required.");

        // When
        var result = await _packagesController.PostPackage(newPackage);

        // Then
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task PutPackage_ValidInput_UpdatesPackage()
    {
        // Given
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

        // When
        var result = await _packagesController.PutPackage(packageId, updatedPackage);

        // Then
        Assert.IsType<NoContentResult>(result);

        var retrievedPackage = await _mockPackageRepository.GetByIdAsync(packageId);
        Assert.Equal("Updated Name", retrievedPackage.Name);
    }

    [Fact]
    public async Task PutPackage_MismatchedId_ReturnsBadRequest()
    {
        // Given
        int packageID = 1;
        var package = new Package { Id = 2 };

        // When
        var result = await _packagesController.PutPackage(packageID, package);

        // Then
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
        // Given
        int invalidPackageID = 999;
        var invalidPackage = new Package { Id = 999 };

        // When
        var result = await _packagesController.PutPackage(invalidPackageID, invalidPackage);

        // Then
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeletePackage_ValidId_DeletesPackage()
    {
        // Given
        int packageID = 1;

        // When
        var result = await _packagesController.DeletePackage(packageID);

        // Then
        Assert.IsType<NoContentResult>(result);

        var deletedPackage = await _mockPackageRepository.GetByIdAsync(packageID);
        Assert.Null(deletedPackage);
    }

    [Fact]
    public async Task DeletePackage_InvalidId_ReturnsNotFound()
    {
        // Given
        int packageID = 999;

        // When
        var result = await _packagesController.DeletePackage(packageID);

        // Then
        Assert.IsType<NotFoundResult>(result);
    }
}
