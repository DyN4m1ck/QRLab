using QRLab.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace QRLab.View.ViewModels;
public sealed partial class GenerationViewModel(WorkspaceViewModel workspace)
    : ScreenViewModel("01", "Генерация кодов", "Набор 01 / Тест устойчивости")
{
    public WorkspaceViewModel Workspace { get; } = workspace;
    public IReadOnlyList<string> Modes { get; } = ["Один код", "Пакет"];
    public IReadOnlyList<string> Formats { get; } = ["QR-код", "Штрихкод"];
    public IReadOnlyList<string> Corrections { get; } = ["L • 7%", "M • 15%", "Q • 25%", "H • 30%"];
    public IReadOnlyList<string> FileFormats { get; } = ["PNG", "BMP", "JPEG"];
    public IReadOnlyList<string> Sources { get; } = ["Строка-шаблон с номером", "Произвольный текст"];
    [ObservableProperty] private string _mode = "Пакет";
    [ObservableProperty] private string _format = "QR-код";
    [ObservableProperty] private string _source = "Строка-шаблон с номером";
    [ObservableProperty] private string _template = "LAB-2026-{num}";
    [ObservableProperty] private decimal? _firstNumber = 1;
    [ObservableProperty] private decimal? _count = 120;
    [ObservableProperty] private decimal? _imageWidth = 1920;
    [ObservableProperty] private decimal? _imageHeight = 1080;
    [ObservableProperty] private decimal? _quietZone = 4;
    [ObservableProperty] private string _correction = "H • 30%";
    [ObservableProperty] private string _fileFormat = "PNG";
    [ObservableProperty] private bool _isLibraryOpen;
    [ObservableProperty] private bool _isExportOpen;
    [ObservableProperty] private string _exportDestination = "QRLab / Набор 01";
    [ObservableProperty] private bool _includeMetadata = true;
    public bool IsQr => Format == "QR-код";
    partial void OnFormatChanged(string value) => OnPropertyChanged(nameof(IsQr));
    public bool IsBatch => Mode == "Пакет";
    public string GenerateLabel => IsBatch ? $"＋  Подготовить {Count ?? 1} кодов" : "＋  Подготовить код";
    partial void OnModeChanged(string value) { OnPropertyChanged(nameof(IsBatch)); OnPropertyChanged(nameof(GenerateLabel)); }
    partial void OnCountChanged(decimal? value) => OnPropertyChanged(nameof(GenerateLabel));
    [RelayCommand] private void Generate()
    {
        if (string.IsNullOrWhiteSpace(Template)) { Workspace.Notice = "Введите содержимое или шаблон кода."; return; }
        Workspace.Generate(new(Template, (int)(FirstNumber ?? 1), IsBatch ? Math.Clamp((int)(Count ?? 1), 1, 500) : 1, Format, (int)(ImageWidth ?? 1920), (int)(ImageHeight ?? 1080), Correction, (int)(QuietZone ?? 4), Source == "Строка-шаблон с номером"));
    }
    [RelayCommand] private void OpenLibrary() => IsLibraryOpen = !IsLibraryOpen;
    [RelayCommand] private void LoadSet(LibrarySet set) { Count = set.Count; Template = set.Template; Mode = "Пакет"; Generate(); IsLibraryOpen = false; }
    [RelayCommand] private void ConfirmExport() { Workspace.Export(new(ExportDestination, FileFormat, IncludeMetadata)); IsExportOpen = false; }
    [RelayCommand] private void Export() => IsExportOpen = !IsExportOpen;
}
