namespace StarfieldScreensaver;

internal sealed class ScreensaverAppContext : ApplicationContext
{
    private int _openForms;

    public ScreensaverAppContext(ScreensaverSettings settings)
    {
        var screens = Screen.AllScreens;
        _openForms = screens.Length;

        foreach (var s in screens)
        {
            var form = new ScreensaverForm(settings, ScreensaverMode.Run, IntPtr.Zero, s);
            form.FormClosed += (_, _) =>
            {
                _openForms--;
                if (_openForms <= 0)
                    ExitThread();
            };
            form.Show();
        }
    }
}
