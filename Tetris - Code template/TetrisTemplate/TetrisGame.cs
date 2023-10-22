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

    //boolean indicates whether the game is in singlePlayer or multiplayer mode
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

    Texture2D background, backgroundTargetShape;
    Texture2D background2Player, background2PlayerTargetShape;

    /// <summary>
    /// A static reference to the ContentManager object, used for loading assets.
    /// </summary>
    public static ContentManager ContentManager { get; private set; }

    /// <summary>
    /// A static reference to the EffectsManager object, used for fisplaying effects.
    /// </summary>
    public static EffectsManager EffectsManager{ get; private set; }

    /// <summary>
    /// Whether the game should use special blocks or not
    /// </summary>
    public static bool UseSpecialBlocks { get; set; }


    /// <summary>
    /// Whether the game should indicate a target shape or not
    /// </summary>
    public static bool UseTargetShape { get;  set; }

    /// <summary>
    /// A static reference to the width and height of the screen.
    /// </summary>
    public static Point WorldSize { get; private set; }

    /// <summary>
    /// Whether the game is in full screen or not, updates resolution settings on set
    /// </summary>
    bool FullScreen
    {
        get { return graphics.IsFullScreen; }
        set { ApplyResolutionSettings(value); }
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
        gameState = GameState.MainMenu;
        IsMouseVisible = true;

        spriteBatch = new SpriteBatch(GraphicsDevice);

        font = ContentManager.Load<SpriteFont>("SpelFont");

        background = ContentManager.Load<Texture2D>("background");
        background2Player = ContentManager.Load<Texture2D>("backgroundTwoPlayers");
        backgroundTargetShape = ContentManager.Load<Texture2D>("backgroundTargetShape");
        background2PlayerTargetShape = ContentManager.Load<Texture2D>("backgroundTwoPlayersTargetShape");

        gameWorldPlayer1 = new GameWorld(this, font, true);
        gameWorldPlayer2 = new GameWorld(this, font, false, Keys.Left, Keys.I, Keys.Right, Keys.P, Keys.Down, Keys.RightControl, Keys.O);


        gameWorldPlayer1.Reset();
        gameWorldPlayer2.Reset();

        mediaPlayer = new MediaPlayer();

        EffectsManager = new EffectsManager();

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
        //fullscreen controls
        if (inputHelper.KeyPressed(Keys.F5))
            FullScreen = !FullScreen;

        //media controls
        if(inputHelper.KeyPressed(Keys.T))
            mediaPlayer.TogglePlaying();
        if (inputHelper.KeyPressed(Keys.Y))
            mediaPlayer.Previous();
        if (inputHelper.KeyPressed(Keys.U))
            mediaPlayer.Skip();

        //update general objects
        mediaPlayer.Update(gameTime);
        inputHelper.Update(gameTime);

        //Update different menus and gameworlds based on the gameState
        switch (gameState)
        {
            case GameState.MainMenu:
                mainMenu.Update(inputHelper);
                break;
            case GameState.Playing:
                gameWorldPlayer1.Update(gameTime, inputHelper);
                if(is2Player)
                    gameWorldPlayer2.Update(gameTime, inputHelper);
                EffectsManager.Update(gameTime);
                break;
            case GameState.GameOver:
                gameOverScreen.Update(inputHelper);
                break;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGray);
        //Draws everything scaled from the worldSize to the screen size
        spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, spriteScale);

        //draws menus and gameworlds based on the game state
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
    /// <summary>
    /// Draws everything related to the playing field(s)
    /// </summary>
    /// <param name="gameTime"></param>
    void DrawPlaying(GameTime gameTime)
    {
        //picks a background based on the diffent game modes
        Texture2D currentBackround;
        if (is2Player)
            if (UseTargetShape)
                currentBackround = background2PlayerTargetShape;
            else
                currentBackround = background2Player;
        else
            if(UseTargetShape)
                currentBackround = backgroundTargetShape;
            else
                currentBackround = background;
        //draws the background
        spriteBatch.Draw(currentBackround, Vector2.Zero, Color.White);

        //draws the gameworld(s)
        gameWorldPlayer1.Draw(gameTime, spriteBatch);
        if(is2Player)
            gameWorldPlayer2.Draw(gameTime, spriteBatch);

        //lets effectsManager draw all the effects on top of the worlds
        EffectsManager.Draw(spriteBatch);
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
        EffectsManager.Reset();
    }
    public void StartNormalGame()
    {
        Reset();
    }
    public void StartTwoPlayerGame()
    {
        Reset();
        if (!is2Player)
        {
            Switch2PlayerMode();
        }
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

    /// <summary>
    /// Toggles multiplayer mode and makes sure all positions of objects are updated accordingly
    /// </summary>
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

    //Syncs the level of both players, so both their games are just as fast
    public void SyncLevel(int level)
    {
        gameWorldPlayer1.setLevel(level);
        if(is2Player)
            gameWorldPlayer2.setLevel(level);
    }

    /// <summary>
    /// Sends a line to the other player, this moves the entire grid of that player one cell up
    /// </summary>
    /// <param name="playerWhoSentIsPlayerOne">true when player two should receive a line false when player one should receive a line</param>
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

