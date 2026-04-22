namespace Lenovo_R45w30_Settings;

public abstract class Program
{
    public const string DdcExe = "ddcutil";

    public static bool NoColor;
    
    public static void Main(string[] args)
    {
        if (args.Contains("--no-color")) NoColor = true;
        Utils.ClearScreen(true);
        Utils.DisplayHint("Please wait...");
        var selTab = 0;
        var selEntry = 0;
        foreach (var _ in Constants.MenuEntries)
        {
            // dummy access to force menu entries to initialize before rendering the tabs
        }
        while (true)
        {
            Console.SetCursorPosition(0, 3);
            RenderTabs(selTab);
            Utils.DisplayHint("\u2190/\u2192 - Switch category   \u2191/\u2193 - Change selection   \u21B2 - Edit value   ESC - Exit");
            Console.SetCursorPosition(0, 4);
            foreach (var (idx, mi) in Constants.MenuEntries.Values.ToArray()[selTab].Index())
            {
                Utils.DecodeColors((idx == selEntry ? "~-- > ~-B" : "~--  ") + mi.GetString() + "~--" + (idx == selEntry ? "  " : "   "));
                Console.WriteLine();
            }
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow:
                    selEntry--;
                    break;
                case ConsoleKey.DownArrow:
                    selEntry++;
                    break;
                case ConsoleKey.LeftArrow:
                    selTab--;
                    selEntry = 0;
                    Utils.ClearScreen();
                    break;
                case ConsoleKey.RightArrow:
                    selTab++;
                    selEntry = 0;
                    Utils.ClearScreen();
                    break;
                case ConsoleKey.L:
                    Utils.ClearScreen(true);
                    Utils.Learn();
                    Utils.ClearScreen();
                    break;
                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                    Utils.ClearScreen(true);
                    return;
                case ConsoleKey.Enter:
                    Utils.ClearScreen();
                    var selMenuEntry = Constants.MenuEntries.Values.ToArray()[selTab][selEntry];
                    switch (selMenuEntry.Type)
                    {
                        case MenuEntry.MenuType.PresetList:
                            var newVal = Submenus.ChoosePresetUi(selMenuEntry.GetPresets(), selMenuEntry.Value, selMenuEntry.Label);
                            selMenuEntry.WriteValue(newVal);
                            break;
                        case MenuEntry.MenuType.Range:
                            var newRangeVal = Submenus.RangeSelectUi(selMenuEntry.Label, selMenuEntry.Range1,
                                selMenuEntry.Value);
                            if (newRangeVal == selMenuEntry.Range1.End.Value)
                            {
                                newRangeVal = 255;
                            }
                            selMenuEntry.WriteValue(newRangeVal);
                            break;
                        case MenuEntry.MenuType.Action:
                            selMenuEntry.WriteValue(selMenuEntry.TriggerValue);
                            break;
                        case MenuEntry.MenuType.Toggle:
                            selMenuEntry.WriteValue(selMenuEntry.Value == selMenuEntry.TriggerValue ? 0 : selMenuEntry.TriggerValue);
                            break;
                        case MenuEntry.MenuType.MultiSource:
                            var newSrcVal = Submenus.MultiSourceSelectUi(selMenuEntry.GetPresets(),
                                selMenuEntry.SourceLabels ?? []);
                            if (newSrcVal != -1)
                            {
                                selMenuEntry.WriteValue(newSrcVal);
                            }
                            break;
                    }
                    Utils.ClearScreen();
                    break;
            }
            if (selTab < 0) selTab = Constants.MenuEntries.Count - 1;
            else if (selTab >= Constants.MenuEntries.Count) selTab = 0;
            if (selEntry < 0) selEntry = Constants.MenuEntries.Values.ToArray()[selTab].Length - 1;
            else if (selEntry >= Constants.MenuEntries.Values.ToArray()[selTab].Length) selEntry = 0;
        }
    }

    /// <summary>
    /// Displays the top tabs to the user
    /// </summary>
    /// <param name="selectedTab">The tab that should be highlighted</param>
    private static void RenderTabs(int selectedTab)
    {
        Console.SetCursorPosition(0, 1);
        Console.WriteLine();
        foreach (var (index, tab) in Constants.MenuEntries.Keys.Index())
        {
            var eTab = tab;
            if (selectedTab == index)
            {
                eTab = "< " + tab + " >";
            }
            while (eTab.Length < 12)
            {
                eTab = " " + eTab + " ";
            }

            Utils.DecodeColors((index == selectedTab ? "~B0" : "~87") + eTab);
        }
        Utils.DecodeColors("~--");
        Console.WriteLine();
        Console.WriteLine();
    }
}