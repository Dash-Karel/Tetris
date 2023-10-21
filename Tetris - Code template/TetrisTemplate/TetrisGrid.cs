using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
class TetrisGrid
{
    //an enumerator with every value a cell can have
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

    // The sprite of a single empty cell in the grid.
    Texture2D emptyCell;

    // The position at which this TetrisGrid should be drawn, as well as an origin.
    Vector2 position, origin;

    //array for mapping colours to a value
    Color[] cellColors = new Color[] { Color.LightGray, Color.Yellow, Color.CornflowerBlue, Color.Purple, Color.Orange, Color.DarkBlue, Color.Green, Color.Red };

    //the sound that gets played when a line is cleared
    SoundEffect lineClearSound;

    //a reference to a gameWorld to be able to call methods such as increaseScore and GameOver
    GameWorld gameWorld;

    public Color[] CellColors { get { return cellColors; } }

    // The number of grid elements in the x-direction.
    public int Width { get { return 10; } }

    // The number of grid elements in the y-direction.
    public int Height { get { return 20; } }

    //array representing the grid
    public CellType[,] Grid { get; private set; }

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
                spriteBatch.Draw(emptyCell, position - origin + new Vector2(width * emptyCell.Width, height * emptyCell.Height) + worldOffset, cellColors[(int)Grid[width, height]]);
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    //Methods related to checking if lines are full and removing them

