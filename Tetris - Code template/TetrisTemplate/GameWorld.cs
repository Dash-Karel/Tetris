using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TetrisTemplate;

/// <summary>
/// A class for representing the game world.
/// This contains the grid, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
{
    /// <summary>
    /// An enum for the different game states that the game can have.
    /// </summary>
    enum GameState
    {
        Playing,
        GameOver
    }

    /// <summary>
    /// The random-number generator of the game.
    /// </summary>
    public static Random Random { get { return random; } }
    static Random random;

    /// <summary>
    /// The main font of the game.
    /// </summary>
    SpriteFont font;

    /// <summary>
    /// The current game state.
    /// </summary>
    GameState gameState;

    /// <summary>
    /// The main grid of the game.
    /// </summary>
    TetrisGrid grid;

    Block block;
    Block previewBlock;

    RandomBag bag;

    float secondsUntilNextTick = 1;
    float secondsPerTick = 1;


    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;

        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");

        grid = new TetrisGrid();

        bag = new RandomBag();

        previewBlock = bag.NextBlock(grid);
        NewBlocks();
    }

    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        if(inputHelper.KeyPressed(Keys.D))
            block.RotateRight();

        if(inputHelper.KeyPressed(Keys.A))
            block.RotateLeft();

        if(inputHelper.KeyPressed(Keys.Left))
            block.MoveLeft();

        if(inputHelper.KeyPressed(Keys.Right))
            block.MoveRight();

        if (inputHelper.KeyPressed(Keys.Down))
        {
            BlockMoveDown();
        }

        if (inputHelper.KeyPressed(Keys.Space))
        {
            block.HardDrop();
            NewBlocks();
        }
    }

    public void Update(GameTime gameTime)
    {
        UpdateTickTime(gameTime);

    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        grid.Draw(gameTime, spriteBatch);
        block.Draw(spriteBatch);
        previewBlock.Draw(spriteBatch);
        spriteBatch.DrawString(font, "Hello!", Vector2.Zero, Color.Blue);
        spriteBatch.End();
    }

    public void Reset()
    {
    }
    void NewBlocks()
    {
        block = previewBlock;
        block.MoveToSpawnPosition();
        previewBlock = bag.NextBlock(grid);
    }
    void UpdateTickTime(GameTime gameTime)
    {
        //Subtracting the time since the next frame from the seconds until the next tick.
        secondsUntilNextTick -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        //Checking if its time to execute a tick
        if (secondsUntilNextTick < 0)
        {
            //Calling the Execute tick function.
            ExecuteTick();
        }
    }
    void ExecuteTick() 
    {
        //Moving the current block down.
        BlockMoveDown();

        //Resetting the tick timer.
        secondsUntilNextTick = secondsPerTick;
    }

    void BlockMoveDown() 
    {
        //Moving the current block down and switch to a new block if it hits the bottem
        if (!block.MoveDown())
        {
            NewBlocks();
        }
    }
}
