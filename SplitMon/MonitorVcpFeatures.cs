namespace SplitMon;


using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

class MonitorVcpFeatures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    [DllImport("Dxva2.dll", SetLastError = true)]
    public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("Dxva2.dll", SetLastError = true)]
    public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("Dxva2.dll", SetLastError = true)]
    public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("Dxva2.dll", SetLastError = true)]
    public static extern bool GetCapabilitiesStringLength(IntPtr hMonitor, out uint pdwCapabilitiesStringLength);

    [DllImport("Dxva2.dll", SetLastError = true)]
    public static extern bool CapabilitiesRequestAndCapabilitiesReply(IntPtr hMonitor, StringBuilder pszASCIICapabilitiesString, uint dwCapabilitiesStringLengthInCharacters);

    [DllImport("Dxva2.dll", SetLastError = true)]
    public static extern bool SetVCPFeature(IntPtr hMonitor, byte bVCPCode, uint dwNewValue);

    [DllImport("user32.dll")]
    static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public static void Doit()
    {
        foreach (var screen in Screen.AllScreens)
        {
            IntPtr hMonitor = GetMonitorHandle(screen);
            if (hMonitor == IntPtr.Zero)
                continue;

            uint monitorCount = 0;
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref monitorCount))
                continue;

            var physicalMonitors = new PHYSICAL_MONITOR[monitorCount];
            if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, monitorCount, physicalMonitors))
                continue;

            foreach (var monitor in physicalMonitors)
            {
                Console.WriteLine("Monitor: " + monitor.szPhysicalMonitorDescription);

                if (GetCapabilitiesStringLength(monitor.hPhysicalMonitor, out uint len))
                {
                    StringBuilder sb = new StringBuilder((int)len);
                    if (CapabilitiesRequestAndCapabilitiesReply(monitor.hPhysicalMonitor, sb, len))
                    {
                        Console.WriteLine("Capabilities:");
                        Console.WriteLine(sb.ToString());
                    }
                }

                // Example: Try enabling PBP mode via VCP code 0xD6, value 0x03
                Console.WriteLine("Attempting to set PBP mode via VCP code 0xD6...");
                bool success = SetVCPFeature(monitor.hPhysicalMonitor, 0xD6, 0x03);
                Console.WriteLine(success ? "PBP mode set successfully (if supported)." : "Failed to set PBP mode.");

                DestroyPhysicalMonitors(monitorCount, physicalMonitors);
            }
        }
    }
    private static IntPtr GetMonitorHandle(Screen screen)
    {
        // Use a point near the top-left of the screen to get the HMONITOR
        var point = new POINT(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
        return MonitorFromPoint(point, 2); // MONITOR_DEFAULTTONEAREST
    }
}
