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
            if (!block.MoveDown())
                NewBlocks();
        }

        if (inputHelper.KeyPressed(Keys.Space))
        {
            block.HardDrop();
            NewBlocks();
        }
    }

    public void Update(GameTime gameTime)
    {
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
}
