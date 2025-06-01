using System.ComponentModel.DataAnnotations;
using APBD10.Exceptions;
using APBD10.Models.DTOs;
using APBD10.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD10.Controllers;

[ApiController]
[Route("api")]
public class TripsController(IDbService service) : ControllerBase
{
    [HttpGet("trips")]
    public async Task<IActionResult> GetTrips([Range(1, int.MaxValue)] int page = 1, 
        [Range(1, int.MaxValue)] int pageSize = 10)
    {
        try
        {
            return Ok(await service.GetTripsAsync(page, pageSize));
        }
        catch (PageExceededException exc)
        {
            return BadRequest(exc.Message);
        }
        
    }

    [HttpDelete("clients/{idClient:int}")]
    public async Task<IActionResult> DeleteClient([FromRoute] int idClient)
    {
        try
        {
            await service.DeleteClientAsync(idClient);
            return NoContent();
        } catch (ClientHasTripsException exc)
        {
            return BadRequest(exc.Message);
        }
    }

    [HttpPost("trips/{idTrip:int}/clients")]
    public async Task<IActionResult> CreateClient([FromRoute] int idTrip, [FromBody] ClientCreateDto clientDto)
    {
        try
        {
            await service.AddClientToTripAsync(clientDto);
            return NoContent();
        }
        catch (Exception exc) when (exc is ClientHasAlreadyExistsException or ClientHasSignedForCurrentTripException 
                                        or TripHasAlreadyTookPlaceException)
        {
            return BadRequest(exc.Message);
        }
        catch (TripNotExistsException exc)
        {
            return NotFound(exc.Message);       
        }
    }
}