// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using System.Numerics;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Input;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.SS220.PictureViewer;

[GenerateTypedNameReferences]
public sealed partial class PictureViewer : Control
{
    [Dependency] private readonly IGameTiming _timing = default!;

    private ResPath _viewedPicture;

    private const float ScrollSensitivity = 0.075f;
    public const float MaxZoom = 5f;
    public const float MinZoom = 1f;
    private readonly float _recenterMinimum = 0.05f;

    public float Zoom { get; internal set; } = MinZoom;
    public bool Recentering { get; internal set; } = false;
    private bool _draggin = false;
    private Vector2 _offset = Vector2.Zero;

    public ResPath ViewedPicture
    {
        get => _viewedPicture;
        set
        {
            _viewedPicture = value;
            Picture.TexturePath = value.ToString();
            NoImageLabel.Visible = false;
        }
    }

    public PictureViewer()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        UpdateZoom();
    }

    private void UpdateZoom()
    {
        var invZoom = 1 / Zoom;
        Picture.TextureScale = new Vector2(invZoom, invZoom);
    }

    private void UpdateOffset(Vector2? finalSize = null)
    {
        var position = _offset;
        var rect = UIBox2.FromDimensions(position, finalSize ?? this.Size);
        Picture.Arrange(rect);
    }

    protected override Vector2 ArrangeOverride(Vector2 finalSize)
    {
        UpdateOffset(finalSize);

        return finalSize;
    }

    protected override void MouseWheel(GUIMouseWheelEventArgs args)
    {
        base.MouseWheel(args);
        Zoom -= args.Delta.Y * ScrollSensitivity;
        Zoom = float.Clamp(Zoom, MinZoom, MaxZoom);

        UpdateZoom();
        UpdateOffset();

        args.Handle();
    }

    protected override void KeyBindDown(GUIBoundKeyEventArgs args)
    {
        base.KeyBindDown(args);

        if (args.Function == EngineKeyFunctions.Use)
        {
            _draggin = true;
        }
    }

    protected override void KeyBindUp(GUIBoundKeyEventArgs args)
    {
        base.KeyBindUp(args);

        if (args.Function == EngineKeyFunctions.Use)
        {
            _draggin = false;
        }
    }

    public void Recenter()
    {
        Recentering = true;
    }

    public bool CanBeRecentered()
    {
        return _offset.Length() > _recenterMinimum;
    }

    override protected void Draw(DrawingHandleScreen handle)
    {
        if (Recentering)
        {
            var frameTime = _timing.FrameTime;
            var diff = _offset * (float) frameTime.TotalSeconds;

            if (_offset.LengthSquared() < _recenterMinimum)
            {
                _offset = Vector2.Zero;
                Recentering = false;
            }
            else
            {
                _offset -= diff * 5f;
            }

            UpdateOffset();
        }
    }

    protected override void MouseMove(GUIMouseMoveEventArgs args)
    {
        base.MouseMove(args);

        if (!_draggin)
            return;

        Recentering = false;

        _offset += new Vector2(args.Relative.X, args.Relative.Y);
        UpdateOffset();
    }
}
