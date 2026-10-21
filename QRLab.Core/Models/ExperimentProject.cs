namespace QRLab.Core.Models;

/// <summary>Проект лабораторной работы с исходными образцами.</summary>
public sealed class ExperimentProject
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Новый проект";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public List<CodeSample> Samples { get; } = [];
}

