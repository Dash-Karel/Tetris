

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisTemplate
{   

    internal class MainMenu
    {
        Texture2D buttonTexture;
        SpriteFont standardFont;

        string normalTex, monkeyTex, gameTitelTex;
        Vector2 buttonSize;
        Vector2 normalPos, monkeyPos, gameTitelPos;
        Button normalBut, monkeyBut;
        public MainMenu(SpriteFont _standardFont) 
        { 
            standardFont = _standardFont;
            buttonTexture = TetrisGame.ContentManager.Load<Texture2D>("Button");
            gameTitelTex = "MONKEY  TETRIS";

            //Calculating and setting button parameters
            //The position of the ui elements are based on fractions of the total screen lenght and screen height
            buttonSize = new Vector2(buttonTexture.Width, buttonTexture.Height);

            //Button positions
            normalPos = new Vector2(TetrisGame.ScreenSize.X / 3, TetrisGame.ScreenSize.Y / 5 * 2) - buttonSize / 2;
            monkeyPos = new Vector2(TetrisGame.ScreenSize.X / 3 * 2, TetrisGame.ScreenSize.Y / 5 * 2) - buttonSize / 2;
            
            gameTitelPos = new Vector2(TetrisGame.ScreenSize.X / 2, TetrisGame.ScreenSize.Y / 5) - standardFont.MeasureString(gameTitelTex) / 2;

            //Button text's
            normalTex = "Play  Normal";
            monkeyTex = "Play  Monkey  Mode";

            //Initializing Buttons
            normalBut = new Button(normalPos, buttonSize, normalTex, buttonTexture, standardFont, Color.Green);
            monkeyBut = new Button(monkeyPos, buttonSize, monkeyTex, buttonTexture, standardFont, Color.Green);

            normalBut.buttonPressed += NormalPressed;
            monkeyBut.buttonPressed += MonkeyPressed;
        }
        public void Update()
        {
            //Updating the buttons
            normalBut.Update();
            monkeyBut.Update();
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Drawing the Text and Buttons
            _spriteBatch.DrawString(standardFont, gameTitelTex, gameTitelPos, Color.Blue);
            normalBut.Draw(_spriteBatch);
            monkeyBut.Draw(_spriteBatch);
        }
        void MonkeyPressed()
        {

        }
        void NormalPressed()
        {
            TetrisGame.GameWorld.StartNormalGame();
        }
    }
}
