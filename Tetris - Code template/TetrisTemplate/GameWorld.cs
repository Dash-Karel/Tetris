using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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

    Song themeSong;
    SoundEffect gameOverSound;

    float secondsUntilNextTick;
    float secondsPerTick = 1;

    int score;
    int level;

    double leftHeldForSeconds;
    double rightHeldForSeconds;
    double downHeldForSeconds;

    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;

        themeSong = TetrisGame.ContentManager.Load<Song>("TetrisTheme");
        gameOverSound = TetrisGame.ContentManager.Load<SoundEffect>("gameOverSound");
        MediaPlayer.IsRepeating = true;

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
        //updating timers for auto repeat

        double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
        if (inputHelper.KeyDown(Keys.Left))
            leftHeldForSeconds += elapsedSeconds;
        else
            leftHeldForSeconds = 0f;

        if (inputHelper.KeyDown(Keys.Right))
            rightHeldForSeconds += elapsedSeconds;
        else
            rightHeldForSeconds = 0f;

        //updating timer for soft dropping
        if(inputHelper.KeyDown(Keys.Down))
        {
            downHeldForSeconds += elapsedSeconds;
        }
        else
        {
            downHeldForSeconds = 0f;
        }

        //auto repeat: moving the blocks if a direction key is held for more than 0.3s and the other direction key is not pressed. It repeats every 0.625 seconds
        if (leftHeldForSeconds > 0.3f && rightHeldForSeconds == 0f)
        {
            leftHeldForSeconds = 0.2375f;
            block.MoveLeft();
        }
        else if (rightHeldForSeconds > 0.3f && leftHeldForSeconds == 0f)
        {
            rightHeldForSeconds = 0.2375f;
            block.MoveRight();
        }
        else
        {
            //code for regular horizontal movement, only if auto repeat is not occuring
            if (inputHelper.KeyPressed(Keys.Left))
                block.MoveLeft();

            if (inputHelper.KeyPressed(Keys.Right))
                block.MoveRight();
        }

        //code for soft dropping
        if (downHeldForSeconds > secondsPerTick/10)
        {
            block.MoveDown();
        }

        //code for hard dropping
        if (inputHelper.KeyPressed(Keys.Space))
            block.HardDrop();

        if (inputHelper.KeyPressed(Keys.D))
            block.RotateRight();

        if (inputHelper.KeyPressed(Keys.A))
            block.RotateLeft();
    }

    public void Update(GameTime gameTime, InputHelper inputHelper)
    {
        //ensure music keeps playing
        if (MediaPlayer.State == MediaState.Stopped)
            MediaPlayer.Play(themeSong);
        if (MediaPlayer.State == MediaState.Paused)
            MediaPlayer.Resume();

        if (gameState == GameState.Playing)
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
        gameOverSound.Play();
        gameState = GameState.GameOver;
    }

    public void Reset()
    {
        gameState = GameState.Playing;

        secondsUntilNextTick = secondsPerTick;
        secondsPerTick = 1;

        score = 0;
        level = 1;

        grid.Clear();

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

        if(score >=  500 * (level * (level + 1)))
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
