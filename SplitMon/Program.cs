using System;
using System.Collections.Generic;
using System.IO;

namespace SplitMon;

record ActionHelp
(
    Action Act,
    string HelpString
);
class Program
{
    private static Dictionary<string, ActionHelp> d = new Dictionary<string, ActionHelp>
    {
        {"thunder", new (()=>DDCController.SetInputSource(0x1e), "Switch monitor to ThunderBolt Input") },
        {"hdmi", new (()=>DDCController.SetInputSource(0x11), "Switch monitor to HDMI input") },
        {"pbp", new(()=>DDCController.SetSplit(), "Switch PBP mode on") },
        {"usb", new(() => DDCController.SwitchUsb(), "Switch USB in PBP mode") },
        {"rmsplit", new(() => DDCController.RemoveSplit(), "Remove split (exit PBP mode)") },
        {"swap", new(() => DDCController.SwapSides(), "Swap left and right in PBP mode") },

    };
    private static void Main(string[] args)
    {
        try
        {
            if (args.Length > 0)
            {
                var command = args[0];
                if (!d.TryGetValue(command, out var actionHelp))
                {
                    Console.Error.WriteLine($"cannot find command '{command}'");
                    PrintCommands();
                    Environment.Exit(-1);
                }
                actionHelp.Act();
            }
            else
            {
                PrintCommands();
            }
            // MonitorVcpFeatures.Doit();
            //DDCController.SetSplit();
        }
        catch (Exception ex)
        {
            var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
            var progname = Path.GetFileNameWithoutExtension(fullname);
            Console.Error.WriteLine($"{progname} Error: {ex.Message}");
        }
    }

    private static void PrintCommands()
    {
        Console.WriteLine("Commands are:");
        foreach (var command in d)
        {
            Console.WriteLine($"   {command.Key,-10} {command.Value.HelpString}");
        }
    }
}

