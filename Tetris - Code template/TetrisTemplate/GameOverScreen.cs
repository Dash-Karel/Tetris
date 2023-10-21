using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisTemplate
{
    internal class GameOverScreen
    {
        TetrisGame game;

        Texture2D buttonTexture, darkeningLayer;
        SpriteFont standardFont;

        Vector2 mainMenuPos, replayPos, gameOverPos;
        Vector2 buttonSize;
        Button mainMenuBut, replayBut;
        string gameOverText, standardGameOverText;
        string mainMenuText, replayText;
        public GameOverScreen(SpriteFont _standardFont, TetrisGame game) 
        {
            this.game = game;

            standardFont = _standardFont;
            buttonTexture = TetrisGame.ContentManager.Load<Texture2D>("Button");
            darkeningLayer = TetrisGame.ContentManager.Load<Texture2D>("blankBackground");
            standardGameOverText = "GAMEOVER";
            gameOverText = standardGameOverText;

            //Calculating and setting button parameters
            //The position of the ui elements are based on fractions of the total screen lenght and screen height
            buttonSize = new Vector2(buttonTexture.Width, buttonTexture.Height);

            //Button text's
            mainMenuText = "Main menu";
            replayText = "Play again";

            //Button positions
            mainMenuPos = new Vector2(TetrisGame.WorldSize.X / 3, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            replayPos = new Vector2(TetrisGame.WorldSize.X / 3 * 2, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            gameOverPos = new Vector2(TetrisGame.WorldSize.X / 2, TetrisGame.WorldSize.Y / 5) - standardFont.MeasureString(gameOverText) / 2;

            //Initializing Buttons
            mainMenuBut = new Button(mainMenuPos, buttonSize, mainMenuText, buttonTexture, standardFont, Color.Green);
            replayBut = new Button(replayPos, buttonSize, replayText, buttonTexture, standardFont, Color.Green);

            //Setting the button pressed events
            replayBut.buttonPressed += ReplayPressed;
            mainMenuBut.buttonPressed += MainMenuPressed;
        }
        public void Update(InputHelper inputHelper)
        {
            //Updating the buttons
            mainMenuBut.Update(inputHelper);
            replayBut.Update(inputHelper);
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Drawing the Text and Buttons
            _spriteBatch.Draw(darkeningLayer, Vector2.Zero, Color.Black * 0.7f);
            _spriteBatch.DrawString(standardFont, gameOverText, gameOverPos, Color.White);
            mainMenuBut.Draw(_spriteBatch);
            replayBut.Draw(_spriteBatch);
        }
        public void ApplyResolutionSettings()
        {
            //Button positions
            mainMenuPos = new Vector2(TetrisGame.WorldSize.X / 3, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            replayPos = new Vector2(TetrisGame.WorldSize.X / 3 * 2, TetrisGame.WorldSize.Y / 5 * 2) - buttonSize / 2;
            gameOverPos = new Vector2(TetrisGame.WorldSize.X / 2, TetrisGame.WorldSize.Y / 5) - standardFont.MeasureString(gameOverText) / 2;

            mainMenuBut.UpdatePosition(mainMenuPos);
            replayBut.UpdatePosition(replayPos);
        }
        public void ChangeGameOverText(string Text)
        {
            gameOverText = Text;
            gameOverPos = new Vector2(TetrisGame.WorldSize.X / 2, TetrisGame.WorldSize.Y / 5) - standardFont.MeasureString(gameOverText) / 2;
        }

        void Reset()
        {
            gameOverText = standardGameOverText;
            gameOverPos = new Vector2(TetrisGame.WorldSize.X / 2, TetrisGame.WorldSize.Y / 5) - standardFont.MeasureString(gameOverText) / 2;
        }

        void ReplayPressed()
        {
            game.Reset();
            Reset();
        }
        void MainMenuPressed()
        {
            game.ReturnToMainMenu();
            Reset();
        }
    }
}
