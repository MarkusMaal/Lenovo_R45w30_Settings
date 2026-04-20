// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using System.Reflection.Emit;
using System.Text;

namespace Lenovo_R45w30_Settings;

public abstract class Program
{
    public static readonly string DdcExe = "ddcutil";

    public static bool NoColor = false;
    
    public static void Main(string[] args)
    {
        if (args.Contains("--no-color")) NoColor = true;
        Utils.DisplayHint("Please wait...");
        var selTab = 0;
        var selEntry = 0;
        while (true)
        {
            Console.SetCursorPosition(0, 3);
            RenderMenu(selTab);
            Utils.DisplayHint("\u2190/\u2192 - Switch category   \u2191/\u2193 - Change selection   \u21B2 - Edit value   ESC - Exit");
            Console.SetCursorPosition(0, 4);
            foreach (var (idx, mi) in Constants.MenuEntries[selTab].Index())
            {
                Utils.DecodeColors((idx == selEntry ? "~-- > ~-B" : "~--  ") + mi.GetString(selEntry == idx) + "~--" + (idx == selEntry ? "  " : "   "));
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
                case ConsoleKey.Escape:
                    Utils.ClearScreen(true);
                    return;
                case ConsoleKey.Enter:
                    Utils.ClearScreen();
                    var selMenuEntry = Constants.MenuEntries[selTab][selEntry];
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
                    }
                    Utils.ClearScreen();
                    break;
            }
            if (selTab < 0) selTab = Constants.Tabs.Length - 1;
            else if (selTab >= Constants.Tabs.Length) selTab = 0;
            if (selEntry < 0) selEntry = Constants.MenuEntries[selTab].Length - 1;
            else if (selEntry >= Constants.MenuEntries[selTab].Length) selEntry = 0;
        }
    }

    private static void RenderMenu(int selectedTab)
    {
        Console.SetCursorPosition(0, 1);
        Console.WriteLine();
        Utils.DecodeColors((selectedTab == 0 ? "~BB" : "~88") + " " + (selectedTab == 0 ? "~BB" : "") + "  ");
        foreach (var (index, tab) in Constants.Tabs.Index())
        {
            var eTab = tab;
            if (selectedTab == index)
            {
                eTab = "< " + tab + " >";
            }
            while (eTab.Length < 15)
            {
                eTab = " " + eTab + " ";
            }

            if (index == 0) eTab = eTab[3..];
            Utils.DecodeColors((index == selectedTab ? "~B0" : "~87") + eTab);
        }
        Utils.DecodeColors("~--");
        Console.WriteLine();
        Console.WriteLine();
    }
}