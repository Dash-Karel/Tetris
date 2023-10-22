using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

/// <summary>
/// A class for representing the game world.
/// This contains the grid, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
{
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
    /// The main grid of the game.
    /// </summary>
    TetrisGrid grid;

    TetrisGame game;

    Block block;
    Block previewBlock;

    Block holdBlock;
    bool holdUsed;

    RandomBag bag;

    TargetShape targetShape;

    SoundEffect gameOverSound;

    Keys rotateLeftKey, moveLeftKey, rotateRightKey, moveRightKey, moveDownKey, hardDropKey, holdKey;

    float secondsUntilNextTick;
    float secondsPerTick = 1;

    int score;
    int level;
    int linesClearedSinceLevelUp;

    double leftHeldForSeconds;
    double rightHeldForSeconds;
    double downHeldForSeconds;

    public Vector2 WorldOffset { get; private set; }

    Vector2 levelStringLocation, scoreStringLocation;

    bool isPlayerOne;
    bool targetShapeMode;

    public GameWorld(TetrisGame tetrisGame, SpriteFont font, bool isPlayerOne, Keys moveLeft = Keys.Left, Keys rotateLeft = Keys.A, Keys moveRight = Keys.Right, Keys rotateRight = Keys.D, Keys moveDown = Keys.Down, Keys hardDrop = Keys.Space, Keys hold = Keys.LeftShift)
    {
        game = tetrisGame;

        this.isPlayerOne = isPlayerOne;

        random = new Random();

        gameOverSound = TetrisGame.ContentManager.Load<SoundEffect>("gameOverSound");

        this.font = font;

        grid = new TetrisGrid(this);

        bag = new RandomBag(this);

        ChangeKeyBindings(moveLeft, rotateLeft, moveRight, rotateRight, moveDown, hardDrop, hold);

        secondsUntilNextTick = secondsPerTick;
        secondsPerTick = 1;

        score = 0;
        level = 1;

        previewBlock = bag.NextBlock(grid);
        NewBlocks();

        holdUsed = false;
    }

    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        //helper variable for timers
        double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

        //updating timers for auto repeat
        if (inputHelper.KeyDown(moveLeftKey))
            leftHeldForSeconds += elapsedSeconds;
        else
            leftHeldForSeconds = 0f;

        if (inputHelper.KeyDown(moveRightKey))
            rightHeldForSeconds += elapsedSeconds;
        else
            rightHeldForSeconds = 0f;

        //updating timer for soft dropping
        if (inputHelper.KeyDown(moveDownKey))
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
        if (inputHelper.KeyPressed(moveLeftKey) && rightHeldForSeconds == 0f)
            block.MoveLeft();

        if (inputHelper.KeyPressed(moveRightKey) && leftHeldForSeconds == 0f)
            block.MoveRight();

        //code for soft dropping
        if (inputHelper.KeyPressed(moveDownKey))
            block.MoveDown();
        if (downHeldForSeconds > secondsPerTick / 20)
        {
            block.MoveDown();
            downHeldForSeconds = 0f;
        }
        //code for hard dropping
        if (inputHelper.KeyPressed(hardDropKey))
            block.HardDrop();

        if (inputHelper.KeyPressed(rotateRightKey))
            block.RotateRight();

        if (inputHelper.KeyPressed(rotateLeftKey))
            block.RotateLeft();

        if (inputHelper.KeyPressed(holdKey))
            Hold();
    }
    
    public void Update(GameTime gameTime, InputHelper inputHelper)
    {
        UpdateTickTime(gameTime);
        HandleInput(gameTime, inputHelper);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        grid.Draw(gameTime, spriteBatch, WorldOffset);
        block.Draw(spriteBatch, WorldOffset);
        previewBlock.Draw(spriteBatch, WorldOffset);
        if(holdBlock != null)
            holdBlock.Draw(spriteBatch, WorldOffset);
        if (targetShapeMode)
            targetShape.Draw(spriteBatch, WorldOffset);

        spriteBatch.DrawString(font, level.ToString(), levelStringLocation - font.MeasureString(level.ToString()) / 2 + WorldOffset, Color.Yellow);
        spriteBatch.DrawString(font, score.ToString(), scoreStringLocation - font.MeasureString(score.ToString()) / 2 + WorldOffset, Color.Yellow);
    }

    public void Reset(bool targetShapeMode)
    {
        this.targetShapeMode = targetShapeMode;

        if (targetShapeMode)
            targetShape = new TargetShape(grid);

        secondsUntilNextTick = secondsPerTick;
        secondsPerTick = 1;

        score = 0;
        level = 1;

        grid.Clear();

        holdBlock = null;
        previewBlock = bag.NextBlock(grid);
        NewBlocks();
    }
    public void ApplyResolutionSettings()
    {
        grid.ApplyResolutionSettings();
        targetShape.ApplyResolutionSettings();
        levelStringLocation = new Vector2(TetrisGame.WorldSize.X / 2 - 228, 33);
        scoreStringLocation = new Vector2(TetrisGame.WorldSize.X / 2 + 228, 33);
    }
    void Hold()
    {
        if (!holdUsed)
        {
            Block currentHeld = holdBlock;
            holdBlock = block;
            holdBlock.Reset();
            holdBlock.setOffset(new Vector2(-93, 155));
            if (!(currentHeld == null))
            {
                block = currentHeld;
                block.MoveToSpawnPosition();
            }
            else
                NewBlocks();

            holdUsed = true;
        }
    }
    public void NewBlocks()
    {
        holdUsed = false;
        secondsUntilNextTick = secondsPerTick;
        block = previewBlock;
        block.MoveToSpawnPosition();
        previewBlock = bag.NextBlock(grid);
    }
    public void IncreaseScore(int LinesCleared)
    {
        if (LinesCleared >= 4)
        {
            score += 200 * LinesCleared * level;

            //send lines to the other player as a reward for getting a tetris
            for(int x = 3; x < LinesCleared; x++)
                game.SendLine(isPlayerOne);
        }
        else
            score += (2 * LinesCleared - 1) * 100 * level;

        linesClearedSinceLevelUp += LinesCleared;

        if (linesClearedSinceLevelUp >= 10)
        {
            //save the linesClearedSinceLevelUp beacause the setLevel method resets it(so it can be used for syncing the level of the other player)
            int currentLines = linesClearedSinceLevelUp;

            //increase the level
            game.SyncLevel(level + 1);
            //send a line to the other player as a reward for completing the level first
            game.SendLine(isPlayerOne);

            linesClearedSinceLevelUp = currentLines % 10;
        }
    }
    public void setLevel(int level)
    {
        linesClearedSinceLevelUp = 0;
        this.level = level;
        secondsPerTick = MathF.Pow((0.8f - ((level - 1) * 0.007f)), level - 1);
    }

    public void ReceiveLine()
    {
        grid.MoveAllLinesUp();
        block.MoveUp();
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
    public void OffsetWorld(Vector2 offset)
    {
        WorldOffset = offset;
    }
    public void ChangeKeyBindings(Keys moveLeft, Keys rotateLeft, Keys moveRight, Keys rotateRight, Keys moveDown, Keys hardDrop, Keys hold)
    {
        moveLeftKey = moveLeft;
        rotateLeftKey = rotateLeft;
        moveRightKey = moveRight;
        rotateRightKey = rotateRight;
        moveDownKey = moveDown;
        hardDropKey = hardDrop;
        holdKey = hold;
    }
    public void GameOver()
    {
        gameOverSound.Play();
        game.GameOver(isPlayerOne);
    }
    public void CheckTargetShape()
    {
        if (grid.CheckTargetShape(targetShape.Shape))
        {
            targetShape.NewShape();
        }
    }
}
