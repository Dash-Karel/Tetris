using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
class TetrisGrid
{
    public enum cellType
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
    Color[] gridColors = new Color[] { Color.White, Color.Yellow, Color.LightBlue, Color.Purple, Color.Orange, Color.DarkBlue, Color.Green, Color.Red };

    //array representing the grid
    cellType[,] grid;

    public cellType[,] Grid { get { return grid; } }

    /// The number of grid elements in the x-direction.
    public int Width { get { return 10; } }

    /// The number of grid elements in the y-direction.
    public int Height { get { return 20; } }

    /// <summary>
    /// Creates a new TetrisGrid.
    /// </summary>
    /// <param name="b"></param>
    public TetrisGrid()
    {
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        position = new Vector2(TetrisGame.ScreenSize.X, TetrisGame.ScreenSize.Y) / 2;
        origin = new Vector2(Width * emptyCell.Width, Height * emptyCell.Height) / 2;
        Clear();
    }

    /// <summary>
    /// Draws the grid on the screen.
    /// </summary>
    /// <param name="gameTime">An object with information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">The SpriteBatch used for drawing sprites and text.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        for (int width = 0; width < Width; width++)
        {
            for (int height = 0; height < Height; height++)
            {
                spriteBatch.Draw(emptyCell, position - origin + new Vector2(width * emptyCell.Width, height * emptyCell.Height), gridColors[(int)grid[width, height]]);
            }
        }
    }

    /// <summary>
    /// Clears the grid.
    /// </summary>
    public void Clear()
    {
        grid = new cellType[Width, Height];
    }

    public void SetValueInGrid(Point index, cellType value)
    {
        if (index.X < 0 || index.Y < 0 || index.X >= Width || index.Y >= Height)
            Debug.WriteLine("index out of range" + index);
        else
            grid[index.X, index.Y] = value;
    }

    public Vector2 GetPositionOfCell(int cellX, int cellY)
    {
        return position - origin + new Vector2(cellX * emptyCell.Width, cellY * emptyCell.Height);
    }
    public Point GetCellAtPosition(Vector2 worldPosition)
    {
        Vector2 offset = worldPosition - (position - origin);
        int x = (int)offset.X / emptyCell.Width;
        int y = (int)offset.Y / emptyCell.Height;
        return new Point(x, y);
    }
}

