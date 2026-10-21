using CommunityToolkit.Mvvm.ComponentModel;
using QRLab.Core.Models;
using QRLab.Core.Services;
using QRLab.Image;

namespace QRLab.View.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
    public ExperimentProject Project { get; } = new();
    public string Version => $"v{typeof(MainWindowViewModel).Assembly.GetName().Version?.ToString(3)}";
    public string CoreStatus { get; } = new CoreModuleInfo().GetStatus();
    public string ImageStatus { get; } = new ImageModuleInfo().GetStatus();

    public IReadOnlyList<ScreenViewModel> Screens { get; } =
    [
        new("01", "Генератор", "Создание исходных QR-кодов и штрихкодов для лабораторной работы."),
        new("02", "Редактор искажений", "Настройка последовательности растровых искажений и повреждений."),
        new("03", "Эксперименты", "Проведение серий испытаний и сравнение устойчивости кодов."),
        new("04", "Инспектор", "Просмотр образцов и результатов декодирования.")
    ];

    [ObservableProperty]
    private ScreenViewModel _selectedScreen;

    public MainWindowViewModel() => _selectedScreen = Screens[0];
}

