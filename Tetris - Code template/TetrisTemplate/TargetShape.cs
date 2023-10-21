using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// TargetShape is a class that shows an extra objective. This objective is shape of collers which can be made in game.
/// When such an objective is reached there are an apropriate amount of lines cleared and a new objective is chosen.
/// </summary>
internal class TargetShape
{
    TetrisGrid.CellType[][,] shape;
    public TetrisGrid.CellType[,] Shape { get { return shape[currentShapeIndex]; } }

    int currentShapeIndex = 0;
    const int amountOfDifferentShapes = 7;

    Vector2 position;
    Texture2D blockTexture;
    TetrisGrid grid;
    public TargetShape(TetrisGrid grid)
    {
        //An array containing all the different shapes. With TetrisGrid CellTypes to indicate the coller. All shapes are a maximum of 4X4
        shape = new TetrisGrid.CellType[amountOfDifferentShapes][,] {
            new TetrisGrid.CellType[4, 4] {
                { TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow },
                { TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow },
                { TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow },
                { TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow }
            },
            new TetrisGrid.CellType[4, 4] {
                { TetrisGrid.CellType.green, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.green, TetrisGrid.CellType.green, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.empty, TetrisGrid.CellType.green, TetrisGrid.CellType.green, TetrisGrid.CellType.green },
                { TetrisGrid.CellType.empty, TetrisGrid.CellType.green, TetrisGrid.CellType.green, TetrisGrid.CellType.empty }
            },
            new TetrisGrid.CellType[4, 4] {
                { TetrisGrid.CellType.empty, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.green, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.red },
                { TetrisGrid.CellType.green, TetrisGrid.CellType.green, TetrisGrid.CellType.red, TetrisGrid.CellType.red},
                { TetrisGrid.CellType.empty, TetrisGrid.CellType.green, TetrisGrid.CellType.red, TetrisGrid.CellType.empty }
            },
            new TetrisGrid.CellType[4, 4] {
                { TetrisGrid.CellType.lightBlue, TetrisGrid.CellType.lightBlue, TetrisGrid.CellType.lightBlue, TetrisGrid.CellType.lightBlue},
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty, TetrisGrid.CellType.darkBlue},
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty, TetrisGrid.CellType.darkBlue},
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.darkBlue }
            },
            new TetrisGrid.CellType[4, 4] {
                { TetrisGrid.CellType.empty, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.empty, TetrisGrid.CellType.orange, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.empty }
            },
            new TetrisGrid.CellType[4, 4] {
                { TetrisGrid.CellType.empty, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.orange, TetrisGrid.CellType.orange},
                { TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.orange},
                { TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.yellow, TetrisGrid.CellType.yellow, TetrisGrid.CellType.orange}
            },
            new TetrisGrid.CellType[4, 4] {
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.green, TetrisGrid.CellType.green, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.green, TetrisGrid.CellType.green, TetrisGrid.CellType.orange, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.orange, TetrisGrid.CellType.empty }
            }};

        //Flipping x and y of every shape so that we can nicely edit the shapes in code
        
        for (int i = 0; i < shape.Length; i++)
        {
                
            TetrisGrid.CellType[,] newShape = new TetrisGrid.CellType[4, 4];
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    newShape[x, y] = shape[i][y, x];
                }
            }
            shape[i] = newShape;
        }
        
        //Randomizing which shape is the current shaper.
        NewShape();

        this.grid = grid;
        blockTexture = TetrisGame.ContentManager.Load<Texture2D>("block");

        //Setting the position.
        ApplyResolutionSettings();
        
    }
    /// <summary>
    /// Draws the roughly shape in the bottem left corner.
    /// </summary>
    /// <param name="spriteBatch"></param> the SpriteBatch object to draw the textures with
    /// <param name="worldOffset"></param> an offset that gets added to the position
    public void Draw(SpriteBatch spriteBatch, Vector2 worldOffset)
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Vector2 drawPos = position + new Vector2(x * blockTexture.Width, y * blockTexture.Height) + worldOffset;
                spriteBatch.Draw(blockTexture, drawPos, grid.CellColors[(int)shape[currentShapeIndex][x, y]]);

            }
        }
    }
    /// <summary>
    /// Picks a random shape as the current shape
    /// </summary>
    public void NewShape()
    {
        currentShapeIndex = GameWorld.Random.Next(amountOfDifferentShapes);
    }

    /// <summary>
    /// Makes sure the position is correct when the gameWorld gets a different size
    /// </summary>
    public void ApplyResolutionSettings()
    {
        position = new Vector2(TetrisGame.WorldSize.X / 2 - 287, 355);
    }
}


