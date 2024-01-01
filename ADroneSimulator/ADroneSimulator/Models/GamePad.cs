using HidSharp;
using HidSharp.Utility;

namespace ADroneSimulator.Models;

internal class GamePad
{
    #region Fields
    private readonly List<HidDevice> devices = [];
    HidDevice device = null!;
    HidStream stream = null!;
    private readonly DeviceList list = DeviceList.Local;
    #endregion

    public GamePad()
    {
        HidSharpDiagnostics.EnableTracing = true;
        HidSharpDiagnostics.PerformStrictChecks = true;

        list.Changed += (sender, e) => ResetDevices();
        ResetDevices();
    }


    #region public methods
    public void ResetDevices()
    {
        devices.Clear();
        devices.AddRange(list.GetHidDevices());
        foreach (HidDevice hid in devices)
        {
            try
            {
                if (hid.GetManufacturer() == "FrSky")
                {
                    device = hid;
                    if (!device.TryOpen(out stream)) throw new Exception("device opening failed ");
                    break;
                }
            }
            catch { }
        }
    }

    public void StartListening()
    {
        if (stream == null) return;


        var toto = stream.Read();
    }

    #endregion




}
