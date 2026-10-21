namespace QRLab.View.ViewModels;

/// <summary>Содержимое одного из разделов на этапе каркаса.</summary>
public sealed record ScreenViewModel(string Number, string Title, string Description)
{
    public string Placeholder => "Функциональность будет добавлена позже";
}

