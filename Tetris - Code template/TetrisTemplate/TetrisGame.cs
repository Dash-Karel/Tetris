using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

class TetrisGame : Game
{
    /// <summary>
    /// An enum for the different game states that the game can have.
    /// </summary>
    enum GameState
    {
        Playing,
        GameOver,
        MainMenu
    }

    GameState gameState;
    GameState lastGameState;

    bool is2Player = false;

    SpriteBatch spriteBatch;
    InputHelper inputHelper;
    GraphicsDeviceManager graphics;

    Point windowSize;
    Matrix spriteScale;

    GameWorld gameWorldPlayer1;
    GameWorld gameWorldPlayer2;
    

    MediaPlayer mediaPlayer;

    SoundEffect themeSong, shittySong;

    GameOverScreen gameOverScreen;
    MainMenu mainMenu;

    SpriteFont font;

    Texture2D background;
    Texture2D background2Player;
    /// <summary>
    /// A static reference to the ContentManager object, used for loading assets.
    /// </summary>
    public static ContentManager ContentManager { get; private set; }
    public static bool UseSpecialBlocks { get; set; }


    /// <summary>
    /// A static reference to the width and height of the screen.
    /// </summary>
    public static Point WorldSize { get; private set; }

    bool FullScreen
    {
        get { return graphics.IsFullScreen; }
        set { ApplyResolutionSettings(value); }
    }
    public void SetMouseVisible(bool isVisible)
    {
        IsMouseVisible = isVisible;
    }

    [STAThread]
    static void Main(string[] args)
    {
        TetrisGame game = new TetrisGame();
        game.Run();
    }

    public TetrisGame()
    {
        // initialize the graphics device
        graphics = new GraphicsDeviceManager(this);

        // store a static reference to the content manager, so other objects can use it
        ContentManager = Content;

        // set the directory where game assets are located
        Content.RootDirectory = "Content";

        // create the input helper object
        inputHelper = new InputHelper(this);
    }

    protected override void LoadContent()
    {
        UseSpecialBlocks = true;

        gameState = GameState.MainMenu;
        IsMouseVisible = true;

        spriteBatch = new SpriteBatch(GraphicsDevice);

        font = ContentManager.Load<SpriteFont>("SpelFont");

        background = ContentManager.Load<Texture2D>("background");
        background2Player = ContentManager.Load<Texture2D>("backgroundTwoPlayers");

        gameWorldPlayer1 = new GameWorld(this, font, true);
        gameWorldPlayer2 = new GameWorld(this, font, false, Keys.NumPad1, Keys.OemOpenBrackets, Keys.NumPad3, Keys.OemBackslash, Keys.NumPad2, Keys.RightShift, Keys.OemCloseBrackets);

        gameWorldPlayer1.Reset();
        gameWorldPlayer2.Reset();

        mediaPlayer = new MediaPlayer();

        themeSong = ContentManager.Load<SoundEffect>("TetrisTheme");
        shittySong = ContentManager.Load<SoundEffect>("shittyMusic");

        mediaPlayer.AddSongToQueue(themeSong);
        mediaPlayer.AddSongToQueue(shittySong);

        gameOverScreen = new GameOverScreen(font, this);
        mainMenu = new MainMenu(font, this);

        WorldSize = new Point(background.Width, background.Height);
        windowSize = WorldSize;
        FullScreen = false;
    }

