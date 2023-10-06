using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TetrisTemplate
{
    internal class ToggleButton : Button
    {
        bool isPressed = false;
        public bool IsPressed { get { return isPressed; } }

        Color notPressedColor, pressedColor;
        public ToggleButton(Vector2 _topLeftPoint, Vector2 _size, string _buttonText, Texture2D _buttonTexture, SpriteFont _standardFont, Color _color, Color _notPressedColor) 
            : base(_topLeftPoint, _size, _buttonText, _buttonTexture, _standardFont, _color)
        { 
            notPressedColor = _notPressedColor;
            pressedColor = _color;
        }
        protected override void Pressed()
        {
            base.Pressed();
            isPressed = !isPressed;

            if (isPressed)
                color = pressedColor;

            else
                color = notPressedColor;
        }
    }
}
