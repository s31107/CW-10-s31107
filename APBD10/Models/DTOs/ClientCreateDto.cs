using System.ComponentModel.DataAnnotations;

namespace APBD10.Models.DTOs;

public class ClientCreateDto
{
    [MaxLength(120)]
    public required string FirstName { get; set; }
    [MaxLength(120)]
    public required string LastName { get; set; }
    [MaxLength(120)]
    [EmailAddress]
    public required string Email { get; set; }
    [MaxLength(120)]
    [Phone]
    public required string Telephone { get; set; }
    [MaxLength(120)]
    public required string Pesel { get; set; }
    public required int IdTrip { get; set; }
    [MaxLength(120)]
    public required string TripName { get; set; }
    public DateTime? PaymentDate { get; set; }
}