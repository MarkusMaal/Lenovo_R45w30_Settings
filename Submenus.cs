using System.Text;

namespace Lenovo_R45w30_Settings;

public abstract class Submenus
{
    
    /// <summary>
    /// Show a menu for selecting a range (e.g. brightness control)
    /// </summary>
    /// <param name="label">Text to display next to the slider</param>
    /// <param name="valueRange">Min/max values the user can choose from</param>
    /// <param name="currentValue">Current value from VCP</param>
    /// <returns>New value to write to VCP</returns>
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

    /// <summary>
    /// Displays a menu where the user can select from a list of options
    /// </summary>
    /// <param name="presets">The list of menu items and values assigned to them</param>
    /// <param name="currentValue">Current value from VCP</param>
    /// <param name="settingName">Text to display at the top</param>
    /// <returns>New value to write to VCP</returns>
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

    /// <summary>
    /// Displays a menu that has two screens to choose values from
    /// </summary>
    /// <param name="presets">Menu items and values assigned to them</param>
    /// <param name="sourceLabels">Text to display for each screen</param>
    /// <returns>Chosen values combined as short, to be written to VCP</returns>
    public static int MultiSourceSelectUi(Dictionary<string, int> presets, string[] sourceLabels)
    {
        var results = new List<int>();
        foreach (var sl in sourceLabels)
        {
            Utils.ClearScreen();
            Utils.DisplayHint("\u2191/\u2193 - Choose setting   \u21B2 - Save changes   ESC - Discard changes");
            Console.SetCursorPosition(0, 4);
            Console.WriteLine(sl);
            var selPreset = 0;
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

                var breakLoop = false;
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.DownArrow:
                        selPreset++;
                        break;
                    case ConsoleKey.UpArrow:
                        selPreset--;
                        break;
                    case ConsoleKey.Enter:
                        results.Add(presets.Values.ToArray()[selPreset]);
                        breakLoop = true;
                        break;
                    case ConsoleKey.Escape:
                        Utils.ClearScreen(true);
                        return -1;
                }

                if (breakLoop) break;
                if (selPreset < 0) selPreset = presets.Count - 1;
                else if (selPreset >= presets.Count) selPreset = 0;
            }
        }
        return Convert.ToInt32(results[1].ToString("X").PadLeft(2, '0') + results[0].ToString("X").PadLeft(2, '0'), 16);
    }

}