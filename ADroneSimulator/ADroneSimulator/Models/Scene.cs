using HelixToolkit.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ADroneSimulator.Models;

internal class Scene
{
    #region Fields
    private static readonly Brush boxColor = new SolidColorBrush(Colors.Red);
    private static readonly double xBoxPosition = 0;
    private static readonly double yBoxPosition = .0;
    private static readonly double zBoxPosition = .0;
    private static readonly double xBoxSize = 1;
    private static readonly double yBoxSize = 2;
    private static readonly double zBoxSize = 3;
    #endregion

    public Scene(ObservableCollection<Visual3D> Objects)
    {
        Objects.Clear();
        Objects.Add(new DefaultLights());
        Objects.Add(new GridLinesVisual3D() { MajorDistance = 10, MinorDistance = 1 });
        Objects.Add(new BoxVisual3D
        {
            Center = new Point3D(xBoxPosition, yBoxPosition, zBoxPosition + (zBoxSize / 2)),
            Height = zBoxSize,
            Length = xBoxSize,
            Width = zBoxSize,
            Fill = boxColor,
        });
    }
}
