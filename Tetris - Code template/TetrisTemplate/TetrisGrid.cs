using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
class TetrisGrid
{
    public enum CellType
    {
        empty,
        yellow,
        lightBlue,
        purple,
        orange,
        darkBlue,
        green,
        red
    }

    /// The sprite of a single empty cell in the grid.
    Texture2D emptyCell;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position, origin;

    //array for mapping colours to a value
    Color[] cellColors = new Color[] { Color.LightGray, Color.Yellow, Color.CornflowerBlue, Color.Purple, Color.Orange, Color.DarkBlue, Color.Green, Color.Red };

    //array representing the grid
    CellType[,] grid;

    SoundEffect lineClearSound;

    GameWorld gameWorld;

    public CellType[,] Grid { get { return grid; } }

    public Color[] CellColors { get { return cellColors; } }

    /// The number of grid elements in the x-direction.
    public int Width { get { return 10; } }

    /// The number of grid elements in the y-direction.
    public int Height { get { return 20; } }

    /// <summary>
    /// Creates a new TetrisGrid.
    /// </summary>
    /// <param name="b"></param>
    public TetrisGrid(GameWorld gameWorld)
    {
        this.gameWorld = gameWorld;
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        lineClearSound = TetrisGame.ContentManager.Load<SoundEffect>("lineClearSound");
        origin = new Vector2(Width * emptyCell.Width, Height * emptyCell.Height) / 2;
        ApplyResolutionSettings();
        Clear();
    }

    /// <summary>
    /// Draws the grid on the screen.
    /// </summary>
    /// <param name="gameTime">An object with information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">The SpriteBatch used for drawing sprites and text.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 worldOffset)
    {
        for (int width = 0; width < Width; width++)
        {
            for (int height = 0; height < Height; height++)
            {
                spriteBatch.Draw(emptyCell, position - origin + new Vector2(width * emptyCell.Width, height * emptyCell.Height) + worldOffset, cellColors[(int)grid[width, height]]);
            }
        }
    }

    /// <summary>
    /// Clears the grid.
    /// </summary>
    public void Clear()
    {
        grid = new CellType[Width, Height];
    }

    public void SetValueInGrid(Point cell, CellType value)
    {
        if (CellIsWithinGrid(cell))
            grid[cell.X, cell.Y] = value;
    }
    //----------------------------------------------------------------------
    //----------------------------------------------------------------------
    public void CheckLines(int[] yCoordinates)
    {
        int LinesCleared = 0;
        foreach (int yCoordinate in yCoordinates)
        {
            if (yCoordinate >= Height || yCoordinate < 0)
                continue;

            bool lineFull = true;
            for (int x = 0; x < Width; x++)
            {
                if (grid[x, yCoordinate] == CellType.empty)
                {
                    lineFull = false;
                    break;
                }
            }

            if (lineFull)
            {
                DeleteLine(yCoordinate);
                LinesCleared++;
            }
        }
        if (LinesCleared > 0)
        {
            gameWorld.IncreaseScore(LinesCleared);
            lineClearSound.Play();
        }
    }

    void DeleteLine(int yCoordinate)
    {
        for(int x = 0; x < Width; x++)
        { 
            grid[x, yCoordinate] = CellType.empty;                
        }
        MoveCellsDown(yCoordinate);
    }

    public void PullCellsUp(Point pullPosition)
    {
        for(int y = pullPosition.Y; y < Height; y++)
        {
            for (int searchY = y; searchY < Height && searchY > pullPosition.Y && grid[pullPosition.X, y] == CellType.empty; searchY++)
            {
                if (grid[pullPosition.X, searchY] != CellType.empty)
                {
                    grid[pullPosition.X, y] = grid[pullPosition.X, searchY];
                    grid[pullPosition.X, searchY] = CellType.empty;
                }
            }
        }
    }

    public void PushCellsDown(Point pushPosition)
    {
        for (int y = Height - 1; y > pushPosition.Y; y--)
        {
            for(int searchY = y - 1; searchY >= pushPosition.Y && grid[pushPosition.X, y] == CellType.empty; searchY--)
            {
                if (grid[pushPosition.X, searchY] != CellType.empty)
                {
                    grid[pushPosition.X, y] = grid[pushPosition.X, searchY];
                    grid[pushPosition.X, searchY] = CellType.empty;
                }
            }
        }
    }

    void MoveCellsDown(int startingYCoordinate)
    {
        for(int x=0; x < Width; x++)
            for (int y = startingYCoordinate; y > 0; y--)
                grid[x,y] = grid[x,y - 1];
    }
    public void MoveAllLinesUp()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 1; y < Height; y++)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = CellType.empty;
            }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------

    public void OffsetGrid(Vector2 offset)
    {
        position += offset;
    }

    public void ApplyResolutionSettings()
    {
        position = new Vector2(TetrisGame.WorldSize.X, TetrisGame.WorldSize.Y) / 2;
    }

    /*
    public Point GetCellAtPosition(Vector2 worldPosition)
    {
        Vector2 offset = worldPosition - position - origin;
        int x = (int)offset.X / emptyCell.Width;
        int y = (int)offset.Y / emptyCell.Height;
        return new Point(x, y);
    }
    */
    public Vector2 GetPositionOfCell(Point cell)
    {
        return position - origin + new Vector2(cell.X * emptyCell.Width, cell.Y * emptyCell.Height);
    }
    public bool CellIsValid(Point cell)
    {
        //return true if the cell is still above the grid, as it should be able to move in that case
        if (cell.Y < 0 && cell.X >= 0 && cell.X < Width )
            return true;

        //returns true if the cell is within the grid and not already occupied or if the cell is still above the grid
        if (CellIsWithinGrid(cell))
            if (!CellIsOccupied(cell))
                return true;
        return false;
    }
    bool CellIsWithinGrid(Point cell)
    {
        return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
    }
    bool CellIsOccupied(Point cell)
    {
        return grid[cell.X, cell.Y] != CellType.empty;
    }
}

