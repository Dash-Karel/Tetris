

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisTemplate
{   

    internal class MainMenu
    {
        TetrisGame game;

        Texture2D buttonTexture;
        SpriteFont standardFont;

        string normalText, twoPlayerText, gameTitelText, infoText;
        Vector2 buttonSize;
        Vector2 normalPos, twoPlayerPos, gameTitelPos, infoPos;
        Button normalBut, twoPlayerBut;
        public MainMenu(SpriteFont _standardFont, TetrisGame game) 
        {
            this.game = game;

            standardFont = _standardFont;
            buttonTexture = TetrisGame.ContentManager.Load<Texture2D>("Button");
            gameTitelText = "TETRIS";

            //Calculating and setting button parameters
            //The position of the ui elements are based on fractions of the total screen lenght and screen height
            buttonSize = new Vector2(buttonTexture.Width, buttonTexture.Height);

            //Button text's
            normalText = "Normal";
            twoPlayerText = "Two Players";
            infoText = "Press F5 to toggle fullscreen";

            //Button positions
            normalPos = new Vector2(TetrisGame.WorldSize.X / 3, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            twoPlayerPos = new Vector2(TetrisGame.WorldSize.X / 3 * 2, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            gameTitelPos = new Vector2(TetrisGame.WorldSize.X / 2, TetrisGame.WorldSize.Y / 5) - standardFont.MeasureString(gameTitelText) / 2;
            infoPos = Vector2.Zero;

            //Initializing Buttons
            normalBut = new Button(normalPos, buttonSize, normalText, buttonTexture, standardFont, Color.Green);
            twoPlayerBut = new Button(twoPlayerPos, buttonSize, twoPlayerText, buttonTexture, standardFont, Color.Green);

            normalBut.buttonPressed += NormalPressed;
            twoPlayerBut.buttonPressed += TwoPlayerPressed;
        }
        public void Update(InputHelper inputHelper)
        {
            //Updating the buttons
            normalBut.Update(inputHelper);
            twoPlayerBut.Update(inputHelper);
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Drawing the Text and Buttons
            _spriteBatch.DrawString(standardFont, gameTitelText, gameTitelPos, Color.Blue);
            _spriteBatch.DrawString(standardFont, infoText, infoPos, Color.Gray);
            normalBut.Draw(_spriteBatch);
            twoPlayerBut.Draw(_spriteBatch);
        }
        public void ApplyResolutionSettings()
        {
            //Button positions
            normalPos = new Vector2(TetrisGame.WorldSize.X / 3, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            twoPlayerPos = new Vector2(TetrisGame.WorldSize.X / 3 * 2, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            gameTitelPos = new Vector2(TetrisGame.WorldSize.X / 2, TetrisGame.WorldSize.Y / 5) - standardFont.MeasureString(gameTitelText) / 2;

            normalBut.UpdatePosition(normalPos);
            twoPlayerBut.UpdatePosition(twoPlayerPos);
        }
        void TwoPlayerPressed()
        {
            game.StartTwoPlayerGame();
        }
        void NormalPressed()
        {
            game.StartNormalGame();
        }
    }
}
