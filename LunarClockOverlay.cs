using System.Runtime.InteropServices;

namespace ClockTray;

/// <summary>
/// Borderless, topmost, draggable overlay that shows the current time,
/// Chinese lunar calendar info (with zodiac), and solar term data.
/// </summary>
public sealed class LunarClockOverlay : Form
{
    private const int WS_EX_NOACTIVATE = 0x08000000;
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    private readonly System.Windows.Forms.Timer _timer;
    private readonly Label _line1;
    private readonly Label _line2;
    private readonly Label _line3;

    private bool _dragging;
    private Point _dragStart;

    /// <summary>
    /// Raised when the overlay is closed via right-click context menu.
    /// </summary>
    public event EventHandler? OverlayClosed;

    public LunarClockOverlay()
    {
        // Form setup
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        BackColor = Color.FromArgb(30, 25, 64);
        Opacity = 0.92;
        AutoScaleMode = AutoScaleMode.Dpi;
        Size = new Size(340, 100);

        // Position at bottom-right of primary screen
        var workArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
        Location = new Point(workArea.Right - Width - 12, workArea.Bottom - Height - 12);

        var font = ResolveFont();
        var fontSmall = new Font(font.FontFamily, font.Size - 1f, FontStyle.Regular);

        _line1 = CreateLabel(font, new Point(10, 6));
        _line2 = CreateLabel(font, new Point(10, 34));
        _line3 = CreateLabel(fontSmall, new Point(10, 62));

        Controls.Add(_line1);
        Controls.Add(_line2);
        Controls.Add(_line3);

        // Right-click to close
        var closeMenu = new ContextMenuStrip();
        closeMenu.Items.Add("Close", null, (_, _) =>
        {
            OverlayClosed?.Invoke(this, EventArgs.Empty);
            Close();
        });
        ContextMenuStrip = closeMenu;

        // Drag support on form and labels
        EnableDrag(this);
        EnableDrag(_line1);
        EnableDrag(_line2);
        EnableDrag(_line3);

        // Timer: update every second
        _timer = new System.Windows.Forms.Timer { Interval = 1000 };
        _timer.Tick += (_, _) => UpdateDisplay();
        _timer.Start();

        UpdateDisplay();
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
            return cp;
        }
    }

    protected override bool ShowWithoutActivation => true;

    private void UpdateDisplay()
    {
        var now = DateTime.Now;

        // Line 1: Time + Gregorian date
        _line1.Text = now.ToString("HH:mm:ss  yy-MM-dd");

        // Line 2: Chinese calendar line (stem-branch year + zodiac + lunar date + day of week)
        _line2.Text = ChineseCalendarHelper.FormatCalendarLine(now);

        // Line 3: Solar term info
        var todayTerm = SolarTermCalculator.GetSolarTerm(now);
        if (todayTerm != null)
        {
            _line3.Text = $"今日节气: {todayTerm}";
        }
        else
        {
            var next = SolarTermCalculator.GetCurrentOrNextSolarTerm(now);
            _line3.Text = $"下个节气: {next.ChineseName} ({next.EnglishName}) {next.Date:MM-dd}";
        }
    }

    private static Font ResolveFont()
    {
        string[] preferred = ["Microsoft YaHei UI", "Microsoft YaHei", "SimHei", "Segoe UI"];

        foreach (var name in preferred)
        {
            using var test = new Font(name, 13f, FontStyle.Regular);
            if (test.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return new Font(name, 13f, FontStyle.Regular);
        }

        return new Font("Segoe UI", 13f, FontStyle.Regular);
    }

    private static Label CreateLabel(Font font, Point location)
    {
        return new Label
        {
            Font = font,
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = location,
        };
    }

    private void EnableDrag(Control control)
    {
        control.MouseDown += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStart = e.Location;
            }
        };

        control.MouseMove += (_, e) =>
        {
            if (_dragging)
            {
                var screen = PointToScreen(e.Location);
                Location = new Point(screen.X - _dragStart.X, screen.Y - _dragStart.Y);
            }
        };

        control.MouseUp += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
                _dragging = false;
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Stop();
            _timer.Dispose();
            _line1.Dispose();
            _line2.Dispose();
            _line3.Dispose();
        }
        base.Dispose(disposing);
    }
}
