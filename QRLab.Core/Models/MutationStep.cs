namespace QRLab.Core.Models;

/// <summary>Описание операции; обработка изображения здесь не выполняется.</summary>
public sealed class MutationStep
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public double Intensity { get; set; }
}

