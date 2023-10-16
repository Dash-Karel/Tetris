using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

internal class Block
{
    Texture2D texture;
    TetrisGrid grid;

    SoundEffect placeSound;

    Vector2 offset;

    //represents the position within the grid
    protected Point position;

    protected TetrisGrid.CellType color;
    protected bool[,] shape;

    int Size
    {
        get { return shape.GetLength(0); }
    }

    protected Block(TetrisGrid grid)
    {
        placeSound = TetrisGame.ContentManager.Load<SoundEffect>("placeBlockSound");
        texture = TetrisGame.ContentManager.Load<Texture2D>("block");
        this.grid = grid;

        //ensures sprite is drawn in the next box
        offset = new Vector2(228, 186);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int width = 0; width < Size; width++)
        {
            for (int height = 0; height < Size; height++)
            {
                if (shape[width, height] == true)
                    spriteBatch.Draw(texture, grid.GetPositionOfCell(position) + new Vector2(width * texture.Width, height * texture.Height) - offset, grid.CellColors[(int)color]);
            }
        }
    }
    bool MoveWasValid()
    {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                if (shape[x, y] == true)
                    if (!grid.CellIsValid(new Point(position.X + x, position.Y + y)))
                        return false;
        return true;
    }
    void PlaceBlock()
    {
        //variable for keeping track of where the actual bottom of the block is
        int bottomOfBlockCoordinate = 0;

        //place the block in the grid
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                if (shape[x, y])
                {
                    bottomOfBlockCoordinate = y;
                    grid.SetValueInGrid(new Point(position.X + x, position.Y + y), color);
                }

        //If the bottom of the block lies outside the grid when placing it the game is over
        if (position.Y + bottomOfBlockCoordinate < 0)
        {
            TetrisGame.GameWorld.GameOver();
            return;
        }

        placeSound.Play();

        // Check if any of the lines got full
        int[] yCoordinates = new int[Size];
        for (int y = 0; y < Size; y++)
            yCoordinates[y] = position.Y + y;

        grid.CheckLines(yCoordinates);
    }
    public void MoveToSpawnPosition()
    {
        offset = Vector2.Zero;
    }
    public void RotateLeft()
    {
        //requires square grids for the shapes
        bool[,] newShape = new bool[Size, Size];
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                newShape[x, y] = shape[Size - 1 - y, x];
            }
        }
        bool[,] originalShape = shape;
        shape = newShape;
        if (!MoveWasValid())
            shape = originalShape;
    }
    public void RotateRight()
    {
        //requires square grids for the shapes
        bool[,] newShape = new bool[Size, Size];
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                newShape[x, y] = shape[y, Size - 1 - x];
            }
        }
        bool[,] originalShape = shape;
        shape = newShape;
        if (!MoveWasValid())
            shape = originalShape;
    }
    public void MoveLeft()
    {
        position.X--;
        if (!MoveWasValid())
            position.X++;
    }
    public void MoveRight()
    {
        position.X++;
        if (!MoveWasValid())
            position.X--;
    }
    public bool MoveDown()
    {
        //moves the block down once if possible and returns false when the block is placed as the block didn't move
        position.Y++;
        if (!MoveWasValid())
        {
            position.Y--;
            PlaceBlock();
            TetrisGame.GameWorld.NewBlocks();
            return false;
        }
        return true;
    }
    public void HardDrop()
    {
        while (MoveDown()) ;
    }

    protected void setOffset()
    {
        offset = new Vector2(-276, -220) + new Vector2(Size * texture.Width, Size * texture.Height) / 2;
    }
}
internal class OBlock : Block
{
    public OBlock(TetrisGrid grid) : base(grid)
    {
        shape = new bool[,] { { true, true }, { true, true } };
        color = TetrisGrid.CellType.yellow;
        position = new Point(4, -2);

        setOffset();
    }
}
internal class IBlock : Block
{
    public IBlock(TetrisGrid grid) : base(grid)
    {
        shape = new bool[,] { { false, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, true, false, false } };
        color = TetrisGrid.CellType.lightBlue;
        position = new Point(3, -2);

        setOffset();
    }
}
internal class TBlock : Block
{
    public TBlock(TetrisGrid grid) : base(grid)
    {
        shape = new bool[,] { { false, true, false }, { true, true, false }, { false, true, false } };
        color = TetrisGrid.CellType.purple;
        position = new Point(3, -2);

        setOffset();
    }
}
internal class LBlock : Block
{
    public LBlock(TetrisGrid grid) : base(grid)
    {
        shape = new bool[,] { { false, true, false }, { false, true, false }, { true, true, false } };
        color = TetrisGrid.CellType.orange;
        position = new Point(3, -2);

        setOffset();
    }
}
internal class JBlock : Block
{
    public JBlock(TetrisGrid grid) : base(grid)
    {
        shape = new bool[,] { { true, true, false }, { false, true, false }, { false, true, false } };
        color = TetrisGrid.CellType.darkBlue;
        position = new Point(3, -2);

        setOffset();
    }
}
internal class SBlock : Block
{
    public SBlock(TetrisGrid grid) : base(grid)
    {
        shape = new bool[,] { { false, true, false }, { true, true, false }, { true, false, false } };
        color = TetrisGrid.CellType.green;
        position = new Point(3, -2);

        setOffset();
    }
}
internal class ZBlock : Block
{
    public ZBlock(TetrisGrid grid) : base(grid)
    {
        shape = new bool[,] { { true, false, false }, { true, true, false }, { false, true, false } };
        color = TetrisGrid.CellType.red;
        position = new Point(3, -2);

        setOffset();
    }
}
