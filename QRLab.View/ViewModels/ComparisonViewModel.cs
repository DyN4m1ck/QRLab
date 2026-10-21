using CommunityToolkit.Mvvm.ComponentModel;
namespace QRLab.View.ViewModels;
public sealed partial class ComparisonViewModel
    : ScreenViewModel
{
    public WorkspaceViewModel Workspace { get; }
    public ComparisonViewModel(WorkspaceViewModel workspace) : base("04", "Сравнение образца", "Набор 01 / Исходник и результат мутаций")
    {
        Workspace = workspace;
        Workspace.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(Workspace.SelectedSample)) Refresh(); };
    }
    public bool[,]? PreviewModules => Workspace.Frame(Workspace.SelectedSample, Workspace.SelectedSample?.Operations.LastOrDefault()?.Options ?? new(0));
    public bool[,]? DifferenceModules => Workspace.Frame(Workspace.SelectedSample, Workspace.SelectedSample?.Operations.LastOrDefault()?.Options ?? new(0), true);
    public double OverlayFraction => OverlayOpacity / 100;
    partial void OnOverlayOpacityChanged(double value) => OnPropertyChanged(nameof(OverlayFraction));
    public void Refresh() { OnPropertyChanged(nameof(PreviewModules)); OnPropertyChanged(nameof(DifferenceModules)); }
    public IReadOnlyList<string> Modes { get; } = ["Рядом", "Наложение", "Разница"];
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsSideBySide), nameof(IsOverlay), nameof(IsDifference))]
    private string _mode = "Рядом";
    [ObservableProperty] private double _overlayOpacity = 50;
    [ObservableProperty] private double _zoom = 100;
    public bool IsSideBySide => Mode == "Рядом";
    public bool IsOverlay => Mode == "Наложение";
    public bool IsDifference => Mode == "Разница";
}
