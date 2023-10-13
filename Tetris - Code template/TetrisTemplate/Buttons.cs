using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public delegate void Notify();
internal class Button
{
    public event Notify buttonPressed;

    MouseState mouse, previousMouse;
    Vector2 topLeftPosition, textPosition, size;
    string buttonText;
    Texture2D buttonTexture;
    Rectangle mouseDetector;
    SpriteFont standardFont;
    SoundEffect clickSound;
    protected Color color;
    float colorHoverdFactor;

    public Button(Vector2 _topLeftPoint, Vector2 _size, string _buttonText, Texture2D _buttonTexture, SpriteFont _standardFont, Color _color)
    {

        //Loading and setting standard variables
        buttonText = _buttonText;
        buttonTexture = _buttonTexture;
        standardFont = _standardFont;
        color = _color;
        size = _size;

        clickSound = TetrisGame.ContentManager.Load<SoundEffect>("buttonClickSound");

        topLeftPosition = _topLeftPoint;
        mouseDetector = new Rectangle(_topLeftPoint.ToPoint(), _size.ToPoint());
        //Centering the text in the rectangle.
        textPosition = mouseDetector.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2; 
        colorHoverdFactor = 0.5f;
    }
    public void Update()
    {
        //Detecting if the mouse is with in the button and if its pressed.
        previousMouse = mouse;
        mouse = Mouse.GetState();
        if (mouseDetector.Contains(mouse.Position))
        {
            Hovered();
                
            //Check if the left button is being clicked and making sure that the button does not switch when the left button is held.
            if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed)
                Pressed();

        }
        else
        {
            NotHovered();
        }
            
    }
    public void Draw(SpriteBatch _spriteBatch)
    {
        //Drawing the text and the button
        _spriteBatch.Draw(buttonTexture, topLeftPosition, color * colorHoverdFactor);
        _spriteBatch.DrawString(standardFont, buttonText, textPosition, Color.Black);
    }

    //This method updates the position for when the screensize is changed.
    public void UpdatePosition(Vector2 position)
    {
        topLeftPosition = position;
        mouseDetector = new Rectangle(topLeftPosition.ToPoint(), size.ToPoint());
        textPosition = mouseDetector.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2;

    }
    protected void Hovered()
    {
        // if the button is hovered by the mouse the button color is the full brightnes.
        colorHoverdFactor = 1.0f;
    }
    protected void NotHovered()
    {
        //when the button is not hovered the color brightnes is reduced.
        colorHoverdFactor = 0.5f;
    }
    protected virtual void Pressed()
    {
        //Call the event button pressed.

        buttonPressed?.Invoke();
    }
}

