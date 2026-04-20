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
    
    public MenuEntry(string label, string vcp, bool checkable, int toggleValue)
    {
        Type = checkable ? MenuType.Toggle : MenuType.Action;
        Label = label;
        Vcp = vcp;
        TriggerValue = toggleValue;
        GetCurrentValue();
    }

    public MenuEntry(string label, string vcp, Dictionary<string, int> presets)
    {
        Type = MenuType.PresetList;
        Label = label;
        Vcp = vcp;
        PresetList = presets;
        GetCurrentValue();
    }

    public MenuEntry(string label, string vcp, Range[] ranges)
    {
        Type = MenuType.Coordinate;
        Label = label;
        Vcp = vcp;
        Range1 = ranges[0];
        Range2 = ranges[1];
        GetCurrentValue();
    }

    public MenuEntry(string label, string vcp, Range range)
    {
        Type = MenuType.Range;
        Label = label;
        Vcp = vcp;
        Range1 = range;
        GetCurrentValue();
    }

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
                RedirectStandardError = true,
            }
        };
        p.Start();
        var output = "";
        while (!p.StandardOutput.EndOfStream)
        {
            output += p.StandardOutput.ReadLine();
        }
        var values =  output.Split(' ');

        Value = values[2] switch
        {
            "C" => (int)Convert.ToUInt32(values[3]),
            "CNC" or "SNC" => (int)Convert.ToUInt32(output.Split('x')[^1], 16),
            _ => Value
        };
    }

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

    public string GetString(bool highlight)
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
                return Label;
            case MenuType.Toggle:
                return "[" + (TriggerValue == Value ? "X" : " ") + "] " + Label;
            default:
                return this.ToString() ?? "";
        }
    }

    public enum MenuType
    {
        PresetList,
        Toggle,
        Range,
        Coordinate,
        Action
    }
}