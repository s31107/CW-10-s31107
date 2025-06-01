using APBD10.Data;
using APBD10.Exceptions;
using APBD10.Models;
using APBD10.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace APBD10.Services;

public interface IDbService
{
    public Task<TripsGetDto> GetTripsAsync(int page, int pageSize);
    public Task DeleteClientAsync(int idClient);
    public Task AddClientToTripAsync(ClientCreateDto clientDto);
}

public class DbService(MasterContext data) : IDbService
{
    public async Task<TripsGetDto> GetTripsAsync(int page, int pageSize)
    {
        var allRecords = await data.Trips.CountAsync();
        var allPages = (int)Math.Ceiling(allRecords / (double)pageSize);
        if ((page - 1) * pageSize > allRecords)
        {
            throw new PageExceededException(
                $"Page {page} of size {pageSize} exceeds the number of trips in the database.");
        }
        var trips = await data.Trips.Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(
            trip => trip.DateFrom).Select(trip => new TripGetDto
        {
            Name = trip.Name,
            Description = trip.Description,
            DateFrom = trip.DateFrom,
            DateTo = trip.DateTo,
            MaxPeople = trip.MaxPeople,
            Clients = trip.ClientTrips.Select(clientTrip => new ClientsGetDto
            {
                FirstName = clientTrip.IdClientNavigation.FirstName,
                LastName = clientTrip.IdClientNavigation.LastName
            }),
            Countries = trip.IdCountries.Select(country => new CountriesGetDto
            {
                Name = country.Name
            })
        }).ToListAsync();
        return new TripsGetDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            Trips = trips
        };
    }
    
    public async Task DeleteClientAsync(int idClient)
    {
        if (await data.ClientTrips.AnyAsync(clientTrip => clientTrip.IdClient == idClient))
        {
            throw new ClientHasTripsException($"Client with id {idClient} has trips, so he can't be deleted.");
        }
        await data.Clients.Where(client => client.IdClient == idClient).ExecuteDeleteAsync();
    }

    public async Task AddClientToTripAsync(ClientCreateDto clientDto)
    {
        if (await data.Clients.AnyAsync(client => client.Pesel == clientDto.Pesel))
        {
            throw new ClientHasAlreadyExistsException("Client with this Pesel already exists.");
        }

        if (await data.ClientTrips.AnyAsync(clientTrip => clientTrip.IdClientNavigation.Pesel == clientDto.Pesel && 
                                                          clientTrip.IdTrip == clientDto.IdTrip))
        {
            throw new ClientHasSignedForCurrentTripException("Client has already signed for this trip.");
        }
        
        var trip = await data.Trips.FirstOrDefaultAsync(trip => trip.IdTrip == clientDto.IdTrip);
        
        if (trip is null || trip.Name != clientDto.TripName)
        {
            throw new TripNotExistsException("Trip with this id does not exist.");
        }

        if (trip.DateFrom <= DateTime.Now)
        {
            throw new TripHasAlreadyTookPlaceException("Trip has already taken place.");
        }

        await data.Clients.AddAsync(new Client
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Email = clientDto.Email,
            Telephone = clientDto.Telephone,
            Pesel = clientDto.Pesel,
            ClientTrips =
            [
                new ClientTrip
                {
                    IdTrip = clientDto.IdTrip,
                    PaymentDate = clientDto.PaymentDate,
                    RegisteredAt = DateTime.Now
                }
            ]
        });
        await data.SaveChangesAsync();
    }
}