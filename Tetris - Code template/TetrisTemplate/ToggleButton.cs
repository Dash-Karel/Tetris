using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/// <summary>
/// ToggleButton is a child class of the button
/// The toggle button is used when a button can either be on or off.
/// </summary>
internal class ToggleButton : Button
{
    public bool IsPressed { get; private set; }

    Color notPressedColor, pressedColor;
    //constructor
    public ToggleButton(Vector2 _topLeftPoint, Vector2 _size, string _buttonText, Texture2D _buttonTexture, SpriteFont _standardFont, Color _color, Color _notPressedColor)
        : base(_topLeftPoint, _size, _buttonText, _buttonTexture, _standardFont, _color)
    {
        notPressedColor = _notPressedColor;
        pressedColor = _color;

        IsPressed = false;
        color = _notPressedColor;
    }
    /// <summary>
    /// If the button is pressed the boolean isPressed is switched.
    /// </summary>
    protected override void Pressed()
    {
        IsPressed = !IsPressed;

        if (IsPressed)
            color = pressedColor;

        else
            color = notPressedColor;
        base.Pressed();
    }
}
