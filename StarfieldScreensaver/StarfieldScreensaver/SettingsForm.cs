namespace StarfieldScreensaver;

internal sealed partial class SettingsForm : Form
{
    private readonly ScreensaverSettings _settings;

    private NumericUpDown numStars = new();
    private NumericUpDown numSpeed = new();
    private NumericUpDown numFov = new();
    private NumericUpDown numSpread = new();
    private NumericUpDown numDepth = new();
    private CheckBox chkTrails = new();
    private NumericUpDown numTrail = new();
    private NumericUpDown numFps = new();
    private CheckBox chkSmooth = new();

    private Button btnSave = new();
    private Button btnCancel = new();

    public SettingsForm()
    {
        Text = "Starfield Screensaver Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(460, 360);

        _settings = ScreensaverSettings.Load();

        BuildUi();
        LoadToUi();
    }

    private void BuildUi()
    {
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 10,
            Padding = new Padding(12),
            AutoSize = false
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));

        Label L(string t) => new() { Text = t, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };

        numStars = Num(100, 5000, 10);
        numSpeed = Num(10, 600, 5, decimalPlaces: 0);
        numFov = Num(100, 1500, 10, decimalPlaces: 0);
        numSpread = Num(50, 2000, 10, decimalPlaces: 0);
        numDepth = Num(50, 800, 10, decimalPlaces: 0);

        chkTrails = new CheckBox { Text = "Trails (speed lines)", Dock = DockStyle.Fill };
        numTrail = Num(1, 6, 1, decimalPlaces: 1, increment: 0.2m);
        chkSmooth = new CheckBox { Text = "Antialias (heavier)", Dock = DockStyle.Fill };
        numFps = Num(15, 240, 1);

        grid.Controls.Add(L("Star count (density)"), 0, 0);
        grid.Controls.Add(numStars, 1, 0);

        grid.Controls.Add(L("Warp speed"), 0, 1);
        grid.Controls.Add(numSpeed, 1, 1);

        grid.Controls.Add(L("Field of view"), 0, 2);
        grid.Controls.Add(numFov, 1, 2);

        grid.Controls.Add(L("Spread"), 0, 3);
        grid.Controls.Add(numSpread, 1, 3);

        grid.Controls.Add(L("Max depth"), 0, 4);
        grid.Controls.Add(numDepth, 1, 4);

        grid.Controls.Add(chkTrails, 0, 5);
        grid.SetColumnSpan(chkTrails, 2);

        grid.Controls.Add(L("Trail thickness"), 0, 6);
        grid.Controls.Add(numTrail, 1, 6);

        grid.Controls.Add(chkSmooth, 0, 7);
        grid.SetColumnSpan(chkSmooth, 2);

        grid.Controls.Add(L("Target FPS"), 0, 8);
        grid.Controls.Add(numFps, 1, 8);

        var panelButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 10, 0, 0)
        };

        btnSave = new Button { Text = "Save", Width = 100 };
        btnCancel = new Button { Text = "Cancel", Width = 100 };

        btnSave.Click += (_, _) => SaveAndClose();
        btnCancel.Click += (_, _) => Close();

        panelButtons.Controls.Add(btnSave);
        panelButtons.Controls.Add(btnCancel);

        grid.Controls.Add(panelButtons, 0, 9);
        grid.SetColumnSpan(panelButtons, 2);

        Controls.Add(grid);

        chkTrails.CheckedChanged += (_, _) => numTrail.Enabled = chkTrails.Checked;
    }

    private void LoadToUi()
    {
        numStars.Value = Math.Clamp(_settings.StarCount, (int)numStars.Minimum, (int)numStars.Maximum);
        numSpeed.Value = (decimal)_settings.WarpSpeed;
        numFov.Value = (decimal)_settings.FieldOfView;
        numSpread.Value = (decimal)_settings.Spread;
        numDepth.Value = (decimal)_settings.MaxDepth;

        chkTrails.Checked = _settings.DrawTrails;
        numTrail.Value = (decimal)_settings.TrailThickness;

        chkSmooth.Checked = _settings.Smooth;
        numFps.Value = Math.Clamp(_settings.TargetFps, (int)numFps.Minimum, (int)numFps.Maximum);

        numTrail.Enabled = chkTrails.Checked;
    }

    private void SaveAndClose()
    {
        _settings.StarCount = (int)numStars.Value;
        _settings.WarpSpeed = (float)numSpeed.Value;
        _settings.FieldOfView = (float)numFov.Value;
        _settings.Spread = (float)numSpread.Value;
        _settings.MaxDepth = (float)numDepth.Value;

        _settings.DrawTrails = chkTrails.Checked;
        _settings.TrailThickness = (float)numTrail.Value;

        _settings.Smooth = chkSmooth.Checked;
        _settings.TargetFps = (int)numFps.Value;

        _settings.Save();
        Close();
    }

    private static NumericUpDown Num(decimal min, decimal max, decimal value, int decimalPlaces = 0, decimal increment = 1m)
    {
        var v = Math.Clamp(value, min, max);

        return new NumericUpDown
        {
            Minimum = min,
            Maximum = max,
            Value = v,
            DecimalPlaces = decimalPlaces,
            Increment = increment,
            Dock = DockStyle.Fill
        };
    }
}
