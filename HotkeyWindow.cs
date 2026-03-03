using System.Runtime.InteropServices;

namespace ClockTray;

/// <summary>
/// Hidden window that registers Ctrl+Alt+T as a global hotkey.
/// </summary>
public class HotkeyWindow : NativeWindow, IDisposable
{
    private const int HOTKEY_ID = 0x0001;
    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_CTRL = 0x0002;
    private const uint MOD_ALT = 0x0001;
    private const uint VK_T = 0x54;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private readonly Action _onHotkey;
    private bool _disposed;

    public HotkeyWindow(Action onHotkey)
    {
        _onHotkey = onHotkey;
        CreateHandle(new CreateParams());
        RegisterHotKey(Handle, HOTKEY_ID, MOD_CTRL | MOD_ALT, VK_T);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            _onHotkey();
        }
        base.WndProc(ref m);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            UnregisterHotKey(Handle, HOTKEY_ID);
            DestroyHandle();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