    /// <summary>
    /// Checks if lines should be cleared and if so removes them from the grid while also increasing the score
    /// </summary>
    /// <param name="yCoordinates"></param> an array that contains all the y coordinates of the lines that should be cleared
    public void CheckLines(int[] yCoordinates)
    {
        //a variable to keep track of how many lines are cleared to be able to pass that information to the IncreaseScore method of a gameWorld
        int LinesCleared = 0;

        //loops over every y coordinate in the array and checks whether it should be removed
        foreach (int yCoordinate in yCoordinates)
        {
            //if the coordinate does not lie within the grid, skip over that iteration
            if (yCoordinate >= Height || yCoordinate < 0)
                continue;

            //a variable to keep track of whether the entire line is full
            bool lineFull = true;

            //loops over every x coordinate in the line, when an empty cell is found lineFull is set to false and the loop breaks as it clear that the line isn't full
            for (int x = 0; x < Width; x++)
            {
                if (Grid[x, yCoordinate] == CellType.empty)
                {
                    lineFull = false;
                    break;
                }
            }

            //if the line turns out to be full delete it and increment the amount of lines cleared
            if (lineFull)
            {
                DeleteLine(yCoordinate);
                LinesCleared++;
            }
        }

        //if any of the lines have been cleared, play a sound to indicate this and increase the score
        if (LinesCleared > 0)
        {
            gameWorld.IncreaseScore(LinesCleared);
            lineClearSound.Play();
        }
    }
    /// <summary>
    /// Checks if the target shape is made by looping over all the y coordinates 
    /// and checking if the blocks under and to the right correspond to the shape that is given.
    /// </summary>
    /// <param name="shape"></param> Is an array of the shape that needs to be checked for
    /// <returns></returns> Returns a boolean to signal if the shape has been completed.
    public bool CheckTargetShape(TetrisGrid.CellType[,] shape)
    {

        //Calculating the amount of empty lines in the shape
        int emptyShapeLines = 0;
        for (int yOfShape = 0; yOfShape < shape.GetLength(0); yOfShape++)
        {
            bool shapeLineIsEmpty = true;
            for (int xOfShape = 0; xOfShape < shape.GetLength(1); xOfShape++)
            {
                if (shape[xOfShape, yOfShape] != TetrisGrid.CellType.empty)
                {
                    shapeLineIsEmpty = false;
                }
            }
            if (shapeLineIsEmpty)
                emptyShapeLines++;
        }
        int LinesCleared = 0;
        //Looping over all the y coordinates.
        for (int yCoordinate = 0; yCoordinate < Height - shape.GetLength(1) + 1; yCoordinate++)
        {
            //Checking to see if we are within the grid
            if (yCoordinate >= Height - shape.GetLength(1) + 1|| yCoordinate < 0)
                continue;

            //Looping over all the x coordinates.
            for (int x = 0; x < Width - shape.GetLength(0) + 1; x++)
            {
                bool isShape = true;
                //Looping over the shape grid
                for (int xOfShape = 0; xOfShape < shape.GetLength(0); xOfShape++)
                {
                    for (int yOfShape = 0; yOfShape < shape.GetLength(1); yOfShape++)
                    {
                        //Checking if the shape type is empty if so the check for that cell will be skipped.
                        if (shape[xOfShape, yOfShape] != TetrisGrid.CellType.empty)
                        {
                            //Checking if the cell type in the shape is the same as the one in the grid
                            if (Grid[x + xOfShape, yCoordinate + yOfShape] != shape[xOfShape, yOfShape])
                            {
                                isShape = false;
                            }
                        }
                    }
                }//Deleting the lines if the shape is found
                if (isShape)
                {//Also skipping the empty lines in the shape
                    for (int yOfShape = emptyShapeLines; yOfShape < shape.GetLength(1); yOfShape++)
                    {
                        DeleteLine(yCoordinate + yOfShape);
                        LinesCleared++;
                        
                    }
                }
            }
        }//If lines have been cleared, play a sound to indicate this and increase the score.
        if (LinesCleared > 0)
        {
            gameWorld.IncreaseScore(LinesCleared);
            lineClearSound.Play();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Deletes a line and makes sure the lines above it are moved down. Plays a nice deletion effect as well
    /// </summary>
    /// <param name="yCoordinate"></param> The y coordinate of the line that needs to be removed
    void DeleteLine(int yCoordinate)
    {
        //set every cell in the line to empty
        for (int x = 0; x < Width; x++)
        {
            //play effects
            if (x < Width / 2)
                TetrisGame.EffectsManager.NewEffect(GetPositionOfCell(new Point(x, yCoordinate)) + gameWorld.WorldOffset, 1.5f * MathF.PI, 350f, cellColors[(int)Grid[x, yCoordinate]], 5f, "block", 15f);
            else
                TetrisGame.EffectsManager.NewEffect(GetPositionOfCell(new Point(x, yCoordinate))+ gameWorld.WorldOffset, 0.5f * MathF.PI, 350f, cellColors[(int)Grid[x, yCoordinate]], 5f, "block", 15f);

            Grid[x, yCoordinate] = CellType.empty;
        }
        //make sure the cells above are moved down
        MoveCellsDown(yCoordinate);
    }

    /// <summary>
    /// Moves all cells above a specified y coordinate down to that y coordinate
    /// </summary>
    /// <param name="startingYCoordinate"></param> the y coordinate at which the loop starts, all cells above it will be moved down one cell
    void MoveCellsDown(int startingYCoordinate)
    {
        for (int x = 0; x < Width; x++)
            for (int y = startingYCoordinate; y > 0; y--)
                Grid[x, y] = Grid[x, y - 1];
    }


    /// <summary>
    /// A method to move all cells one row up
    /// </summary>
    public void MoveAllLinesUp()
    {
        //loops over every cell in the grid except for the very top row
        for (int x = 0; x < Width; x++)
            for (int y = 1; y < Height; y++)
            {
                //moves one cell up
                Grid[x, y - 1] = Grid[x, y];
                Grid[x, y] = CellType.empty;
            }
    }

    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    //Methods used only by special blocks

    /// <summary>
    /// This method pulls all cells under a certain position up to that position without cells ending up inside each other, as a kind of magnet.
    /// </summary>
    /// <param name="pullPosition"></param> The postion of the cell that this pull is applied to
    public void PullCellsUp(Point pullPosition)
    {
        //loops over every cell under the pullPosition starting at the top
        for(int y = pullPosition.Y; y < Height; y++)
        {
            //at each y position that is empty a search is made downwards for an empty cell, when one is found it is put at the current y position and the found cell is made empty.
            for (int searchY = y; searchY < Height && searchY > pullPosition.Y && Grid[pullPosition.X, y] == CellType.empty; searchY++)
            {
                if (Grid[pullPosition.X, searchY] != CellType.empty)
                {
                    Grid[pullPosition.X, y] = Grid[pullPosition.X, searchY];
                    Grid[pullPosition.X, searchY] = CellType.empty;
                }
            }
        }
    }

    /// <summary>
    /// This method pushes all cells from a specified position all the way down, without cells ending up inside each other.
    /// </summary>
    /// <param name="pushPosition"></param> The position of the cell at which this downwards push starts
    public void PushCellsDown(Point pushPosition)
    {
        //loops over every cell under the pullPosition starting at the bottom
        for (int y = Height - 1; y > pushPosition.Y; y--)
        {
            //at each y position that is empty a search is made upwards for an empty cell, when one is found it is put at the current y position and the found cell is made empty.
            for (int searchY = y - 1; searchY >= pushPosition.Y && Grid[pushPosition.X, y] == CellType.empty; searchY--)
            {
                if (Grid[pushPosition.X, searchY] != CellType.empty)
                {
                    Grid[pushPosition.X, y] = Grid[pushPosition.X, searchY];
                    Grid[pushPosition.X, searchY] = CellType.empty;
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    //Methods for getting information about the grid and setting data in the grid

    /// <summary>
    /// Clears the grid.
    /// </summary>
    public void Clear()
    {
        Grid = new CellType[Width, Height];
    }

    /// <summary>
    /// Sets a value in the grid if the speciefied location is within the grid
    /// </summary>
    /// <param name="cell"></param> the location or cell of the value that needs to be changed
    /// <param name="value"></param> the value the grid at that location will be set to
    public void SetValueInGrid(Point cell, CellType value)
    {
        if (CellIsWithinGrid(cell))
            Grid[cell.X, cell.Y] = value;
    }

    /// <summary>
    /// Returns the world position of a cell at a grid position
    /// </summary>
    /// <param name="cell"></param> the position in the grid you want the world position of
    /// <returns></returns> A Vector2 indicating the world position
    public Vector2 GetPositionOfCell(Point cell)
    {
        return position - origin + new Vector2(cell.X * emptyCell.Width, cell.Y * emptyCell.Height);
    }

    /// <summary>
    /// Checks whether it is allowed to be at a certain position in the grid, used by Block to check if a move is valid
    /// </summary>
    /// <param name="cell"></param> the positon within the grid
    /// <returns></returns> a boolean whether the position is valid, true if it is
    public bool CellIsValid(Point cell)
    {
        //return true if the cell/block is still above the grid, as a block should be able to move in that case
        if (cell.Y < 0 && cell.X >= 0 && cell.X < Width )
            return true;

        //returns true if the cell is within the Grid and not already occupied
        if (CellIsWithinGrid(cell) && !CellIsOccupied(cell))
                return true;
        return false;
    }

    /// <summary>
    /// A method to check whether a location in grid positions lies within the grid
    /// </summary>
    /// <param name="cell"></param> the location of the cell/grid position you want to check
    /// <returns></returns> whether the cell/location is within the grid, true if it is
    bool CellIsWithinGrid(Point cell)
    {
        return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
    }

    /// <summary>
    /// A method to check whether a cell is empty or not
    /// </summary>
    /// <param name="cell"></param> the position within the grid of the cell you want to check
    /// <returns></returns> true if the cell is not empty
    bool CellIsOccupied(Point cell)
    {
        return Grid[cell.X, cell.Y] != CellType.empty;
    }

    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    //Methods for changing the position of the entire grid

    /// <summary>
    /// offsets the grid
    /// </summary>
    /// <param name="offset"></param> A vector that indicates the amount and direction of the offset
    public void OffsetGrid(Vector2 offset)
    {
        position += offset;
    }

    /// <summary>
    /// Makes sure the grid's position is updated when the screen size changes
    /// </summary>
    public void ApplyResolutionSettings()
    {
        position = new Vector2(TetrisGame.WorldSize.X, TetrisGame.WorldSize.Y) / 2;
    }

}

