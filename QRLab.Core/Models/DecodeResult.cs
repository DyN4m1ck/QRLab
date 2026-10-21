namespace QRLab.Core.Models;

public sealed class DecodeResult
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid SampleId { get; init; }
    public bool IsSuccess { get; init; }
    public string? DecodedText { get; init; }
    public string? ErrorMessage { get; init; }
    public TimeSpan Duration { get; init; }
}

