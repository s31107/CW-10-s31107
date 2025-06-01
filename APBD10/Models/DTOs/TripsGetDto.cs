namespace APBD10.Models.DTOs;

public class TripsGetDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public IEnumerable<TripGetDto> Trips { get; set; } = null!;
}