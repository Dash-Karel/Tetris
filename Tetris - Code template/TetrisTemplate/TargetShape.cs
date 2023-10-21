
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


internal class TargetShape
{
    bool[][,] shape;
    public bool[,] Shape { get { return shape[currentShapeIndex]; } }
    TetrisGrid.CellType[] cellType;
    public TetrisGrid.CellType CellType { get { return cellType[currentShapeIndex]; } }

    int currentShapeIndex = 1;
    const int amountOffDifferentShapes = 2;

    Vector2 position;
    Texture2D blockTexture;
    TetrisGrid grid;
    public TargetShape(TetrisGrid grid)
    {
        shape = new bool[amountOffDifferentShapes][,] { 
            new bool[4, 4] {{ true, true, true, true }, { true, true, true, true }, { true, true, true, true }, { true, true, true, true } },
            new bool[4, 4] {{ false, true, true, true }, { false, true, true, true }, { false, true, true, true }, { false, true, true, true } }};

        
        cellType = new TetrisGrid.CellType[2] { 
            TetrisGrid.CellType.yellow, 
            TetrisGrid.CellType.lightBlue};

        NewShape();

        this.grid = grid;
        position = new Vector2(50, 500);
        blockTexture = TetrisGame.ContentManager.Load<Texture2D>("block");
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int x = 0; x < 4; x++)
        {
            for(int y = 0; y < 4; y++)
            {
                Vector2 drawPos = position + new Vector2(x * blockTexture.Width, y * blockTexture.Height);
                if (shape[currentShapeIndex][x, y])
                    spriteBatch.Draw(blockTexture, drawPos, grid.CellColors[(int)cellType[currentShapeIndex]]);

                else
                    spriteBatch.Draw(blockTexture, drawPos, grid.CellColors[0]);
            }
        }
        //teken lings onder in de shape
    }

    void NewShape()
    {
        currentShapeIndex = GameWorld.Random.Next(2);
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


