using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ClockTray;

/// <summary>
/// Toggles the Windows taskbar clock visibility via registry.
/// Uses Win11 23H2+ method (no Explorer restart) with fallback to policy key.
/// </summary>
public static class ClockToggler
{
    private const string Win11AdvancedKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private const string Win11ValueName = "ShowSystrayDateTimeValueName";

    private const string PolicyKey = @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
    private const string PolicyValueName = "HideClock";

    private const uint HWND_BROADCAST = 0xFFFF;
    private const uint WM_SETTINGCHANGE = 0x001A;
    private const uint SMTO_ABORTIFHUNG = 0x0002;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam,
        uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);

    public static bool IsWin11Modern()
    {
        // Win11 23H2+ has build >= 22631
        return Environment.OSVersion.Version.Build >= 22631;
    }

    public static bool IsClockVisible()
    {
        if (IsWin11Modern())
        {
            using var key = Registry.CurrentUser.OpenSubKey(Win11AdvancedKey);
            var val = key?.GetValue(Win11ValueName);
            // Default (no key) = visible. 1 = show, 0 = hide.
            return val is not int intVal || intVal != 0;
        }
        else
        {
            using var key = Registry.CurrentUser.OpenSubKey(PolicyKey);
            var val = key?.GetValue(PolicyValueName);
            // Default (no key) = visible. 1 = hidden.
            return val is not int intVal || intVal != 1;
        }
    }

    public static void SetClockVisibility(bool show)
    {
        if (IsWin11Modern())
        {
            SetWin11Modern(show);
        }
        else
        {
            SetPolicyFallback(show);
        }
    }

    private static void SetWin11Modern(bool show)
    {
        using var key = Registry.CurrentUser.OpenSubKey(Win11AdvancedKey, writable: true);
        if (key == null) return;

        key.SetValue(Win11ValueName, show ? 1 : 0, RegistryValueKind.DWord);

        // Broadcast setting change — no Explorer restart needed
        SendMessageTimeout(
            (IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE,
            UIntPtr.Zero, "TraySettings",
            SMTO_ABORTIFHUNG, 100, out _);
    }

    private static void SetPolicyFallback(bool show)
    {
        using var key = Registry.CurrentUser.CreateSubKey(PolicyKey);
        if (show)
            key.DeleteValue(PolicyValueName, throwOnMissingValue: false);
        else
            key.SetValue(PolicyValueName, 1, RegistryValueKind.DWord);

        // Restart Explorer for policy to take effect
        RestartExplorer();
    }

    private static void RestartExplorer()
    {
        try
        {
            foreach (var proc in Process.GetProcessesByName("explorer"))
                proc.Kill();

            Process.Start("explorer.exe");
        }
        catch
        {
            // Best effort
        }
    }
}
