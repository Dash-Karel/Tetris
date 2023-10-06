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

    GameOverScreen gameOverScreen;

    float secondsUntilNextTick;
    float secondsPerTick = 1;

    int score;
    int level;


    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;

        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");

        grid = new TetrisGrid();

        bag = new RandomBag();

        gameOverScreen = new GameOverScreen(font);

        secondsUntilNextTick = secondsPerTick;
        secondsPerTick = 1;

        score = 0;
        level = 1;

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
            block.MoveDown();
        }

        if (inputHelper.KeyPressed(Keys.Space))
        {
            block.HardDrop();
        }
    }

    public void Update(GameTime gameTime, InputHelper inputHelper)
    {
        if(gameState == GameState.Playing)
        {
            UpdateTickTime(gameTime);
            HandleInput(gameTime, inputHelper);
        }
        else
        {
            gameOverScreen.Update();
        } 
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        grid.Draw(gameTime, spriteBatch);
        block.Draw(spriteBatch);
        previewBlock.Draw(spriteBatch);
        spriteBatch.DrawString(font, "Level: " + level.ToString(), Vector2.Zero, Color.White);
        spriteBatch.DrawString(font, "Score: " + score.ToString(), new Vector2(0, font.LineSpacing), Color.White);
        if (gameState == GameState.GameOver)
        {
            gameOverScreen.Draw(spriteBatch);
        }
        spriteBatch.End();
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
    }

    public void Reset()
    {
        gameState = GameState.Playing;

        secondsUntilNextTick = secondsPerTick;
        secondsPerTick = 1;

        score = 0;
        level = 1;

        previewBlock = bag.NextBlock(grid);
        NewBlocks();
    }
    public void NewBlocks()
    {
        secondsUntilNextTick = secondsPerTick;
        block = previewBlock;
        block.MoveToSpawnPosition();
        previewBlock = bag.NextBlock(grid);
    }
    public void IncreaseScore(int LinesCleared)
    {
        if (LinesCleared == 4)
            score += 800 * level;
        else
            score += (2 * LinesCleared - 1) * 100 * level;

        if(score >  500 * (level * (level + 1)))
        {
            IncreaseLevel();
        }
    }
    void IncreaseLevel()
    {
        level++;
        secondsPerTick = MathF.Pow((0.8f - ((level - 1) * 0.007f)), level - 1);
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
        block.MoveDown();

        //Resetting the tick timer.
        secondsUntilNextTick = secondsPerTick;
    }


}
