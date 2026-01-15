using System.Text.Json;

namespace StarfieldScreensaver;

internal sealed class ScreensaverSettings
{
    public int StarCount { get; set; } = 800;
    public float WarpSpeed { get; set; } = 120f;           // 大きいほど速い
    public float FieldOfView { get; set; } = 500f;         // 大きいほど広角
    public float Spread { get; set; } = 350f;              // 星の散らばり
    public float MaxDepth { get; set; } = 220f;            // Zの最大

    public bool DrawTrails { get; set; } = true;
    public float TrailThickness { get; set; } = 1.0f;

    public float StarSizeMin { get; set; } = 1.0f;
    public float StarSizeMax { get; set; } = 3.2f;

    public int StarAlphaMin { get; set; } = 40;
    public int StarAlphaMax { get; set; } = 255;

    public bool Smooth { get; set; } = false;
    public int TargetFps { get; set; } = 60;

    public int ExitMouseMoveThresholdPx { get; set; } = 12;

    // 背景色（今は固定）
    public Color BackgroundColor { get; set; } = Color.Black;

    public static ScreensaverSettings Load()
    {
        try
        {
            var path = GetSettingsPath();
            if (!File.Exists(path)) return new ScreensaverSettings();

            var json = File.ReadAllText(path);
            var dto = JsonSerializer.Deserialize<SettingsDto>(json);
            if (dto is null) return new ScreensaverSettings();

            return dto.ToSettings();
        }
        catch
        {
            return new ScreensaverSettings();
        }
    }

    public void Save()
    {
        var dir = GetSettingsDir();
        Directory.CreateDirectory(dir);

        var dto = SettingsDto.FromSettings(this);
        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(GetSettingsPath(), json);
    }

    private static string GetSettingsDir()
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "StarfieldScreensaver");

    private static string GetSettingsPath()
        => Path.Combine(GetSettingsDir(), "settings.json");

    private sealed class SettingsDto
    {
        public int StarCount { get; set; }
        public float WarpSpeed { get; set; }
        public float FieldOfView { get; set; }
        public float Spread { get; set; }
        public float MaxDepth { get; set; }

        public bool DrawTrails { get; set; }
        public float TrailThickness { get; set; }

        public float StarSizeMin { get; set; }
        public float StarSizeMax { get; set; }

        public int StarAlphaMin { get; set; }
        public int StarAlphaMax { get; set; }

        public bool Smooth { get; set; }
        public int TargetFps { get; set; }

        public int ExitMouseMoveThresholdPx { get; set; }

        public static SettingsDto FromSettings(ScreensaverSettings s) => new()
        {
            StarCount = s.StarCount,
            WarpSpeed = s.WarpSpeed,
            FieldOfView = s.FieldOfView,
            Spread = s.Spread,
            MaxDepth = s.MaxDepth,
            DrawTrails = s.DrawTrails,
            TrailThickness = s.TrailThickness,
            StarSizeMin = s.StarSizeMin,
            StarSizeMax = s.StarSizeMax,
            StarAlphaMin = s.StarAlphaMin,
            StarAlphaMax = s.StarAlphaMax,
            Smooth = s.Smooth,
            TargetFps = s.TargetFps,
            ExitMouseMoveThresholdPx = s.ExitMouseMoveThresholdPx
        };

        public ScreensaverSettings ToSettings() => new()
        {
            StarCount = StarCount,
            WarpSpeed = WarpSpeed,
            FieldOfView = FieldOfView,
            Spread = Spread,
            MaxDepth = MaxDepth,
            DrawTrails = DrawTrails,
            TrailThickness = TrailThickness,
            StarSizeMin = StarSizeMin,
            StarSizeMax = StarSizeMax,
            StarAlphaMin = StarAlphaMin,
            StarAlphaMax = StarAlphaMax,
            Smooth = Smooth,
            TargetFps = TargetFps,
            ExitMouseMoveThresholdPx = ExitMouseMoveThresholdPx
        };
    }
}
