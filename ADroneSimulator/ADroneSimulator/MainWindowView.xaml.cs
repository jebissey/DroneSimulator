using System.Windows.Interop;

namespace ADroneSimulator;

public partial class MainWindowView : System.Windows.Window
{
    public MainWindowView()
    {
        new WindowInteropHelper(this).EnsureHandle();        // Ensure the handle is created


        InitializeComponent();
    }
}