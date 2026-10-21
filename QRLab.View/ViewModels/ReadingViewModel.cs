using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace QRLab.View.ViewModels;
public sealed partial class ReadingViewModel : ScreenViewModel
{
    public WorkspaceViewModel Workspace { get; }
    public IReadOnlyList<string> Filters { get; } = ["Все", "Успешные", "Ошибки"];
    public IReadOnlyList<string> Decoders { get; } = ["ZXing • 2.3.0", "OpenCV • QRCodeDetector"];
    public IReadOnlyList<string> Sources { get; } = ["Последний кадр", "Исходник", "Все кадры"];
    [ObservableProperty] private string _filter = "Все";
    [ObservableProperty] private string _search = "";
    [ObservableProperty] private string _decoder = "ZXing • 2.3.0";
    [ObservableProperty] private string _source = "Последний кадр";
    [ObservableProperty] private decimal? _timeout = 500;
    [ObservableProperty] private bool _enhanced = true;
    [ObservableProperty] private bool _isExportOpen;
    [ObservableProperty] private string _exportDestination = "QRLab / Результаты";
    [ObservableProperty] private SampleViewModel? _selectedResult;
    partial void OnSelectedResultChanged(SampleViewModel? value) { if (value is not null) Workspace.SelectedSample = value; }
    public IReadOnlyList<SampleViewModel> Results => Workspace.Samples.Where(s =>
        (Filter == "Все" || Filter == "Успешные" && s.Result is { Success: true } || Filter == "Ошибки" && s.IsFailure) &&
        (s.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) || s.Content.Contains(Search, StringComparison.OrdinalIgnoreCase))).ToArray();
    public bool NoResults => Results.Count == 0;
    public int Checked => Workspace.Samples.Count(s => s.Result is not null);
    public int Success => Workspace.Samples.Count(s => s.Result is { Success: true });
    public int Failed => Workspace.Samples.Count(s => s.IsFailure);
    public string Average => Checked == 0 ? "—" : $"{Workspace.Samples.Where(s => s.Result is not null).Average(s => s.Result!.Milliseconds):N1} мс";
    public string Rate => Checked == 0 ? "Нет результатов" : $"{100d * Success / Checked:N1}% от проверенных";
    public ReadingViewModel(WorkspaceViewModel workspace) : base("03", "Считывание и статистика", "Набор 01 / Результаты распознавания")
    {
        Workspace = workspace; _selectedResult = workspace.SelectedSample;
        Workspace.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(Workspace.SelectedSample) && Workspace.SelectedSample is not null) SelectedResult = Workspace.SelectedSample; };
        Workspace.Samples.CollectionChanged += (_, _) => Refresh();
    }
    partial void OnFilterChanged(string value) { OnPropertyChanged(nameof(Results)); OnPropertyChanged(nameof(NoResults)); }
    partial void OnSearchChanged(string value) { OnPropertyChanged(nameof(Results)); OnPropertyChanged(nameof(NoResults)); }
    [RelayCommand] private void Run() { Workspace.Recognize(new(Decoder, Source, (int)(Timeout ?? 500), Enhanced)); Refresh(); }
    [RelayCommand] private void ConfirmExport() { Workspace.Export(new(ExportDestination, "CSV", true)); IsExportOpen = false; }
    [RelayCommand] private void Export() => IsExportOpen = !IsExportOpen;
    private void Refresh()
    {
        foreach (var name in new[] { nameof(Results), nameof(NoResults), nameof(Checked), nameof(Success), nameof(Failed), nameof(Average), nameof(Rate) }) OnPropertyChanged(name);
    }
}
