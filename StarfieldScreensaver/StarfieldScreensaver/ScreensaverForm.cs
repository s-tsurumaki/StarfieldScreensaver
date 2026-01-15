using System.Diagnostics;

namespace StarfieldScreensaver;

internal sealed partial class ScreensaverForm : Form
{
    private readonly ScreensaverSettings _settings;
    private readonly ScreensaverMode _mode;
    private readonly IntPtr _previewHwnd;

    private readonly StarfieldEngine _engine;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly Stopwatch _sw = Stopwatch.StartNew();

    private Point _mouseStart;
    private bool _mouseStartCaptured;

    public ScreensaverForm(
        ScreensaverSettings settings,
        ScreensaverMode mode,
        IntPtr previewHwnd,
        Screen? targetScreen = null)
    {
        _settings = settings;
        _mode = mode;
        _previewHwnd = previewHwnd;

        _engine = new StarfieldEngine(_settings);

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        BackColor = Color.Black;

        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);
        DoubleBuffered = true;

        if (_mode == ScreensaverMode.Run)
        {
            var s = targetScreen ?? Screen.PrimaryScreen;
            Bounds = s.Bounds;
            TopMost = true;
            Cursor.Hide();
        }
        else if (_mode == ScreensaverMode.Preview)
        {
            // サイズは後で親HWNDに合わせる（OnHandleCreated内）
            TopMost = false;
        }

        _timer = new System.Windows.Forms.Timer
        {
            Interval = Math.Max(5, 1000 / Math.Clamp(_settings.TargetFps, 15, 240))
        };
        _timer.Tick += (_, _) =>
        {
            var dt = (float)_sw.Elapsed.TotalSeconds;
            _sw.Restart();

            _engine.Update(dt, ClientSize.Width, ClientSize.Height);
            Invalidate();
        };

        KeyDown += (_, _) => RequestClose();
        MouseDown += (_, _) => RequestClose();
        MouseMove += (_, e) => HandleMouseMoveForExit(e.Location);

        // 一部環境で KeyDown が入らないことがあるため
        PreviewKeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.None) RequestClose();
        };
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (_mode == ScreensaverMode.Preview)
        {
            // プレビューウィンドウに埋め込む
            NativeMethods.SetParent(Handle, _previewHwnd);

            // 子ウィンドウスタイル付与
            var style = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_STYLE);
            style |= NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE;
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_STYLE, style);

            // 親のクライアントサイズに合わせる
            NativeMethods.GetClientRect(_previewHwnd, out var rc);
            NativeMethods.MoveWindow(Handle, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top, true);
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        _sw.Restart();
        _timer.Start();

        if (_mode == ScreensaverMode.Run)
        {
            // 終了判定の基準点
            _mouseStart = Cursor.Position;
            _mouseStartCaptured = true;
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer.Stop();
        if (_mode == ScreensaverMode.Run) Cursor.Show();
        base.OnFormClosed(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        _engine.Render(e.Graphics, ClientRectangle);
    }

    private void RequestClose()
    {
        // プレビューで閉じてしまうと挙動が変に見えることがあるので、プレビューは入力で閉じない方針もアリ。
        // 今回は軽く抑制: プレビューはマウス移動だけでは閉じない、クリック/キーは閉じる。
        Close();
    }

    private void HandleMouseMoveForExit(Point clientLocation)
    {
        if (_mode == ScreensaverMode.Preview)
            return;

        if (!_mouseStartCaptured)
        {
            _mouseStart = Cursor.Position;
            _mouseStartCaptured = true;
            return;
        }

        var now = Cursor.Position;
        int dx = now.X - _mouseStart.X;
        int dy = now.Y - _mouseStart.Y;

        // 小さな揺れは無視（寝返り耐性）
        if ((dx * dx + dy * dy) >= (_settings.ExitMouseMoveThresholdPx * _settings.ExitMouseMoveThresholdPx))
        {
            Close();
        }
    }
}
