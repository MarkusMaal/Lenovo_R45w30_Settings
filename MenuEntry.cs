using System.Diagnostics;
using System.Text;

namespace Lenovo_R45w30_Settings;

public class MenuEntry
{
    public string Label { get; set; }
    private string Vcp { get; set; }
    public int Value { get; set; }
    public MenuType Type { get; set; }
    
    public Range Range1 { get; set; }
    
    public Range Range2 { get; set; }
    
    private Dictionary<string, int>? PresetList { get; set; }
    
    public int TriggerValue { get; set; }
    
    public string[]? SourceLabels { get; set; }

    public string? VerboseValue { get; set; }
    
    /// <summary>
    /// Constructor for toggles/actions
    /// </summary>
    /// <param name="label">User-friendly name for the setting</param>
    /// <param name="vcp">VCP in format xNN where NN is a hexadecimal value from 00-FF</param>
    /// <param name="checkable">Enables toggle mode. In this mode the user can check/uncheck a setting (unchecked = 0, checked = toggleValue)</param>
    /// <param name="toggleValue">The value to write. In toggle mode, this gets written when the setting is checked.</param>
    public MenuEntry(string label, string vcp, bool checkable, int toggleValue)
    {
        Type = checkable ? MenuType.Toggle : MenuType.Action;
        Label = label;
        Vcp = vcp;
        TriggerValue = toggleValue;
        GetCurrentValue();
    }

    /// <summary>
    /// Constructor for preset list
    /// </summary>
    /// <param name="label">User-friendly name for the setting</param>
    /// <param name="vcp">VCP in format xNN where NN is a hexadecimal value from 00-FF</param>
    /// <param name="presets">List of setting values, where key is a label for the user and value is the actual value to write to VCP</param>
    public MenuEntry(string label, string vcp, Dictionary<string, int> presets)
    {
        Type = MenuType.PresetList;
        Label = label;
        Vcp = vcp;
        PresetList = presets;
        GetCurrentValue();
    }

    /// <summary>
    /// Constructor for coordinate (currently unused)
    /// </summary>
    /// <param name="label">User-friendly name for the setting</param>
    /// <param name="vcp">VCP in format xNN where NN is a hexadecimal value from 00-FF</param>
    /// <param name="ranges">Two range values corresponding to the bounds of a 2D coordinate</param>
    public MenuEntry(string label, string vcp, Range[] ranges)
    {
        Type = MenuType.Coordinate;
        Label = label;
        Vcp = vcp;
        Range1 = ranges[0];
        Range2 = ranges[1];
        GetCurrentValue();
    }

    /// <summary>
    /// Constructor for a range
    /// </summary>
    /// <param name="label">User-friendly name for the setting</param>
    /// <param name="vcp">VCP in format xNN where NN is a hexadecimal value from 00-FF</param>
    /// <param name="range">Minimum and maximum value for the setting</param>
    public MenuEntry(string label, string vcp, Range range)
    {
        Type = MenuType.Range;
        Label = label;
        Vcp = vcp;
        Range1 = range;
        GetCurrentValue();
    }

    /// <summary>
    /// Constructor for testing
    /// </summary>
    /// <param name="label">User-friendly name for the setting</param>
    /// <param name="vcp">VCP in format xNN where NN is a hexadecimal value from 00-FF</param>
    public MenuEntry(string label, int vcp)
    {
        Type = MenuType.Range;
        Vcp = "x" + vcp.ToString("X");
        Label = label;
        GetCurrentValue();
    }

    /// <summary>
    /// Constructor for multi-source
    /// </summary>
    /// <param name="label">User-friendly name for the setting</param>
    /// <param name="vcp">VCP in format xNN where NN is a hexadecimal value from 00-FF</param>
    /// <param name="presets">List of setting values, where key is a label for the user and value is the actual value to write to VCP</param>
    /// <param name="sourceLabels">Labels for the two settings that will be written</param>
    public MenuEntry(string label, string vcp, Dictionary<string, int> presets, string[] sourceLabels)
    {
        Type = MenuType.MultiSource;
        Vcp = vcp;
        Label = label;
        PresetList =  presets;
        SourceLabels = sourceLabels;
    }

    /// <summary>
    /// Read current setting value
    /// </summary>
    private void GetCurrentValue()
    {
        if (Type == MenuType.Action) return;
        Utils.DisplayHint("\u23F3 Querying " + Label.TrimStart());
        Console.SetCursorPosition(3, 2);
        var p = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Program.DdcExe,
                Arguments = "-t getvcp " + Vcp,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            }
        };
        p.Start();
        var output = "";
        while (!p.StandardOutput.EndOfStream)
        {
            output += p.StandardOutput.ReadLine();
        }

        if (output.Contains("not readable"))
        {
            Value = -1;
            return;
        }

        VerboseValue = output;
        var values =  output.Split(' ');
        Value = values[2] switch
        {
            "C" => (int)Convert.ToUInt32(values[3]),
            "SNC" => (int)Convert.ToUInt32(values[values.IndexOf("SNC")+1][1..], 16),
            "CNC" => (int)Convert.ToUInt32((values[^2] + values[^1]).Replace("x", ""), 16),
            "ERR" => -1,
            _ => Value
        };
    }

    /// <summary>
    /// Write a new value
    /// </summary>
    /// <param name="newValue">The value to write to VCP</param>
    public void WriteValue(int newValue)
    {
        var p = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Program.DdcExe,
                Arguments = "-t setvcp " + Vcp + " " + newValue,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        };
        p.Start();
        Value = newValue;
    }

    public Dictionary<string, int> GetPresets()
    {
        return PresetList ?? [];
    }

    /// <summary>
    /// Gets the menu entry as a string
    /// </summary>
    /// <returns>Menu entry to be displayed to the user</returns>
    public string GetString()
    {
        switch (Type)
        {
            case MenuType.PresetList:
                var selLabel = "???";
                foreach (var p in PresetList!.Where(p => p.Value == Value))
                {
                    selLabel = p.Key;
                }
                return Label + ": [" + selLabel + "]";
            case MenuType.Range:
                var rangeVal = Value;
                if (Value > Range1.End.Value)
                {
                    rangeVal = Range1.End.Value;
                }
                var rangeRatio = rangeVal / (float)(Range1.End.Value);
                var bar = new StringBuilder();
                int i;
                for (i = 0; i < (rangeRatio * 10.0); i++)
                {
                    bar.Append('\u2588');
                }

                for (; i < 10; i++)
                {
                    bar.Append(' ');
                }
                return Label + ": [" + bar + "] " + Math.Round(rangeRatio * 100.0) + "%";
            case MenuType.Action:
            case MenuType.Coordinate:
            case MenuType.MultiSource:
                return Label;
            case MenuType.Toggle:
                return "[" + (Value > 0 ? "X" : " ") + "] " + Label;
            default:
                return this.ToString() ?? "";
        }
    }

    /// <summary>
    /// Determines how the setting should be displayed to the user
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// List of preset values
        /// </summary>
        PresetList,
        /// <summary>
        /// A setting that can be turned on/off
        /// </summary>
        Toggle,
        /// <summary>
        /// A setting that has a range of values we can set
        /// </summary>
        Range,
        /// <summary>
        /// A setting that has two ranges of values we can set
        /// </summary>
        Coordinate,
        /// <summary>
        /// A button that performs a specific action
        /// </summary>
        Action,
        /// <summary>
        /// A setting that has multiple lists of preset values, which are combined into one value byte-by-byte
        /// </summary>
        MultiSource
    }
}