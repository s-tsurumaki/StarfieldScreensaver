using System.Globalization;

namespace StarfieldScreensaver;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var (mode, previewHwnd) = ParseArgs(args);

        // デバッグや直接実行時は設定画面を出す（Windowsの通常動作では /s /c /p が来る）
        if (mode == ScreensaverMode.Config)
        {
            using var form = new SettingsForm();
            Application.Run(form);
            return;
        }

        if (mode == ScreensaverMode.Preview && previewHwnd == IntPtr.Zero)
        {
            // プレビュー要求なのにHWNDが無い場合は設定にフォールバック
            using var form = new SettingsForm();
            Application.Run(form);
            return;
        }

        var settings = ScreensaverSettings.Load();

        if (mode == ScreensaverMode.Preview)
        {
            // プレビューは1枚のみ
            using var previewForm = new ScreensaverForm(settings, mode, previewHwnd);
            Application.Run(previewForm);
            return;
        }

        // 全画面（マルチモニタ）: ApplicationContext で複数フォームをまとめる
        var context = new ScreensaverAppContext(settings);
        Application.Run(context);
    }

    private static (ScreensaverMode mode, IntPtr previewHwnd) ParseArgs(string[] args)
    {
        if (args.Length == 0) return (ScreensaverMode.Config, IntPtr.Zero);

        // 例:
        // /s
        // /c
        // /p 123456
        // /p:123456
        string a0 = args[0].Trim().ToLowerInvariant();

        if (a0.StartsWith("/s")) return (ScreensaverMode.Run, IntPtr.Zero);
        if (a0.StartsWith("/c")) return (ScreensaverMode.Config, IntPtr.Zero);

        if (a0.StartsWith("/p"))
        {
            // /p:HWND の形式
            if (a0.Contains(':'))
            {
                var parts = a0.Split(':', 2);
                if (parts.Length == 2 && TryParseHwnd(parts[1], out var hwnd1))
                    return (ScreensaverMode.Preview, hwnd1);
            }

            // /p HWND の形式
            if (args.Length >= 2 && TryParseHwnd(args[1], out var hwnd2))
                return (ScreensaverMode.Preview, hwnd2);

            return (ScreensaverMode.Preview, IntPtr.Zero);
        }

        // 想定外は設定画面
        return (ScreensaverMode.Config, IntPtr.Zero);
    }

    private static bool TryParseHwnd(string s, out IntPtr hwnd)
    {
        s = s.Trim();

        // decimal
        if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
        {
            hwnd = new IntPtr(v);
            return true;
        }

        // hex (0x...)
        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
            long.TryParse(s[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hx))
        {
            hwnd = new IntPtr(hx);
            return true;
        }

        hwnd = IntPtr.Zero;
        return false;
    }
}

internal enum ScreensaverMode
{
    Run,
    Preview,
    Config
}
