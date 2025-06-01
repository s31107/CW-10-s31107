namespace APBD10.Models.DTOs;

public class TripGetDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<CountriesGetDto> Countries { get; set; } = null!;
    public IEnumerable<ClientsGetDto> Clients { get; set; } = null!;
}