using System.Windows.Input;
using QRLab.Core.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
namespace QRLab.View.Controls;
/// <summary>Renders supplied preview modules. It does not generate or decode codes.</summary>
public sealed class CodePreview : Control
{
    public static readonly StyledProperty<bool[,]?> ModulesProperty = AvaloniaProperty.Register<CodePreview, bool[,]?>(nameof(Modules));
    public static readonly StyledProperty<double> ZoomProperty = AvaloniaProperty.Register<CodePreview, double>(nameof(Zoom), 100);
    public static readonly StyledProperty<bool> DifferenceProperty = AvaloniaProperty.Register<CodePreview, bool>(nameof(Difference));
    public static readonly StyledProperty<double> LayerOpacityProperty = AvaloniaProperty.Register<CodePreview, double>(nameof(LayerOpacity), 1);
    public bool[,]? Modules { get => GetValue(ModulesProperty); set => SetValue(ModulesProperty, value); }
    public double Zoom { get => GetValue(ZoomProperty); set => SetValue(ZoomProperty, value); }
    public bool Difference { get => GetValue(DifferenceProperty); set => SetValue(DifferenceProperty, value); }
    public double LayerOpacity { get => GetValue(LayerOpacityProperty); set => SetValue(LayerOpacityProperty, value); }
    public static readonly StyledProperty<ICommand?> PaintCommandProperty = AvaloniaProperty.Register<CodePreview, ICommand?>(nameof(PaintCommand));
    public static readonly StyledProperty<bool> PanEnabledProperty = AvaloniaProperty.Register<CodePreview, bool>(nameof(PanEnabled));
    public ICommand? PaintCommand { get => GetValue(PaintCommandProperty); set => SetValue(PaintCommandProperty, value); }
    public bool PanEnabled { get => GetValue(PanEnabledProperty); set => SetValue(PanEnabledProperty, value); }
    private Point? _dragStart;
    private Vector _pan;
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        if (PanEnabled) { _dragStart = e.GetPosition(this); e.Pointer.Capture(this); return; }
        if (Modules is not { } modules || PaintCommand is not { } command) return;
        var unit = Unit(modules);
        var p = e.GetPosition(this);
        var x = (p.X - (Bounds.Width - modules.GetLength(1) * unit) / 2 - _pan.X) / (modules.GetLength(1) * unit);
        var y = (p.Y - (Bounds.Height - modules.GetLength(0) * unit) / 2 - _pan.Y) / (modules.GetLength(0) * unit);
        var point = new PreviewPoint(x, y);
        if (x is >= 0 and <= 1 && y is >= 0 and <= 1 && command.CanExecute(point)) command.Execute(point);
        e.Handled = true;
    }
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_dragStart is not { } last) return;
        var position = e.GetPosition(this); _pan += position - last; _dragStart = position; InvalidateVisual();
    }
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e); _dragStart = null; e.Pointer.Capture(null);
    }
    private double Unit(bool[,] modules) => Math.Min(Bounds.Width / (modules.GetLength(1) + 10), Bounds.Height / (modules.GetLength(0) + 8)) * Zoom / 100 * _wheelZoom;
    public static readonly StyledProperty<bool> AllowZoomProperty = AvaloniaProperty.Register<CodePreview, bool>(nameof(AllowZoom));
    public bool AllowZoom { get => GetValue(AllowZoomProperty); set => SetValue(AllowZoomProperty, value); }
    private double _wheelZoom = 1;
    static CodePreview() => AffectsRender<CodePreview>(ModulesProperty, ZoomProperty, DifferenceProperty, LayerOpacityProperty);
    public CodePreview() { ClipToBounds = true; }
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        if (!AllowZoom) { base.OnPointerWheelChanged(e); return; }
        _wheelZoom = Math.Clamp(_wheelZoom + e.Delta.Y * .1, .5, 3); InvalidateVisual(); e.Handled = true;
    }
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var rect = new Rect(Bounds.Size); context.FillRectangle(Brushes.White, rect);
        if (Modules is not { } modules) return;
        var cols = modules.GetLength(1); var rows = modules.GetLength(0);
        var unit = Unit(modules);
        var left = (Bounds.Width - cols * unit) / 2 + _pan.X; var top = (Bounds.Height - rows * unit) / 2 + _pan.Y;
        var ink = new SolidColorBrush(Color.Parse(Difference ? "#DB716F" : "#111918"));
        using (context.PushOpacity(LayerOpacity))
        {
            for (var y = 0; y < rows; y++)
            for (var x = 0; x < cols; x++)
            {
                if (!modules[y, x]) continue;
                context.FillRectangle(ink, new Rect(left + x * unit, top + y * unit, unit + .1, unit + .1));
            }

        }
    }
}
