namespace SplitMon;


using System;
using System.Linq;
using System.Runtime.InteropServices;


class DDCController
{
    [DllImport("dxva2.dll", SetLastError = true)]
    static extern bool SetVCPFeature(IntPtr hMonitor, byte bVCPCode, uint dwNewValue);

    [DllImport("dxva2.dll", SetLastError = true)]
    static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll", SetLastError = true)]
    static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    public static void SetSplit()
    {
        IntPtr hWnd = GetForegroundWindow();
        IntPtr hMonitor = MonitorFromWindow(hWnd, 2); // MONITOR_DEFAULTTONEAREST

        uint numMonitors = 0;
        GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref numMonitors);

        var monitors = new PHYSICAL_MONITOR[numMonitors];
        GetPhysicalMonitorsFromHMONITOR(hMonitor, numMonitors, monitors);

        // E9(00 01 02 21 22 24 27 28 29 2A )
        // switch USB: E7 0xFF00
        foreach (var mon in monitors.Where(x => x.szPhysicalMonitorDescription.IndexOf("dell", StringComparison.OrdinalIgnoreCase) >= 0))
        {
            SetVCPFeature(mon.hPhysicalMonitor, 0xE9, 0x24);
        }
    }
    public static void RemoveSplit()
    {
        IntPtr hWnd = GetForegroundWindow();
        IntPtr hMonitor = MonitorFromWindow(hWnd, 2); // MONITOR_DEFAULTTONEAREST

        uint numMonitors = 0;
        GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref numMonitors);

        var monitors = new PHYSICAL_MONITOR[numMonitors];
        GetPhysicalMonitorsFromHMONITOR(hMonitor, numMonitors, monitors);

        // E9(00 01 02 21 22 24 27 28 29 2A )
        // switch USB: E7 0xFF00
        foreach (var mon in monitors.Where(x => x.szPhysicalMonitorDescription.IndexOf("dell", StringComparison.OrdinalIgnoreCase) >= 0))
        {
            SetVCPFeature(mon.hPhysicalMonitor, 0xE9, 0x00);
        }
    }
    public static void SwitchUsb()
    {
        IntPtr hWnd = GetForegroundWindow();
        IntPtr hMonitor = MonitorFromWindow(hWnd, 2); // MONITOR_DEFAULTTONEAREST

        uint numMonitors = 0;
        GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref numMonitors);

        var monitors = new PHYSICAL_MONITOR[numMonitors];
        GetPhysicalMonitorsFromHMONITOR(hMonitor, numMonitors, monitors);

        // E9(00 01 02 21 22 24 27 28 29 2A )
        // switch USB: E7 0xFF00
        foreach (var mon in monitors.Where(x => x.szPhysicalMonitorDescription.IndexOf("dell", StringComparison.OrdinalIgnoreCase) >= 0))
        {
            SetVCPFeature(mon.hPhysicalMonitor, 0xE7, 0xFF00);
        }
    }

    public static void SetInputSource(uint inputSource)
    {
        IntPtr hWnd = GetForegroundWindow();
        IntPtr hMonitor = MonitorFromWindow(hWnd, 2); // MONITOR_DEFAULTTONEAREST

        uint numMonitors = 0;
        GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref numMonitors);

        var monitors = new PHYSICAL_MONITOR[numMonitors];
        GetPhysicalMonitorsFromHMONITOR(hMonitor, numMonitors, monitors);

        foreach (var mon in monitors)
        {
            // 0x60 is the VCP code for Input Source
            SetVCPFeature(mon.hPhysicalMonitor, 0x60, inputSource);
        }

        Console.WriteLine("Input source set to: " + inputSource);
    }

    internal static void SwapSides()
    {
        IntPtr hWnd = GetForegroundWindow();
        IntPtr hMonitor = MonitorFromWindow(hWnd, 2); // MONITOR_DEFAULTTONEAREST

        uint numMonitors = 0;
        GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref numMonitors);

        var monitors = new PHYSICAL_MONITOR[numMonitors];
        GetPhysicalMonitorsFromHMONITOR(hMonitor, numMonitors, monitors);

        // E9(00 01 02 21 22 24 27 28 29 2A )
        // switch sides E5 0xF001
        foreach (var mon in monitors.Where(x => x.szPhysicalMonitorDescription.IndexOf("dell", StringComparison.OrdinalIgnoreCase) >= 0))
        {
            SetVCPFeature(mon.hPhysicalMonitor, 0xE5, 0xF001);
        }
    }
}
