using QRLab.Image.Demo;
using QRLab.View.ViewModels;
using Xunit;
namespace QRLab.Test;
public sealed class PrototypeTests
{
    private static MainWindowViewModel Create() => new(new PrototypeData());
    [Fact]
    public void GenerationChangesCollectionAndKeepsFormsAcrossNavigation()
    {
        var vm = Create();
        vm.Generation.Count = 12; vm.Generation.FirstNumber = 20; vm.Generation.Template = "TEST-{num}";
        vm.Generation.GenerateCommand.Execute(null);
        Assert.Equal(12, vm.Workspace.Samples.Count);
        Assert.Equal("TEST-0020", vm.Workspace.SelectedSample!.Content);
        vm.NavigateCommand.Execute("02"); vm.NavigateCommand.Execute("01");
        Assert.Equal(12, vm.Generation.Count);
        vm.Generation.Mode = "Один код"; vm.Generation.GenerateCommand.Execute(null);
        Assert.Single(vm.Workspace.Samples);
    }
    [Fact]
    public void MutationHistorySupportsUndoRedoResetAndBatchApplication()
    {
        var vm = Create(); var editor = vm.Mutations;
        var originalCount = editor.Operations.Count;
        editor.AddCommand.Execute("2"); Assert.Equal(originalCount + 1, editor.Operations.Count);
        editor.UndoCommand.Execute(null); Assert.Equal(originalCount, editor.Operations.Count);
        editor.RedoCommand.Execute(null); Assert.Equal(originalCount + 1, editor.Operations.Count);
        editor.ApplyCommand.Execute(null);
        Assert.All(vm.Workspace.Samples, s => Assert.Equal(originalCount + 1, s.Operations.Count));
        editor.ResetCommand.Execute(null);
        Assert.Empty(editor.Operations); Assert.False(editor.CanUndo); Assert.False(editor.CanRedo);
        Assert.All(vm.Workspace.Samples.Where(s => s != vm.Workspace.SelectedSample), s => Assert.Equal(originalCount + 1, s.Operations.Count));
    }
    [Fact]
    public void ReadingComputesStatisticsAndCombinesSearchWithOutcomeFilter()
    {
        var vm = Create(); vm.Reading.RunCommand.Execute(null);
        Assert.Equal(120, vm.Reading.ResultsChecked); Assert.Equal(96, vm.Reading.ResultsSuccess); Assert.Equal(16, vm.Reading.ResultsFailed);
        vm.Reading.Filter = "Ошибки"; Assert.Equal(24, vm.Reading.Results.Count);
        vm.Reading.Search = "0042"; Assert.Equal("QR-0042", Assert.Single(vm.Reading.Results).Name);
        vm.Reading.Filter = "Успешные"; Assert.Empty(vm.Reading.Results);
    }
    [Fact]
    public void ComparisonReflectsResetAndKeepsSelectedSampleAcrossScreens()
    {
        var vm = Create(); var sample = vm.Workspace.SelectedSample;
        vm.Mutations.ResetCommand.Execute(null); vm.NavigateCommand.Execute("04");
        Assert.Same(sample, vm.Comparison.Workspace.SelectedSample);
        Assert.Equal(sample!.Modules, vm.Comparison.PreviewModules);
        Assert.All(vm.Comparison.DifferenceModules!.Cast<bool>(), cell => Assert.False(cell));
        vm.Comparison.Mode = "Наложение"; Assert.True(vm.Comparison.IsOverlay); Assert.False(vm.Comparison.IsSideBySide);
        vm.Comparison.OverlayOpacity = 25; Assert.Equal(.25, vm.Comparison.OverlayFraction);
    }
}
