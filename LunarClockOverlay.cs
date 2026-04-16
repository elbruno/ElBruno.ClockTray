namespace ClockTray;

/// <summary>
/// Borderless, always-on-top floating window that shows the current time,
/// Gregorian date, and Chinese lunar calendar information.
/// The window is draggable and does not steal focus from other applications.
/// </summary>
internal sealed class LunarClockOverlay : Form
{
    // Win32 extended window-style flags
    private const int WS_EX_NOACTIVATE  = 0x08000000;
    private const int WS_EX_TOOLWINDOW  = 0x00000080;

    private static readonly Color BackgroundColor = Color.FromArgb(30, 25, 64);
    private static readonly Color TextColor       = Color.White;

    private readonly Label _timeLabel;
    private readonly Label _lunarLabel;
    private readonly System.Windows.Forms.Timer _timer;
    private Point _dragOrigin;

    // Raised when the user closes the overlay via its own right-click menu.
    public event EventHandler? OverlayClosed;

    public LunarClockOverlay()
    {
        SuspendLayout();

        FormBorderStyle = FormBorderStyle.None;
        BackColor       = BackgroundColor;
        ForeColor       = TextColor;
        TopMost         = true;
        ShowInTaskbar   = false;
        Opacity         = 0.92;
        AutoSize        = true;
        AutoSizeMode    = AutoSizeMode.GrowAndShrink;
        StartPosition   = FormStartPosition.Manual;

        Font timeFont  = CreateDisplayFont(12f);
        Font lunarFont = CreateDisplayFont(11f);

        _timeLabel = new Label
        {
            Font      = timeFont,
            ForeColor = TextColor,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Margin    = new Padding(0, 0, 0, 2)
        };

        _lunarLabel = new Label
        {
            Font      = lunarFont,
            ForeColor = TextColor,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Margin    = new Padding(0)
        };

        var layout = new TableLayoutPanel
        {
            ColumnCount = 1,
            RowCount    = 2,
            AutoSize    = true,
            BackColor   = Color.Transparent,
            Margin      = new Padding(0),
            Padding     = new Padding(10, 6, 10, 6)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(_timeLabel,  0, 0);
        layout.Controls.Add(_lunarLabel, 0, 1);
        Controls.Add(layout);

        // Right-click context menu to close the overlay
        var closeMenu = new ContextMenuStrip();
        closeMenu.Items.Add("Close", null, OnCloseOverlay);
        foreach (Control ctrl in new Control[] { this, layout, _timeLabel, _lunarLabel })
            ctrl.ContextMenuStrip = closeMenu;

        // Mouse dragging
        WireDrag(this);
        WireDrag(layout);
        WireDrag(_timeLabel);
        WireDrag(_lunarLabel);

        // Position near the bottom-right corner (above the taskbar)
        PlaceNearTray();

        // Update every second
        _timer = new System.Windows.Forms.Timer { Interval = 1000 };
        _timer.Tick += (_, _) => UpdateDisplay();
        _timer.Start();

        UpdateDisplay();
        ResumeLayout();
    }

    // Prevent the overlay from stealing focus
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
            return cp;
        }
    }

    private void PlaceNearTray()
    {
        var work = Screen.PrimaryScreen?.WorkingArea
                   ?? Screen.GetWorkingArea(new Point(0, 0));
        // Start at the bottom-right; the form will resize itself via AutoSize
        Location = new Point(work.Right - 240, work.Bottom - 90);
    }

    private void WireDrag(Control ctrl)
    {
        ctrl.MouseDown += (s, e) =>
        {
            if (e.Button != MouseButtons.Left) return;
            var src = s as Control ?? this;
            var screen = src.PointToScreen(e.Location);
            _dragOrigin = new Point(screen.X - Left, screen.Y - Top);
        };

        ctrl.MouseMove += (s, e) =>
        {
            if (e.Button != MouseButtons.Left) return;
            var src = s as Control ?? this;
            var screen = src.PointToScreen(e.Location);
            Location = new Point(screen.X - _dragOrigin.X, screen.Y - _dragOrigin.Y);
        };
    }

    private void UpdateDisplay()
    {
        var now = DateTime.Now;
        _timeLabel.Text  = $"{now:HH:mm:ss}  {now:yy-MM-dd}";
        _lunarLabel.Text = ChineseCalendarHelper.FormatCalendarLine(now);
    }

    private void OnCloseOverlay(object? sender, EventArgs e)
    {
        OverlayClosed?.Invoke(this, EventArgs.Empty);
        Close();
    }

    // ── font selection ────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the best available font for CJK character display.
    /// Prefers "Microsoft YaHei UI" (Windows 10/11) and falls back gracefully.
    /// </summary>
    private static Font CreateDisplayFont(float size)
    {
        string[] preferred = { "Microsoft YaHei UI", "Microsoft YaHei", "SimHei", "Segoe UI" };
        var installed = new HashSet<string>(
            FontFamily.Families.Select(f => f.Name),
            StringComparer.OrdinalIgnoreCase);

        foreach (var name in preferred)
        {
            if (installed.Contains(name))
                return new Font(name, size);
        }

        return new Font(FontFamily.GenericSansSerif, size);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Stop();
            _timer.Dispose();
            _timeLabel.Font?.Dispose();
            _lunarLabel.Font?.Dispose();
        }
        base.Dispose(disposing);
    }
}
