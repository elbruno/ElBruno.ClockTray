using System.Runtime.InteropServices;

namespace ClockTray;

public class ClockTrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _showItem;
    private readonly ToolStripMenuItem _hideItem;
    private readonly HotkeyWindow _hotkeyWindow;

    public ClockTrayApplicationContext()
    {
        _showItem = new ToolStripMenuItem("Show Date/Time", null, OnShow);
        _hideItem = new ToolStripMenuItem("Hide Date/Time", null, OnHide);

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add(_showItem);
        contextMenu.Items.Add(_hideItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Exit", null, OnExit);

        _notifyIcon = new NotifyIcon
        {
            Icon = CreateClockIcon(),
            Text = "ClockTray — Toggle taskbar clock",
            ContextMenuStrip = contextMenu,
            Visible = true
        };

        _notifyIcon.DoubleClick += (_, _) => Toggle();

        UpdateMenuState();

        _hotkeyWindow = new HotkeyWindow(Toggle);
    }

    private static Icon CreateClockIcon()
    {
        // Try loading embedded icon first
        var icoPath = Path.Combine(AppContext.BaseDirectory, "clock.ico");
        if (File.Exists(icoPath))
            return new Icon(icoPath);

        // Fallback: draw a simple clock icon
        var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);
        using var pen = new Pen(Color.White, 2);
        g.FillEllipse(Brushes.DodgerBlue, 2, 2, 28, 28);
        g.DrawEllipse(pen, 2, 2, 28, 28);
        // Clock hands
        g.DrawLine(pen, 16, 16, 16, 7);  // hour
        g.DrawLine(pen, 16, 16, 23, 16); // minute
        return Icon.FromHandle(bmp.GetHicon());
    }

    private void UpdateMenuState()
    {
        bool isVisible = ClockToggler.IsClockVisible();
        _showItem.Enabled = !isVisible;
        _hideItem.Enabled = isVisible;
    }

    private void OnShow(object? sender, EventArgs e)
    {
        ClockToggler.SetClockVisibility(true);
        UpdateMenuState();
    }

    private void OnHide(object? sender, EventArgs e)
    {
        ClockToggler.SetClockVisibility(false);
        UpdateMenuState();
    }

    private void Toggle()
    {
        bool current = ClockToggler.IsClockVisible();
        ClockToggler.SetClockVisibility(!current);
        UpdateMenuState();
    }

    private void OnExit(object? sender, EventArgs e)
    {
        _hotkeyWindow.Dispose();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _hotkeyWindow.Dispose();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
        base.Dispose(disposing);
    }
}
