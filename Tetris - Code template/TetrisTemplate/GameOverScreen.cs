using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisTemplate
{
    internal class GameOverScreen
    {
        Texture2D buttonTexture;
        SpriteFont standardFont;

        Vector2 mainMenuPos, replayPos, gameOverPos;
        Vector2 buttonSize;
        Button mainMenuBut, replayBut;
        public string gameOverText;
        string mainMenuText, replayText;
        public GameOverScreen(SpriteFont _standardFont) 
        {
            standardFont = _standardFont;
            buttonTexture = TetrisGame.ContentManager.Load<Texture2D>("Button");
            gameOverText = "Game over";

            //Calculating and setting button parameters
            //The position of the ui elements are based on fractions of the total screen lenght and screen height
            buttonSize = new Vector2(buttonTexture.Width, buttonTexture.Height);

            //Button positions
            mainMenuPos = new Vector2(TetrisGame.ScreenSize.X / 3, TetrisGame.ScreenSize.Y / 5 * 2) - buttonSize / 2;
            replayPos = new Vector2(TetrisGame.ScreenSize.X / 3 * 2, TetrisGame.ScreenSize.Y / 5 * 2) - buttonSize / 2;
            //
            gameOverPos = new Vector2(TetrisGame.ScreenSize.X / 2, TetrisGame.ScreenSize.Y / 5) - standardFont.MeasureString(gameOverText) / 2;

            //Button text's
            mainMenuText = "Main menu";
            replayText = "Play again";

            //Initializing Buttons
            mainMenuBut = new Button(mainMenuPos, buttonSize, mainMenuText, buttonTexture, standardFont, Color.Green);
            replayBut = new Button(replayPos, buttonSize, replayText, buttonTexture, standardFont, Color.Green);

            //Setting the button pressed events
            replayBut.buttonPressed += ReplayPressed;
            mainMenuBut.buttonPressed += MainMenuPressed;
        }
        public void Update()
        {
            //Updating the buttons
            mainMenuBut.Update();
            replayBut.Update();
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Drawing the Text and Buttons
            _spriteBatch.DrawString(standardFont, gameOverText, gameOverPos, Color.Blue);
            mainMenuBut.Draw(_spriteBatch);
            replayBut.Draw(_spriteBatch);
        }
        void ReplayPressed()
        {

        }
        void MainMenuPressed()
        {

        }
    }
}
