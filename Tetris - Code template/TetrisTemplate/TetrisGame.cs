using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

class TetrisGame : Game
{
    SpriteBatch spriteBatch;
    InputHelper inputHelper;
    GraphicsDeviceManager graphics;

    Point windowSize;
    Matrix spriteScale;

    static GameWorld gameWorld;


    /// <summary>
    /// A static reference to the ContentManager object, used for loading assets.
    /// </summary>
    public static ContentManager ContentManager { get; private set; }


    /// <summary>
    /// A static reference to the width and height of the screen.
    /// </summary>
    public static Point WorldSize { get; private set; }

    bool FullScreen
    {
        get { return graphics.IsFullScreen; }
        set { ApplyResolutionSettings(value); }
    }
    public static GameWorld GameWorld
    {
        get { return gameWorld; }
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
        spriteBatch = new SpriteBatch(GraphicsDevice);

        WorldSize = new Point(800, 620);
        windowSize = new Point(800, 620);
        FullScreen = false;

        // create and reset the game world
        gameWorld = new GameWorld(this);
        gameWorld.Reset();
    }

    protected override void Update(GameTime gameTime)
    {
        inputHelper.Update(gameTime);
        gameWorld.Update(gameTime, inputHelper);

        if (inputHelper.KeyPressed(Keys.F5))
            FullScreen = !FullScreen;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGray);
        gameWorld.Draw(gameTime, spriteBatch, spriteScale);
    }

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
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        Vector2 viewportTopLeft = new Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y);

        float screenToWorldScale = WorldSize.X / (float)GraphicsDevice.Viewport.Width;

        return (screenPosition - viewportTopLeft) * screenToWorldScale;
    }
}