    protected override void Update(GameTime gameTime)
    {
        if (inputHelper.KeyPressed(Keys.F5))
            FullScreen = !FullScreen;

        mediaPlayer.Update();
        inputHelper.Update(gameTime);

        switch (gameState)
        {
            case GameState.MainMenu:
                lastGameState = gameState;
                mainMenu.Update(inputHelper);
                break;
            case GameState.Playing:
                if (gameState != lastGameState)
                    Reset();
                gameWorldPlayer1.Update(gameTime, inputHelper);
                if(is2Player)
                    gameWorldPlayer2.Update(gameTime, inputHelper);
                lastGameState = gameState;
                break;
            case GameState.GameOver:
                lastGameState = gameState;
                gameOverScreen.Update(inputHelper);
                break;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGray);
        spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, spriteScale);
        switch (gameState)
        {
            case GameState.MainMenu:
                mainMenu.Draw(spriteBatch);
                break;
            case GameState.Playing:
                DrawPlaying(gameTime);
                break;
            case GameState.GameOver:
                DrawPlaying(gameTime);
                gameOverScreen.Draw(spriteBatch);
                break;
        }
        spriteBatch.End();
    }
    void DrawPlaying(GameTime gameTime)
    {
        spriteBatch.Draw(background, Vector2.Zero, Color.White);
        if (is2Player)
        {
            spriteBatch.Draw(background2Player, Vector2.Zero, Color.White);
            gameWorldPlayer2.Draw(gameTime, spriteBatch);
        }
        gameWorldPlayer1.Draw(gameTime, spriteBatch);
    }

    public void GameOver(bool gameOverCallerIsPlayerOne)
    {
        IsMouseVisible = true;
        gameState = GameState.GameOver;
        if (is2Player)
        {
            if (gameOverCallerIsPlayerOne)
                gameOverScreen.ChangeGameOverText("Player Two Wins!");
            else
                gameOverScreen.ChangeGameOverText("Player One Wins!");

        }
    }
    public void Reset()
    {
        IsMouseVisible = false;
        gameState = GameState.Playing;
        gameWorldPlayer1.Reset();
        gameWorldPlayer2.Reset();
    }
    public void StartNormalGame()
    {
        gameState = GameState.Playing;
    }
    public void StartTwoPlayerGame()
    {
        if (!is2Player)
        {
            Switch2PlayerMode();
        }
        gameState = GameState.Playing;
    }
    public void ReturnToMainMenu()
    {
        if(is2Player)
            Switch2PlayerMode();
        gameState = GameState.MainMenu;
    }

    //----------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------
    //TWO PLAYER RELATED METHODS

    void Switch2PlayerMode()
    {
        is2Player = !is2Player;

        if (is2Player)
        {
            WorldSize = new Point(background2Player.Width, background2Player.Height);
            windowSize = WorldSize;
            ApplyResolutionSettings(FullScreen);
            gameWorldPlayer1.OffsetWorld(new Vector2(-WorldSize.X / 4, 0));
            gameWorldPlayer2.OffsetWorld(new Vector2(WorldSize.X / 4, 0));
            gameWorldPlayer1.ChangeKeyBindings(Keys.C, Keys.A, Keys.B, Keys.D, Keys.V, Keys.S, Keys.LeftShift);
        }
        else
        {
            gameWorldPlayer1.OffsetWorld(Vector2.Zero);
            WorldSize = new Point(background.Width, background.Height);
            windowSize = WorldSize;
            ApplyResolutionSettings(FullScreen);
            gameWorldPlayer1.ChangeKeyBindings(Keys.Left, Keys.A, Keys.Right, Keys.D, Keys.Down, Keys.Space, Keys.LeftShift);
        }
    }

    public void SyncLevel(int level)
    {
        gameWorldPlayer1.setLevel(level);
        if(is2Player)
            gameWorldPlayer2.setLevel(level);
    }
    public void SendLine(bool playerWhoSentIsPlayerOne)
    {
        if (is2Player)
        {
            if (playerWhoSentIsPlayerOne)
                gameWorldPlayer2.ReceiveLine();
            else
                gameWorldPlayer1.ReceiveLine();
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------
    //SCREEN SIZE RELATED METHODS

    Viewport CalculateViewport(Point windowSize)
    {
        Viewport viewport = new Viewport();
        float gameAspectRatio = (float)WorldSize.X / WorldSize.Y;
        float windowAspectRatio = (float)windowSize.X / windowSize.Y;

        if (windowAspectRatio > gameAspectRatio)
        {
            viewport.Width = (int)(windowSize.Y * gameAspectRatio);
            viewport.Height = windowSize.Y;
        }
        else
        {
            viewport.Width = windowSize.X;
            viewport.Height = (int)(windowSize.X / gameAspectRatio);
        }

        viewport.X = (windowSize.X - viewport.Width) / 2;
        viewport.Y = (windowSize.Y - viewport.Height) / 2;

        return viewport;
    }

    void ApplyResolutionSettings(bool fullScreen)
    {
        Point screenSize;

        if (fullScreen)
            screenSize = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        else
            screenSize = windowSize;

        graphics.IsFullScreen = fullScreen;
        graphics.PreferredBackBufferWidth = screenSize.X;
        graphics.PreferredBackBufferHeight = screenSize.Y;
        graphics.ApplyChanges();

        GraphicsDevice.Viewport = CalculateViewport(screenSize);
        spriteScale = Matrix.CreateScale((float)GraphicsDevice.Viewport.Width / WorldSize.X, (float)GraphicsDevice.Viewport.Height / WorldSize.Y, 1);

        gameWorldPlayer1.ApplyResolutionSettings();
        gameWorldPlayer2.ApplyResolutionSettings();
        mainMenu.ApplyResolutionSettings();
        gameOverScreen.ApplyResolutionSettings();

    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        Vector2 viewportTopLeft = new Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y);

        float screenToWorldScale = WorldSize.X / (float)GraphicsDevice.Viewport.Width;

        return (screenPosition - viewportTopLeft) * screenToWorldScale;
    }
}

