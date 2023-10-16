using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using TetrisTemplate;

/// <summary>
/// A class for representing the game world.
/// This contains the gridPlayer1, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
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
    GameState lastGameState;

    /// <summary>
    /// The main gridPlayer1 of the game.
    /// </summary>
    /// 
    TetrisGame game;

    TetrisGrid gridPlayer1;
    TetrisGrid gridPlayer2;

    Block block;
    Block previewBlock;

    RandomBag bag;

    GameOverScreen gameOverScreen;
    MainMenu mainMenu;

    MediaPlayer mediaPlayer;

    SoundEffect themeSong, shittySong;
    SoundEffect gameOverSound;

    Texture2D background1, background2;

    float secondsUntilNextTick;
    float secondsPerTick = 1;

    int score;
    int level;

    double leftHeldForSeconds;
    double rightHeldForSeconds;
    double downHeldForSeconds;

    bool is2Player;

    public GameWorld(TetrisGame tetrisGame)
    {
        game = tetrisGame;

        mediaPlayer = new MediaPlayer();

        random = new Random();
        gameState = GameState.MainMenu;

        themeSong = TetrisGame.ContentManager.Load<SoundEffect>("TetrisTheme");
        shittySong = TetrisGame.ContentManager.Load<SoundEffect>("shittyMusic");
        mediaPlayer.AddSongToQueue(themeSong);
        mediaPlayer.AddSongToQueue(shittySong);

        gameOverSound = TetrisGame.ContentManager.Load<SoundEffect>("gameOverSound");

        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");

        background1 = TetrisGame.ContentManager.Load<Texture2D>("background");

        gridPlayer1 = new TetrisGrid();
        gridPlayer2 = new TetrisGrid();

        bag = new RandomBag();

        gameOverScreen = new GameOverScreen(font);
        mainMenu = new MainMenu(font);

        secondsUntilNextTick = secondsPerTick;
        secondsPerTick = 1;

        score = 0;
        level = 1;

        previewBlock = bag.NextBlock(gridPlayer1);
        NewBlocks();
    }

    void Switch2PlayerMode()
    {
        is2Player = !is2Player;

        gridPlayer1.OffsetGrid(new Vector2(-TetrisGame.WorldSize.X / 4, 0));
        gridPlayer2.OffsetGrid(new Vector2(TetrisGame.WorldSize.X / 4, 0));
    }

    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        //helper variable for timers
        double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

        //updating timers for auto repeat
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

        //auto repeat: moving the blocks if a direction key is held for more than 0.3s and the other direction key is not pressed. It repeats every 0.05 seconds
        if (leftHeldForSeconds > 0.3f && rightHeldForSeconds == 0f)
        {
            leftHeldForSeconds = 0.25f;
            block.MoveLeft();
        }
        else if (rightHeldForSeconds > 0.3f && leftHeldForSeconds == 0f)
        {
            rightHeldForSeconds = 0.25f;
            block.MoveRight();
        }

        //code for regular horizontal movement, only if auto repeat is not occuring
        if (inputHelper.KeyPressed(Keys.Left) && rightHeldForSeconds == 0f)
            block.MoveLeft();

        if (inputHelper.KeyPressed(Keys.Right) && leftHeldForSeconds == 0f)
            block.MoveRight();

        //code for soft dropping
        if (inputHelper.KeyPressed(Keys.Down))
            block.MoveDown();
        if (downHeldForSeconds > secondsPerTick / 20)
        {
            block.MoveDown();
            downHeldForSeconds = 0f;
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
        mediaPlayer.Update();

        switch (gameState)
        {
            case GameState.MainMenu:
                lastGameState = gameState;
                mainMenu.Update(inputHelper);
                break;
            case GameState.Playing:
                if (gameState != lastGameState)
                    Reset();
                UpdateTickTime(gameTime);
                HandleInput(gameTime, inputHelper);
                lastGameState = gameState;
                break;
            case GameState.GameOver:
                lastGameState = gameState;
                gameOverScreen.Update(inputHelper);
                break;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Matrix spriteScale)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, spriteScale);
        switch (gameState)
        {   
            case GameState.MainMenu:
                mainMenu.Draw(spriteBatch);
                break;
            case GameState.Playing:
                DrawPlaying(gameTime, spriteBatch);
                break;

            case GameState.GameOver:
                DrawPlaying(gameTime, spriteBatch);
                gameOverScreen.Draw(spriteBatch);
                break;      
        }  
        spriteBatch.End();
    }

    public void GameOver()
    {
        gameOverSound.Play();
        game.IsMouseVisible = true;
        gameState = GameState.GameOver;
    }

    public void Reset()
    {
        gameState = GameState.Playing;
        game.IsMouseVisible= false;

        secondsUntilNextTick = secondsPerTick;
        secondsPerTick = 1;

        score = 0;
        level = 1;

        gridPlayer1.Clear();

        previewBlock = bag.NextBlock(gridPlayer1);
        NewBlocks();
    }
    public void NewBlocks()
    {
        secondsUntilNextTick = secondsPerTick;
        block = previewBlock;
        block.MoveToSpawnPosition();
        previewBlock = bag.NextBlock(gridPlayer1);
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

    void DrawPlaying(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(background1, Vector2.Zero, Color.White);
        gridPlayer1.Draw(gameTime, spriteBatch);
        block.Draw(spriteBatch);
        previewBlock.Draw(spriteBatch);
        spriteBatch.DrawString(font, level.ToString(), new Vector2(171 - font.MeasureString(level.ToString()).X / 2, 22), Color.Yellow);
        spriteBatch.DrawString(font, score.ToString(), new Vector2(628 - font.MeasureString(score.ToString()).X / 2, 22), Color.Yellow);
        Debug.WriteLine(new Vector2(0, font.LineSpacing + font.MeasureString("Score: ").Y) + font.MeasureString("Level " + 1022940));
    }

    public void StartNormalGame()
    {
        gameState = GameState.Playing;
    }
    public void ReturnToMainMenu() 
    {
        gameState = GameState.MainMenu;
    }

}
