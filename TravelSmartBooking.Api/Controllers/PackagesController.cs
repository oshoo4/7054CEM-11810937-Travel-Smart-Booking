using Microsoft.AspNetCore.Mvc;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Models;

namespace TravelSmartBooking.Api.Controllers;

// SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
[Route("api/[controller]")]
[ApiController]
public class PackagesController : ControllerBase
{
    private readonly IPackageRepository _repository;
    private readonly IBookingFacade _bookingFacade;

    // DEPENDENCY INVERSION PRINCIPLE (SOLID Principles)
    public PackagesController(IPackageRepository repository, IBookingFacade bookingFacade)
    {
        _repository = repository;
        _bookingFacade = bookingFacade;
    }

    // GET (Endpoint)
    // Route: GET /api/Packages
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Package>>> GetPackages()
    {
        var packages = await _repository.GetAllAsync();
        return Ok(packages);
    }

    // GET (Endpoint)
    // Route: GET /api/Packages/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Package>> GetPackage(int id)
    {
        // SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
        var package = await _repository.GetByIdAsync(id);

        if (package == null)
        {
            return NotFound();
        }

        return package;
    }

    // POST (Endpoint)
    // Route: POST /api/Packages
    [HttpPost]
    public async Task<ActionResult<Package>> PostPackage(Package package)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
        var createdPackage = await _repository.AddAsync(package);
        return CreatedAtAction(nameof(GetPackage), new { id = createdPackage.Id }, createdPackage);
    }

    // PUT (Endpoint)
    // Route: PUT /api/Packages/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPackage(int id, Package package)
    {
        if (id != package.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
            await _repository.UpdateAsync(package);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE (Endpoint)
    // Route: DELETE /api/Packages/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePackage(int id)
    {
        var package = await _repository.GetByIdAsync(id);
        if (package == null)
        {
            return NotFound();
        }

        // SINGLE RESPONSIBILITY PRINCIPLE (SOLID Principles)
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
