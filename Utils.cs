namespace Lenovo_R45w30_Settings;

public class Utils
{
    /// <summary>
    /// Display a message at the bottom of the screen
    /// </summary>
    /// <param name="value">The message to display</param>
    public static void DisplayHint(string value)
    {
        Console.SetCursorPosition(0, 18);
        DecodeColors("~-7 " + value.PadRight(Console.WindowWidth) + "~--");
    }

    /// <summary>
    /// Clears the text from the screen
    /// </summary>
    /// <param name="full">By default, only the center menus are cleared, this option lets you clear the whole screen</param>
    public static void ClearScreen(bool full = false)
    {
        if (full)
        {
            Console.SetCursorPosition(0, 0);
            for (var i = 0; i < Console.WindowHeight; i++)
            {
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 0);
            return;
        }
        Console.SetCursorPosition(0, 4);
        for (var i = 0; i < 13; i++)
        {
            Console.Write(new string(' ', Console.WindowWidth));
        }
    }
    

    private static void HexStrToColor(string hex) // internal method, do not touch
    {
        var bg = hex[0];
        var fg = hex[1]; 
        if ((bg == '-') && (fg == '-'))
        {
            Console.ResetColor();
            return;
        }

        if (bg != '-')
        {
            var bgI = Convert.FromHexString("0" + bg)[0];
            Console.BackgroundColor = (ConsoleColor)bgI;
        }

        if (fg == '-') return;
        var fgI = Convert.FromHexString("0" + fg)[0];
        Console.ForegroundColor = (ConsoleColor)fgI;
    }

    /// <summary>
    /// Paints text characters inside the string to specific colors specified by my custom encoding and displays them.<br/><br/>
    ///
    /// Colors are encoded like so: ~[BG][FG], where BG represents a background color using a hex nibble (0-F)<br/>
    /// or dash character (-), FG is the same, but for foreground color. The first character of the encoded string MUST<br/>
    /// be a tilde (~).<br/><br/>
    ///
    /// Example: "~1FThis is white on blue.~-- This is default. ~-AThis is green on default.~4-This is default on red.~--"
    /// </summary>
    /// <param name="encoded">The encoded text</param>
    public static void DecodeColors(string encoded)
    {
        foreach (var _sect in encoded.Split('~'))
        {
            if (_sect.Length == 0) continue;
            var sect = _sect.Replace("::::", "~")[2..];
            var colorCode = _sect[..2].ToUpper(); 
            if (!Program.NoColor) HexStrToColor(colorCode);
            Console.Write(sect);
        }
    }
    
    
    /// <summary>
    /// For development/research purposes: Allows you to detect changes to VCP values.
    /// </summary>
    public static void Learn()
    {
        Console.Write("Please wait...");
        int[] blackList = [0x73, 0x74, 0x75, 0x78, 0xa4, 0xb4, 0xc3, 0xd2];
        var vcpDump = "";
        for (var i = 0x0; i < 0xFE; i++)
        {
            if (blackList.Contains(i)) continue;
            vcpDump += "0x" + i.ToString("X") + " - " +  new MenuEntry("VCP 0x" + i.ToString("X"), i).VerboseValue + "\n";
        } 
        var newVcpDump = "";
        Console.Write("Change the desired setting and press ANY key to continue . . . ");
        Console.ReadKey();
        Console.Write("\r".PadRight(Console.WindowWidth));
        Console.Write("\rLooking for changes...");
        for (var i = 0x0; i < 0xFE; i++)
        {
            if (blackList.Contains(i)) continue;
            newVcpDump += "0x" + i.ToString("X") + " - " + new MenuEntry("VCP 0x" + i.ToString("X"), i).VerboseValue + "\n";
        }
        Utils.ClearScreen(true);
        Console.WriteLine("The following differences were found:");
        for (var i = 0; i < vcpDump.Split('\n').Length; i++)
        {
            if (vcpDump.Split('\n')[i] == newVcpDump.Split('\n')[i]) continue;
            Console.WriteLine("BEFORE: " + vcpDump.Split('\n')[i]);
            Console.WriteLine("AFTER:  " + newVcpDump.Split('\n')[i]);
            Console.WriteLine();
        }
        Console.Write("Press any key to exit . . . ");
        Console.ReadKey();
        Utils.ClearScreen(true);
    }
}