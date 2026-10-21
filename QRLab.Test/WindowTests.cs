using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using QRLab.View.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using QRLab.Image.Demo;
using QRLab.View;
using QRLab.View.ViewModels;
using QRLab.View.Views;
using Xunit;
[assembly: AvaloniaTestApplication(typeof(QRLab.Test.TestAppBuilder))]
namespace QRLab.Test;
public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().WithInterFont().UseSkia()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
public sealed class WindowTests
{
    [AvaloniaFact]
    public void AllFourScreensLoadAndNavigationChangesVisibleContent()
    {
        var vm = new MainWindowViewModel(new PrototypeData());
        var window = new MainWindow { DataContext = vm };
        try
        {
            window.Show();
            foreach (var screen in vm.Screens)
            {
                var navigation = window.GetVisualDescendants().OfType<ListBox>().Single(l => l.Classes.Contains("nav"));
                navigation.SelectedItem = screen;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
                Assert.Same(screen, vm.SelectedScreen);
                var navIcons = navigation.GetVisualDescendants().OfType<SvgIcon>().ToArray();
                Assert.Equal(4, navIcons.Length);
                Assert.All(navIcons, icon =>
                {
                    var item = icon.GetVisualAncestors().OfType<ListBoxItem>().First();
                    var expected = Color.Parse(item.IsSelected ? "#75D9BE" : "#86928F");
                    Assert.Equal(expected, Assert.IsAssignableFrom<ISolidColorBrush>(icon.Foreground).Color);
                    var caption = item.GetVisualDescendants().OfType<TextBlock>().Single();
                    Assert.Equal(expected, Assert.IsAssignableFrom<ISolidColorBrush>(caption.Foreground).Color);
                });
                var text = window.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();
                Assert.Contains(screen.Title, text);
                Assert.Contains(screen.Description, text);
                Assert.DoesNotContain(text, t => t?.Contains("Функциональность будет добавлена") == true);
                if (screen != vm.Reading) Assert.NotEmpty(window.GetVisualDescendants().OfType<QRLab.View.Controls.CodePreview>());
                if (Environment.GetEnvironmentVariable("QRLAB_CAPTURE") is { } directory)
                {
                    Directory.CreateDirectory(directory);
                    using var bitmap = new RenderTargetBitmap(new PixelSize(1440, 940), new Vector(96, 96));
                    bitmap.Render(window); using var output = File.Create(Path.Combine(directory, $"screen-{screen.Number}.png")); bitmap.Save(output, PngBitmapEncoderOptions.Default);
                }
            }
        }
        finally { window.Close(); }
    }
    [AvaloniaFact]
    public void NavigationButtonsAndFrameSelectionKeepSharedSample()
    {
        var vm = new MainWindowViewModel(new PrototypeData());
        var window = new MainWindow { DataContext = vm };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var button = window.GetVisualDescendants().OfType<Button>().Single(b => b.Command == vm.NavigateCommand && Equals(b.CommandParameter, "02"));
            Assert.NotNull(button.Command); button.Command.Execute(button.CommandParameter);
            Dispatcher.UIThread.RunJobs();
            Assert.Same(vm.Mutations, vm.SelectedScreen);
            Assert.NotNull(vm.Workspace.SelectedSample);
            vm.Mutations.AngleX = 19;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(19, vm.Mutations.Operations.Last().Options!.AngleX);
            Assert.NotNull(vm.Mutations.SelectedOperation);
            vm.Mutations.SelectedOperation = vm.Mutations.Frames[0];
            Assert.Equal(0, vm.Mutations.Stage);
            vm.Workspace.NextCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();
            Assert.NotNull(vm.Workspace.SelectedSample);
            vm.NavigateCommand.Execute("03");
            vm.Reading.RunCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(120, vm.Reading.ResultsChecked);
            var selected = vm.Workspace.SelectedSample;
            vm.Reading.Filter = "Ошибки"; vm.Reading.Search = "no-match";
            Dispatcher.UIThread.RunJobs();
            Assert.Same(selected, vm.Workspace.SelectedSample);
            vm.NavigateCommand.Execute("01"); Dispatcher.UIThread.RunJobs();
            vm.Generation.OpenLibraryCommand.Execute(null); Dispatcher.UIThread.RunJobs();
            Assert.Contains(window.GetVisualDescendants().OfType<Button>(), b => b.Command == vm.Generation.LoadSetCommand);
            vm.Generation.LoadSetCommand.Execute(vm.Workspace.LibrarySets[0]); Dispatcher.UIThread.RunJobs();
            Assert.Equal(12, vm.Workspace.Samples.Count);
            vm.Generation.ExportCommand.Execute(null); Dispatcher.UIThread.RunJobs();
            Assert.Contains(window.GetVisualDescendants().OfType<Button>(), b => b.Command == vm.Generation.ConfirmExportCommand);
            vm.Generation.ConfirmExportCommand.Execute(null); Dispatcher.UIThread.RunJobs();
            Assert.False(vm.Generation.IsExportOpen);
            Assert.Contains("Демоэкспорт", vm.Workspace.Notice);
        }
        finally { window.Close(); }
    }
}
