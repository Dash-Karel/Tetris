using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TetrisTemplate
{/// <summary>
/// ToggleButton is a child class of the button and is curently not in use. 
/// The toggle button is used when i button can either be on or off.
/// </summary>
    internal class ToggleButton : Button
    {
        bool isPressed = false;
        public bool IsPressed { get { return isPressed; } }

        Color notPressedColor, pressedColor;
        //constructor
        public ToggleButton(Vector2 _topLeftPoint, Vector2 _size, string _buttonText, Texture2D _buttonTexture, SpriteFont _standardFont, Color _color, Color _notPressedColor) 
            : base(_topLeftPoint, _size, _buttonText, _buttonTexture, _standardFont, _color)
        { 
            notPressedColor = _notPressedColor;
            pressedColor = _color;
        }
        /// <summary>
        /// If the button is pressed the boolen ispressed is switched.
        /// </summary>
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
