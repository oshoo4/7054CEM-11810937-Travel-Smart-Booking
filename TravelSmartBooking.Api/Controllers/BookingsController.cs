using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TravelSmartBooking.Api.Dtos;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;
using TravelSmartBooking.Api.Repositories;

namespace TravelSmartBooking.Api.Controllers;

// SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingFacade _bookingFacade;
    private readonly IConfirmationSenderStrategyFactory _confirmationSenderFactory;

    // DEPENDENCY INVERSION PRINCIPLE (SOLID Principles)
    public BookingsController(
        IBookingRepository bookingRepository,
        IBookingFacade bookingFacade,
        IConfirmationSenderStrategyFactory confirmationSenderFactory
    )
    {
        _bookingRepository = bookingRepository;
        _bookingFacade = bookingFacade;
        _confirmationSenderFactory = confirmationSenderFactory;
    }

    // GET (Endpoint)
    // Route: GET /api/Bookings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
    {
        // SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
        return Ok(await _bookingRepository.GetAllAsync());
    }

    // POST (Endpoint)
    // Route: POST /api/Bookings/{packageId}/book
    [HttpPost("{packageId}/book")]
    public async Task<IActionResult> BookPackage(
        int packageId,
        [FromBody] BookingRequestDto bookingRequest
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        IConfirmationSenderStrategy confirmationSender;
        try
        {
            // STRATEGY (GoF Patterns)
            confirmationSender = _confirmationSenderFactory.GetConfirmationSender(
                bookingRequest.ConfirmationMethod
            );
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid confirmation method.");
        }

        // FACADE (GoF Patterns)
        // SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
        var bookingFacade = new BookingFacade(
            _bookingFacade.GetPackageRepository(),
            _bookingRepository,
            confirmationSender
        );

        return Ok();
    }
}
