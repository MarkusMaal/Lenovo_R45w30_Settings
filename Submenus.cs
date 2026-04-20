using System.Text;

namespace Lenovo_R45w30_Settings;

public abstract class Submenus
{
    
    public static int RangeSelectUi(string label, Range valueRange, int currentValue)
    {
        var selValue = currentValue;
        while (true)
        {
            Utils.DisplayHint("\u2190/\u2192 - Set value   \u21B2 - Save changes   ESC - Discard changes");
            Console.SetCursorPosition(0, 4);
            var rangeVal = selValue;
            if (rangeVal > valueRange.End.Value) rangeVal = valueRange.End.Value;
            var rangeRatio = rangeVal / (float)(valueRange.End.Value);
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
            Utils.DecodeColors( "~-- * ~-B" + label + ": [" + bar + "] " + Math.Round(rangeRatio * 100.0) + "%  ~--\r");
            
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.LeftArrow:
                    selValue--;
                    break;
                case ConsoleKey.RightArrow:
                    selValue++;
                    break;
                case ConsoleKey.Enter:
                    return selValue;
                case ConsoleKey.Escape:
                    return currentValue;
            }
            
            if (selValue < valueRange.Start.Value) selValue = valueRange.Start.Value;
            else if (selValue > valueRange.End.Value) selValue = valueRange.End.Value;
        }
    }

    public static int ChoosePresetUi(Dictionary<string, int> presets, int currentValue, string settingName)
    {
        var selPreset = presets.TakeWhile(preset => preset.Value != currentValue).Count();
        Utils.DisplayHint("\u2191/\u2193 - Choose setting   \u21B2 - Save changes   ESC - Discard changes");
        Console.SetCursorPosition(0, 4);
        Console.WriteLine(settingName);
        while (true)
        {
            var i = 0;
            Console.SetCursorPosition(0, 6);
            foreach (var preset in presets)
            {
                Utils.DecodeColors("~--" + (selPreset == i ? " > ~-B" : "  ") + preset.Key + "~--  ");
                Console.WriteLine();
                i++;
            }

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.DownArrow:
                    selPreset++;
                    break;
                case ConsoleKey.UpArrow:
                    selPreset--;
                    break;
                case ConsoleKey.Enter:
                    return presets.Values.ToArray()[selPreset];
                case ConsoleKey.Escape:
                    return currentValue;
            }
            
            if (selPreset < 0) selPreset = presets.Count - 1;
            else if (selPreset >= presets.Count) selPreset = 0;
        }
    }

}