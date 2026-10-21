namespace QRLab.Core.Models;

public sealed class CodeSample
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Новый образец";
    public string Content { get; set; } = string.Empty;
    public string Format { get; set; } = "QR";
    public List<MutationStep> Mutations { get; } = [];
}

