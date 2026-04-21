namespace Lenovo_R45w30_Settings;

public class Constants
{
    /// <summary>
    /// Tabs to display at the top
    /// </summary>
    public static string[] Tabs = ["Game", "Screen", "Color", "Port", "Menu", "Special"];

    /// <summary>
    /// The index of each menu entry array corresponds to the same index as Tabs array
    /// </summary>
    public static List<MenuEntry[]> MenuEntries = [
        [
            new MenuEntry("Game Mode", "xf9", new Dictionary<string, int>
            {
                {"Standard", 0},
                {"FPS1", 1},
                {"FPS2", 2},
                {"Racing", 3},
                {"RTS", 4},
                {"Game1", 5},
                {"Game2", 6},
            }),
            new MenuEntry("Over Drive", "xe0", new Dictionary<string, int>
            {
                {"Off", 0},
                {"Level 1", 3},
                {"Level 2", 4},
                {"Level 3", 5},
                {"Level 4", 6},
            })
        ],
        [
            new MenuEntry("Brightness", "x10", new Range(0, 100)),
            new MenuEntry("Contrast", "x12", new Range(0, 100)),
            new MenuEntry("DCR", "xea", new Dictionary<string, int>
            {
                {"Off", 0},
                {"On", 1},
            }),
            new MenuEntry("HDR", "xef", new Dictionary<string, int>
            {
                {"Off", 0},
                {"HDR Auto", 1},
                {"HDR 400", 6},
                {"HDR Game", 7},
                {"HDR Movie", 8},
                {"HDR Photo", 9},
            }),
            new MenuEntry("Scaling Mode", "x86", new Dictionary<string, int>
            {
                {"Keep aspect ratio", 2},
                {"Full Screen", 5},
            }),
            new MenuEntry("Sharpness", "x87", new Dictionary<string, int>
            {
                {"0%", 0},
                {"20%", 1},
                {"40%", 2},
                {"60%", 3},
                {"80%", 4},
                {"100%", 5},
            }),
            new MenuEntry("Relative gamma", "x72", new Dictionary<string, int>
            {
                {"-0.4", 0x5000},
                {"-0.2", 0x6400},
                {"Default", 0x7800},
                {"+0.2", 0x8c00},
                {"+0.4", 0xa000},
            })
        ],
        [
            new MenuEntry("Color Temperature", "x14", new Dictionary<string, int>
            {
                {"sRGB", 1},
                {"Warm (6500K)", 5},
                {"Neutral (7500K)", 6},
                {"Cool (9300K)", 8},
                {"User", 11},
            }),
            new MenuEntry("  Red", "x16", new Range(0, 100)),
            new MenuEntry("Green", "x18", new Range(0, 100)),
            new MenuEntry(" Blue", "x1a", new Range(0, 100)),
        ],
        [
            new MenuEntry("True split", "xf7", new Dictionary<string, int>
            {
                {"Off", 0},
                {"Mode 1 (50:50)", 1},
                {"Mode 2 (75:25)", 2},
            }),
            new MenuEntry("Input source", "x60", new Dictionary<string, int>
            {
                {"DisplayPort", 0xF},
                {"HDMI 1", 0x11},
                {"HDMI 2", 0x12},
                {"USB-C", 0x31},
            }),
            new MenuEntry("Picture by Picture", "xF5", true, 257),
            new MenuEntry("PbP input sources", "x60", new Dictionary<string, int>
            {
                {"DisplayPort", 0xF},
                {"HDMI 1", 0x11},
                {"HDMI 2", 0x12},
                {"USB-C", 0x31},
            }, ["Left", "Right"])
        ],
        [
            new MenuEntry("Language", "xcc", new Dictionary<string, int>
            {
                {"English", 2},
                {"French", 3},
                {"German", 4},
                {"Italian", 5},
                {"Japanese", 6},
                {"Russian", 9},
                {"Spanish", 10},
                {"Simplified Chinese", 13},
                {"Thai", 0x23},
                {"Ukrainian", 0x24},
            }),
            new MenuEntry("Restore factory defaults", "x04", false, 1),
            new MenuEntry("Restore color defaults", "x08", false, 1),
        ],
        [
            new MenuEntry("Administrative lock", "xf6", new Dictionary<string, int>
            {
                {"Off", 0},
                {"On", 1},
            }),
            new MenuEntry("Overlay", "xfc", new Dictionary<string, int>
            {
                {"Off", 0},
                {"FPS counter", 4},
                {"Countdown", 8},
                {"Crosshair", 16},
                {"System stats", 32},
                {"Stopwatch", 72}
            }),
            new MenuEntry("Power off", "xd6", false, 5)
        ]
    ];
}