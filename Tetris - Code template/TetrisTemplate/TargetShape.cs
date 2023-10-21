
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Drawing;

internal class TargetShape
{
    TetrisGrid.CellType[][,] shape;
    public TetrisGrid.CellType[,] Shape { get { return shape[currentShapeIndex]; } }

    int currentShapeIndex = 0;
    const int amountOffDifferentShapes = 7;

    Vector2 position;
    Texture2D blockTexture;
    TetrisGrid grid;
    public TargetShape(TetrisGrid grid)
    {
        shape = new TetrisGrid.CellType[amountOffDifferentShapes][,] {
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
                { TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.empty },
                { TetrisGrid.CellType.darkBlue, TetrisGrid.CellType.green, TetrisGrid.CellType.green, TetrisGrid.CellType.empty },
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
        

        NewShape();

        this.grid = grid;
        position = new Vector2(50, 500);
        blockTexture = TetrisGame.ContentManager.Load<Texture2D>("block");
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Vector2 drawPos = position + new Vector2(x * blockTexture.Width, y * blockTexture.Height);
                spriteBatch.Draw(blockTexture, drawPos, grid.CellColors[(int)shape[currentShapeIndex][x, y]]);

            }
        }
        //teken lings onder in de shape
    }

    public void NewShape()
    {
        
        currentShapeIndex = GameWorld.Random.Next(amountOffDifferentShapes);
    }


    /*Things to do

    * randomlie pick a shape
    * place for all the default shapes
    * check to see if the shape is made
    * een boolean die alles aan en uit zet
    * 
    dingen
    * max 4X4

    */

    }


