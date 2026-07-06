namespace FitnessChallenge.Api.Application.Dtos;

public sealed class IngestActivityRequest
{
    public string? UserId { get; set; }
    public string? Datetime { get; set; }
    public string? Sport { get; set; }
    public int? Steps { get; set; }
    public decimal? Distance { get; set; }
    public string? Duration { get; set; }
}
