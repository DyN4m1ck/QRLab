using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using QRLab.View;
using QRLab.View.ViewModels;
using QRLab.View.Views;
using Xunit;

[assembly: AvaloniaTestApplication(typeof(QRLab.Test.TestAppBuilder))]

namespace QRLab.Test;

public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

public sealed class WindowTests
{
    [AvaloniaFact]
    public void WindowLoadsAndNavigationChangesVisibleContent()
    {
        var vm = new MainWindowViewModel();
        var window = new MainWindow { DataContext = vm };
        try
        {
            window.Show();
            var navigation = window.GetVisualDescendants().OfType<ListBox>().Single();
            foreach (var screen in vm.Screens)
            {
                navigation.SelectedItem = screen;
                Dispatcher.UIThread.RunJobs();
                Assert.Same(screen, vm.SelectedScreen);
                var text = window.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();
                Assert.Contains(screen.Description, text);
                Assert.Contains(screen.Placeholder, text);
                Assert.Contains(vm.CoreStatus, text);
                Assert.Contains(vm.ImageStatus, text);
            }
        }
        finally { window.Close(); }
    }
}
