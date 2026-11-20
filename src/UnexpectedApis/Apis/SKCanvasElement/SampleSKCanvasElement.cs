#if !WINDOWS
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;

namespace UnexpectedApis.Apis;

public sealed class SampleSKCanvasElement : SKCanvasElement
{
    public const int SampleCount = 3;

    private int _sample;

    public int Sample
    {
        get => _sample;
        set
        {
            _sample = Math.Clamp(value, 0, SampleCount - 1);
            Invalidate();
        }
    }

    protected override void RenderOverride(SKCanvas canvas, Size area)
    {
        // Every drawing assumes a 260x260 box, centered and scaled to fit.
        var side = Math.Min(area.Width, area.Height);
        canvas.Translate((float)((area.Width - side) / 2), (float)((area.Height - side) / 2));
        canvas.Scale((float)(side / 260), (float)(side / 260));

        switch (Sample)
        {
            case 0:
                DrawShapes(canvas);
                break;
            case 1:
                DrawCurves(canvas);
                break;
            case 2:
                DrawGradientStar(canvas);
                break;
        }
    }

    // https://fiddle.skia.org/c/@shapes
    private static void DrawShapes(SKCanvas canvas)
    {
        using SKPaint paint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 4,
            Color = new SKColor(0xff4285F4),
        };

        var rect = SKRect.Create(10, 10, 100, 160);
        canvas.DrawRect(rect, paint);

        using SKPathBuilder roundedBuilder = new();
        roundedBuilder.AddRoundRect(rect, 20, 20);
        using var rounded = roundedBuilder.Detach();
        rounded.Offset(new SKPoint(40, 80));
        paint.Color = new SKColor(0xffDB4437);
        canvas.DrawPath(rounded, paint);

        paint.Color = new SKColor(0xff0F9D58);
        canvas.DrawCircle(180, 50, 25, paint);

        rect.Offset(80, 50);
        paint.Color = new SKColor(0xffF4B400);
        paint.Style = SKPaintStyle.Stroke;
        canvas.DrawRoundRect(rect, 10, 10, paint);
    }

    // https://fiddle.skia.org/c/@bezier_curves
    private static void DrawCurves(SKCanvas canvas)
    {
        using SKPaint paint = new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 8,
            Color = new SKColor(0xff4285F4),
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
        };

        using SKPathBuilder builder = new();
        builder.MoveTo(10, 10);
        builder.QuadTo(256, 64, 128, 128);
        builder.QuadTo(10, 192, 250, 250);
        using var path = builder.Detach();
        canvas.DrawPath(path, paint);
    }

    // https://fiddle.skia.org/c/@shader
    private static void DrawGradientStar(SKCanvas canvas)
    {
        using var pathEffect = SKPathEffect.CreateDiscrete(10.0f, 4.0f);
        using var shader = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(256, 256),
            [new SKColor(66, 133, 244), new SKColor(15, 157, 88)],
            SKShaderTileMode.Clamp);
        using SKPaint paint = new()
        {
            PathEffect = pathEffect,
            Shader = shader,
            IsAntialias = true,
        };

        const float radius = 60, center = 128;
        using SKPathBuilder builder = new();
        builder.MoveTo(center + radius, center);
        for (var i = 1; i < 15; i++)
        {
            var angle = 0.44879895f * i;
            var r = radius + radius * (i % 2);
            builder.LineTo((float)(center + r * Math.Cos(angle)), (float)(center + r * Math.Sin(angle)));
        }

        using var star = builder.Detach();
        canvas.DrawPath(star, paint);
    }
}
#endif
