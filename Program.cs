namespace ClockTray;

static class Program
{
    private const string Version = "1.0.0";

    static int Main(string[] args)
    {
        // CLI mode: parse arguments and run headlessly
        if (args.Length > 0)
        {
            return RunCli(args);
        }

        // GUI mode: launch tray app with STA thread
        return RunGui();
    }

    private static int RunCli(string[] args)
    {
        string command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "show":
                ClockToggler.SetClockVisibility(true);
                Console.WriteLine("✓ Taskbar clock shown.");
                return 0;

            case "hide":
                ClockToggler.SetClockVisibility(false);
                Console.WriteLine("✓ Taskbar clock hidden.");
                return 0;

            case "status":
                bool isVisible = ClockToggler.IsClockVisible();
                bool isWin11 = ClockToggler.IsWin11Modern();
                Console.WriteLine($"Clock Status: {(isVisible ? "Visible" : "Hidden")}");
                Console.WriteLine($"OS Method: {(isWin11 ? "Windows 11 Modern (23H2+)" : "Legacy Policy Key")}");
                return 0;

            case "--help":
            case "-h":
            case "help":
                PrintHelp();
                return 0;

            case "--version":
            case "-v":
            case "version":
                Console.WriteLine($"ClockTray v{Version}");
                return 0;

            default:
                Console.Error.WriteLine($"Error: Unknown command '{args[0]}'");
                Console.Error.WriteLine();
                PrintHelp();
                return 1;
        }
    }

    [STAThread]
    private static int RunGui()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new ClockTrayApplicationContext());
        return 0;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("ClockTray - Toggle Windows taskbar clock visibility");
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("  clocktray              Launch system tray app (GUI mode)");
        Console.WriteLine("  clocktray show         Show the taskbar clock");
        Console.WriteLine("  clocktray hide         Hide the taskbar clock");
        Console.WriteLine("  clocktray status       Check clock visibility status");
        Console.WriteLine("  clocktray --help       Display this help");
        Console.WriteLine("  clocktray --version    Display version");
        Console.WriteLine();
        Console.WriteLine("GUI Mode:");
        Console.WriteLine("  - Runs in system tray with context menu");
        Console.WriteLine("  - Global hotkey: Ctrl+Alt+T");
        Console.WriteLine();
        Console.WriteLine("CLI Mode:");
        Console.WriteLine("  - Runs headlessly (no UI, exits after action)");
        Console.WriteLine("  - Supports Windows 11 23H2+ modern method (instant)");
        Console.WriteLine("  - Falls back to legacy policy key on older Windows");
    }
}