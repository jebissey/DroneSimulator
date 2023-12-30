using System.Diagnostics;
using System.IO;
using Usb.Events;



namespace ADroneSimulator.Models;

internal class GamePad
{
    private static IUsbEventWatcher usbEventWatcher = new UsbEventWatcher();
    public GamePad()
    {
        usbEventWatcher.UsbDeviceRemoved += UsbDeviceRemoved;
        usbEventWatcher.UsbDeviceAdded += UsbDeviceAdded;
        usbEventWatcher.UsbDriveEjected += UsbDriveEjected;
        usbEventWatcher.UsbDriveMounted += UsbDriveMounted;
    }

    #region Private methods
    private static void UsbDeviceRemoved(object? sender, UsbDevice device)
    {
        PrintIfNotEmpty("Removed:", $"{Environment.NewLine}{device}{Environment.NewLine}");
    }

    private static void UsbDeviceAdded(object? sender, UsbDevice device)
    {
        PrintIfNotEmpty("Added: ",$"{Environment.NewLine}{device}{Environment.NewLine}");
        Device? dev = Device.Get(device.DeviceSystemPath);
        if (dev == null) return;

        Debug.WriteLine("Friendly Name: " + dev.GetStringProperty(Device.DEVPKEY_Device_FriendlyName));
        Debug.WriteLine("");

        Debug.WriteLine("Parent: " + dev.ParentPnpDeviceId);

        Device? parent = Device.Get(dev.ParentPnpDeviceId);
        if (parent == null) return;

        PrintIfNotEmpty("Device Desc: ", parent.GetStringProperty(Device.DEVPKEY_Device_DeviceDesc));
        PrintIfNotEmpty("Bus Reported Device Desc: ", parent.GetStringProperty(Device.DEVPKEY_Device_BusReportedDeviceDesc));
        PrintIfNotEmpty("Friendly Name: ", parent.GetStringProperty(Device.DEVPKEY_Device_FriendlyName));
        Debug.WriteLine("");

        foreach (string pnpDeviceId in dev.ChildrenPnpDeviceIds)
        {
            PrintIfNotEmpty("Child: " , pnpDeviceId);

            Device? child = Device.Get(pnpDeviceId);

            if (child == null) continue;

            PrintIfNotEmpty("Device Desc: " , child.GetStringProperty(Device.DEVPKEY_Device_DeviceDesc));
            PrintIfNotEmpty("Bus Reported Device Desc: ", child.GetStringProperty(Device.DEVPKEY_Device_BusReportedDeviceDesc));
            PrintIfNotEmpty("Friendly Name: ", child.GetStringProperty(Device.DEVPKEY_Device_FriendlyName));
            Debug.WriteLine("");
        }
    }

    private static void UsbDriveEjected(object? sender, string path)
    {
        Debug.WriteLine("Ejected:" + Environment.NewLine + path + Environment.NewLine);
    }

    private static void UsbDriveMounted(object? sender, string path)
    {
        Debug.WriteLine("Mounted:" + Environment.NewLine + path + Environment.NewLine);

        foreach (string entry in Directory.GetFileSystemEntries(path))
            Debug.WriteLine(entry);

        Debug.WriteLine("");
    }

    private static void PrintIfNotEmpty(string tittle, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        Debug.WriteLine($"{tittle}{value}");
    }
    #endregion


   
}
