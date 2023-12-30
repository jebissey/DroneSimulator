using Djlastnight.Hid;
using Djlastnight.Input;
using System.Windows;
using System.Windows.Documents;

namespace ADroneSimulator.Models;

internal class GamePad
{
    #region Fields
    private readonly List<IIinputDevice> devices = [];
    private IIinputDevice device = null!;
    private HidDataReader reader = null!;

    private int pid = 0;
    private int vid = 0;

    #endregion
    public GamePad()
    {
        ResetDevices();
    }


    #region public methods
    public void ResetDevices()
    {
        devices.Clear();
        devices.AddRange(DeviceScanner.GetUsbGamepads());
        if (devices.Count > 0) device = devices[0];
    }

    public void StartListening(Window window)
    {
        if (device == null)
        {
            MessageBox.Show("Please choose device first", "No device selected", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

      
        try
        {
            pid = Convert.ToInt32(device.Product, 16);
            vid = Convert.ToInt32(device.Vendor, 16);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error, while parsing device.Product or device.Vendor: " + ex.Message);
            return;
        }

        if (pid < 0 || vid < 0)
        {
            MessageBox.Show("Error! Negative pid or vid are not accepted!");
            return;
        }

        reader = new HidDataReader(window);
        reader.HidDataReceived += OnHidDataReceived;
    }

    #endregion


    private void OnHidDataReceived(object sender, HidEvent e)
    {
        try
        {
            if (device == null) return;
            if (e.Device == null)
            {
                if (e.IsKeyboard)
                {
                    // On-Screen Keyboard
                    var log = string.Format(
                        "{0} {1} | Make code: {2} | VKey: {3}",
                        e.VirtualKey,
                        e.IsButtonDown ? "[pressed]" : "[released]",
                        e.RawInput.keyboard.MakeCode,
                        e.RawInput.keyboard.VKey);

                }
                return;
            }

            if (device is UsbGamepad && e.InputReport == null)
            {
                return;
            }

            var senderVid = e.Device.VendorId.ToString("X4");
            var senderPid = e.Device.ProductId.ToString("X4");
            if (device.Vendor != senderVid || device.Product != senderPid)
            {
                return;
            }

            string[] senderTokens = e.Device.Name.Split(['#']);
            if (senderTokens.Length != 4)
            {
                return;
            }

            var senderDeviceID = senderTokens[1].ToLower();
            var currentTokens = device.DeviceID.Split(new char[] { '\\' });
            var currDeviceID = currentTokens[1].ToLower();

            if (senderDeviceID != currDeviceID)
            {
                return;
            }

            if (e.InputReport != null)
            {
                //if (this.dataCheckBoxes == null || this.dataCheckBoxes.Count != e.InputReport.Length)
                //{
                //    this.dataCheckBoxes = new List<CheckBox>();
                //    this.lastData = new byte[e.InputReport.Length];
                //    this.layersWrapPanel.Children.Clear();

                //    for (int i = 0; i < e.InputReport.Length; i++)
                //    {
                //        var checkbox = new CheckBox()
                //        {
                //            Content = i,
                //            Margin = new Thickness(5),
                //            IsChecked = true,
                //            ToolTip = "Subscribe for byte " + i + " changes"
                //        };

                //        this.dataCheckBoxes.Add(checkbox);
                //        this.layersWrapPanel.Children.Add(checkbox);
                //    }
                //}
            }

            //var currentText = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
            //if (this.GetLines(currentText).Length > 100)
            //{
            //    this.rtb.Document.Blocks.Clear();
            //    this.rtb.Document.Blocks.Add(new Paragraph(new Run("--- AutoCleared (exeed 100 lines) ---")));
            //}

            var paragraph = new Paragraph();

            if (device is UsbGamepad)
            {
                //paragraph = this.CreateColorParagraphFromData(e.InputReport, this.lastData);
                //this.lastData = e.InputReport;
            }
            else if (device is HidKeyboard)
            {
                paragraph.Inlines.Add(new Run(string.Format("{0} {1} | Make code: {2} | VKey: {3}", e.VirtualKey, e.IsButtonDown ? "[pressed]" : "[released]", e.RawInput.keyboard.MakeCode, e.RawInput.keyboard.VKey)));
            }
            else if (device is HidMouse)
            {
                // Getting the mouse buttons flags
                var mouseButtonFlags = e.RawInput.mouse.buttonsStr.usButtonFlags;
                if (mouseButtonFlags != 0)
                {
                    paragraph.Inlines.Add(new Run(mouseButtonFlags.ToString()));
                }
                else
                {
                    // Getting the mouse delta move coordinates
                    paragraph.Inlines.Add(new Run(string.Format("Mouse delta move X:{0} Y:{1}", e.RawInput.mouse.lLastX, e.RawInput.mouse.lLastY)));
                }
            }
            else
            {
                // Other device
                //if (e.InputReport != null)
                //{
                //    paragraph = this.CreateColorParagraphFromData(e.InputReport, this.lastData);
                //    this.lastData = e.InputReport;
                //}
                //else
                //{
                //    // Displaying the unknown data
                //    paragraph.Inlines.Add(new Run(string.Format("Unknown raw data received:{0}{1}", Environment.NewLine, e.RawInput)));
                //    paragraph.Foreground = Brushes.Red;
                //    this.rtb.Document.Blocks.Add(paragraph);
                //    return;
                //}
            }

            if (paragraph.Inlines.Count == 0)
            {
                return;
            }

            //var lastLine = this.GetLines(currentText).LastOrDefault();
            //var newLine = string.Join(string.Empty, paragraph.Inlines.Select(line => line.ContentStart.GetTextInRun(LogicalDirection.Forward)));
            //if (lastLine != newLine)
            //{
            //    this.rtb.Document.Blocks.Add(paragraph);
            //}
        }
        catch (Exception ex)
        {
            //this.rtb.Document.Blocks.Add(new Paragraph(new Run("Error: " + ex.Message) { Foreground = Brushes.Red, Background = Brushes.Black }));
        }
    }

}
