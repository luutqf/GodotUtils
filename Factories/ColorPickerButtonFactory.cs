using Godot;

namespace GodotUtils;

public static class ColorPickerButtonFactory
{
    public static ColorPickerButton Create(Color initialColor)
    {
        ColorPickerButton button = new()
        {
            CustomMinimumSize = Vector2.One * 30
        };

        button.PickerCreated += () =>
        {
            ColorPicker picker = button.GetPicker();

            picker.Color = initialColor;

            PopupPanel popupPanel = picker.GetParent<PopupPanel>();

            popupPanel.InitialPosition = Window.WindowInitialPosition.Absolute;

            popupPanel.AboutToPopup += () =>
            {
                Vector2 viewportSize = popupPanel.GetTree().Root.GetViewport().GetVisibleRect().Size;

                // Position the ColorPicker to be at the top right of the screen
                popupPanel.Position = new Vector2I((int)(viewportSize.X - popupPanel.Size.X), 0);
            };
        };

        button.PopupClosed += button.ReleaseFocus;

        return button;
    }
}
