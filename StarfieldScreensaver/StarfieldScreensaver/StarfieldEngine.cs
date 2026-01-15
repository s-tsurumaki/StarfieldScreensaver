using System.Drawing.Drawing2D;

namespace StarfieldScreensaver;

internal sealed class StarfieldEngine
{
    private readonly Random _rng = new();
    private ScreensaverSettings _settings;

    private Star[] _stars = Array.Empty<Star>();

    private int _lastW;
    private int _lastH;

    public StarfieldEngine(ScreensaverSettings settings)
    {
        _settings = settings;
    }

    public void Update(float dt, int width, int height)
    {
        if (width <= 0 || height <= 0) return;

        // 画面サイズが変わった or 初回 or 設定変更を想定して再生成
        if (_stars.Length != _settings.StarCount || width != _lastW || height != _lastH)
        {
            _lastW = width;
            _lastH = height;
            _stars = new Star[_settings.StarCount];
            for (int i = 0; i < _stars.Length; i++)
                Respawn(ref _stars[i], width, height, keepZRandom: true);
        }

        float speed = _settings.WarpSpeed;           // Z方向の速度
        float maxZ = _settings.MaxDepth;
        float spread = _settings.Spread;

        // dt が跳ねた時に突き抜けすぎないよう抑える
        dt = MathF.Min(dt, 0.05f);

        for (int i = 0; i < _stars.Length; i++)
        {
            ref var s = ref _stars[i];

            // 前フレームの投影位置（トレイル用）
            s.PrevPx = s.Px;
            s.PrevPy = s.Py;

            s.Z -= speed * dt;

            // 手前を超えたら奥へ
            if (s.Z <= 0.8f)
            {
                Respawn(ref s, width, height, keepZRandom: false);
                continue;
            }

            // 投影
            Project(ref s, width, height);

            // 画面外へ飛び出した場合もリスポーン（ワープ感維持）
            if (s.Px < -100 || s.Px > width + 100 || s.Py < -100 || s.Py > height + 100 || s.Z > maxZ)
            {
                Respawn(ref s, width, height, keepZRandom: false);
            }
        }
    }

    public void Render(Graphics g, Rectangle bounds)
    {
        g.SmoothingMode = _settings.Smooth ? SmoothingMode.AntiAlias : SmoothingMode.None;
        g.Clear(_settings.BackgroundColor);

        if (_stars.Length == 0) return;

        using var pen = new Pen(Color.White, _settings.TrailThickness);
        using var brush = new SolidBrush(Color.White);

        float maxZ = _settings.MaxDepth;

        // “奥が薄く、手前が濃い” を雑に再現（alphaを距離で変える）
        // ※ Color を星ごとに new すると重いので、ここは軽量寄りに妥協。
        for (int i = 0; i < _stars.Length; i++)
        {
            var s = _stars[i];

            // サイズ: 手前ほど大きい
            float t = 1f - (s.Z / maxZ);
            float size = MathF.Max(1f, _settings.StarSizeMin + t * (_settings.StarSizeMax - _settings.StarSizeMin));

            // 透明度: 手前ほど濃い
            int a = (int)Math.Clamp(_settings.StarAlphaMin + t * (_settings.StarAlphaMax - _settings.StarAlphaMin), 0, 255);

            if (_settings.DrawTrails)
            {
                // トレイルは前フレーム位置との線
                // 透明度の適用のため Pen を変える（ここで new するが、星数が多いと重い）
                // 重い場合は固定色にする or まとめ描きにするのが次の改善点。
                using var trailPen = new Pen(Color.FromArgb(a, 255, 255, 255), _settings.TrailThickness);
                g.DrawLine(trailPen, s.PrevPx, s.PrevPy, s.Px, s.Py);
            }

            using var starBrush = new SolidBrush(Color.FromArgb(a, 255, 255, 255));
            g.FillEllipse(starBrush, s.Px - size * 0.5f, s.Py - size * 0.5f, size, size);
        }
    }

    private void Respawn(ref Star s, int width, int height, bool keepZRandom)
    {
        float spread = _settings.Spread;
        float maxZ = _settings.MaxDepth;

        s.X = (float)(_rng.NextDouble() * 2 - 1) * spread;
        s.Y = (float)(_rng.NextDouble() * 2 - 1) * spread;
        s.Z = keepZRandom ? (float)(_rng.NextDouble() * maxZ) + 1f : maxZ;

        s.Px = width * 0.5f;
        s.Py = height * 0.5f;
        s.PrevPx = s.Px;
        s.PrevPy = s.Py;

        Project(ref s, width, height);
    }

    private void Project(ref Star s, int width, int height)
    {
        float cx = width * 0.5f;
        float cy = height * 0.5f;

        float invZ = 1f / s.Z;
        float fov = _settings.FieldOfView;

        s.Px = cx + s.X * invZ * fov;
        s.Py = cy + s.Y * invZ * fov;
    }

    private struct Star
    {
        public float X, Y, Z;
        public float Px, Py;
        public float PrevPx, PrevPy;
    }
}
