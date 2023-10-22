using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// This class represents the falling block that the player can control.
/// The class includes movement, rotation, placing the block in the grid and the different block types.
/// This file also includes all the subclasses representing the different shapes.
/// </summary>
internal class Block
{
    public enum BlockType { normal, explosive, pushDown, pullUp}
    BlockType blockType;
    Texture2D[] typeToTexture;

    Texture2D texture;
    TetrisGrid grid;
    GameWorld gameWorld;

    SoundEffect placeSound;
    SoundEffect explosionSound, pushDownSound, pullUpSound;

    Vector2 offset;

    //represents the position within the grid
    protected Point position;

    protected TetrisGrid.CellType color;
    protected bool[,] shape;

    int Size
    {
        get { return shape.GetLength(0); }
    }
    //Constructor
    protected Block(TetrisGrid grid, GameWorld gameWorld, BlockType type = BlockType.normal)
    {
        //Reference to the gameWorld.
        this.gameWorld = gameWorld;

        //Loading in the different sounds.
        placeSound = TetrisGame.ContentManager.Load<SoundEffect>("placeBlockSound");
        explosionSound = TetrisGame.ContentManager.Load<SoundEffect>("explosionSound");
        pushDownSound = TetrisGame.ContentManager.Load<SoundEffect>("pushDownSound");
        pullUpSound = TetrisGame.ContentManager.Load<SoundEffect>("pullUpSound");
        //Loading in the different Textures.
        texture = TetrisGame.ContentManager.Load<Texture2D>("block");
        typeToTexture = new Texture2D[4];
        typeToTexture[(int)BlockType.explosive] = TetrisGame.ContentManager.Load<Texture2D>("bomb");
        typeToTexture[(int)BlockType.pullUp] = TetrisGame.ContentManager.Load<Texture2D>("upArrow");
        typeToTexture[(int)BlockType.pushDown] = TetrisGame.ContentManager.Load<Texture2D>("downArrow");

        //Reference to the grid.
        this.grid = grid;

        //Setting the blocktype
        blockType = type;
    }
    /// <summary>
    /// Drawing the Block by looping over the shape of the block and adding the top left position of the shape.
    /// </summary>
    /// <param name="spriteBatch"></param> Used for drawing the textures.
    /// <param name="worldOffset"></param> Is the same offset that the grid has in pixels.
    public void Draw(SpriteBatch spriteBatch, Vector2 worldOffset)
    {
        //Looping over all the cells of the shape.
        for (int width = 0; width < Size; width++)
        {
            for (int height = 0; height < Size; height++)
            {
                //Drawing the block piece if it is in the shape.
                if (shape[width, height] == true)
                {
                    //Determining the draw position by adding the cell position, the blockpiece offset and the grid offset.
                    Vector2 drawPos = grid.GetPositionOfCell(position) + new Vector2(width * texture.Width, height * texture.Height) + offset + worldOffset;

                    //Drawing the blockpiece.
                    spriteBatch.Draw(texture, drawPos, grid.CellColors[(int)color]);

                    //Adding the blocktype texture if the block is not a normal block.
                    if(blockType != BlockType.normal)
                        spriteBatch.Draw(typeToTexture[(int)blockType], drawPos, Color.White);
                }
            }
        }
    }
    /// <summary>
    /// Goes over all the positions within the shape and checks if the cell was not ocuppied.
    /// </summary>
    /// <returns></returns> Returns if the move was valid.
    bool MoveWasValid()
    {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                if (shape[x, y] == true)
                    if (!grid.CellIsValid(new Point(position.X + x, position.Y + y)))
                        return false;
        return true;
    }

    /// <summary>
    /// Places the block in the grid.
    /// And applying every effect that is selected.
    /// </summary>
    void PlaceBlock()
    {
        //variable for keeping track of where the actual bottom of the block is
        int bottomOfBlockCoordinate = 0;
        
        //Looping over the entire block to place the block and display a general effect.
        for (int x = 0; x < Size; x++)
        {
            for(int y = Size - 1; y >= 0; y--)
            {
                //play general placement effects
                if (!grid.CellIsValid(new Point(position.X + x, position.Y + y + 1)) && shape[x,y])
                    TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x, position.Y + y + 1)) + gameWorld.WorldOffset, MathF.PI, 200f, Color.LightGray, 1, "dust", 10f);
            }
            for (int y = 0; y < Size; y++)
            {
                if (shape[x, y])
                {
                    //place the block in the grid
                    grid.SetValueInGrid(new Point(position.X + x, position.Y + y), color);
                    bottomOfBlockCoordinate = y;
                }
            }
        }


        //for most of these functions the blocks should all be placed first before they can be called so they need to be in a seperate loop
        //Looping over the Shape to apply all the special effects.
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                if (shape[x, y])
                {
                    switch (blockType)
                    {
                        case BlockType.explosive:
                            ExplodeBlockPiece(x, y);
                            break;
                        case BlockType.pushDown:
                            PushDownBlockPiece(x,y);
                            break;
                        case BlockType.pullUp:
                            PushUpBlockPiece(x,y);
                            break;
                    }
                }
            }
        }

        //If the bottom of the block lies outside the grid when placing it the game is over
        if (position.Y + bottomOfBlockCoordinate < 0)
        {
            gameWorld.GameOver();
            return;
        }

        //Playing the appropriate sounds and defining the amount line cleared checks that need to be done.
        int numberOfChecks;
        switch (blockType)
        {
            case BlockType.normal:
                placeSound.Play();
                numberOfChecks = Size;
                break;
            case BlockType.explosive:
                explosionSound.Play();
                numberOfChecks = 0;
                break;
            case BlockType.pushDown:
                pushDownSound.Play();
                numberOfChecks = grid.Height - position.Y;
                break;
            case BlockType.pullUp:
                pullUpSound.Play();
                numberOfChecks = grid.Height - position.Y;
                break;
            default:
                numberOfChecks= 0;
                break;
        }
        //Making an array with all the y coordinates that need to be checked.
        int[] yCoordinates = new int[numberOfChecks];
        for (int y = 0; y < numberOfChecks; y++)
            yCoordinates[y] = y + position.Y;

        if(TetrisGame.UseTargetShape)
            gameWorld.CheckTargetShape();

        grid.CheckLines(yCoordinates);
    }
    /// <summary>
    /// This method aplies the explosion to a certain grid position.
    /// It does this by setting surrounding grid positions to empty
    /// and shooting effects from the position.
    /// </summary>
    /// <param name="x"></param> The x coordinate.
    /// <param name="y"></param> The y coordinate
    void ExplodeBlockPiece(int x, int y)
    {
        //Loops over the range of the explosion and sets the grid celltype to empty.
        for (int explosionX = -2; explosionX < 3; explosionX++)
            for (int explosionY = -2; explosionY < 3; explosionY++)
                grid.SetValueInGrid(new Point(position.X + x + explosionX, position.Y + y + explosionY), TetrisGrid.CellType.empty);

        //Creates the effect for the explosion
        float effectSpeed = 400f;
        float rotationSpeed = 10f;
        if (x - 1 < 0 || !shape[x - 1, y])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x - 1, position.Y + y)), 1.5f * MathF.PI, effectSpeed, Color.White, 2f, "fire", rotationSpeed);
        if (x + 1 >= Size || !shape[x + 1, y])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x + 1, position.Y + y)), 0.5f * MathF.PI, effectSpeed, Color.White, 2f, "fire", rotationSpeed);
        if (y - 1 < 0 || !shape[x, y - 1])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x, position.Y + y - 1)), MathF.PI, effectSpeed, Color.White, 2f, "fire", rotationSpeed);
        if (y + 1 >= Size || !shape[x, y + 1])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x, position.Y + y + 1)), 0f, effectSpeed, Color.White, 2f, "fire", rotationSpeed);
        if (x - 1 < 0 || y - 1 < 0 || !shape[x - 1, y - 1])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x - 1, position.Y + y - 1)), 1.25f * MathF.PI, effectSpeed, Color.White, 2.828f, "fire", rotationSpeed);
        if (x + 1 >= Size || y + 1 >= Size || !shape[x + 1, y + 1])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x + 1, position.Y + y + 1)), 0.25f * MathF.PI, effectSpeed, Color.White, 2.828f, "fire", rotationSpeed);
        if (x - 1 < 0 || y + 1 >= Size || !shape[x - 1, y + 1])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x - 1, position.Y + y + 1)), 1.75f * MathF.PI, effectSpeed, Color.White, 2.828f, "fire", rotationSpeed);
        if (x + 1 >= Size || y - 1 < 0 || !shape[x + 1, y - 1])
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x, position.Y + y)), 0.75f * MathF.PI, effectSpeed, Color.White, 2.828f, "fire", rotationSpeed);
    }

    /// <summary>
    /// Applying the Pushdown effect and making the grid push all the blocks under it down.
    /// </summary>
    /// <param name="x"></param> The x coordinate.
    /// <param name="y"></param> The y coordinate
    void PushDownBlockPiece(int x, int y)
    {
        if (position.Y + y >= 0)
        {
            grid.PushCellsDown(new Point(position.X + x, position.Y + y));
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x, position.Y + y)), 0f, 800f, Color.White, grid.Height - (position.Y + y));
        }
    }

    /// <summary>
    /// Applying the Pushup effect and making the grid push all the blocks under it up.
    /// </summary>
    /// <param name="x"></param> The x coordinate.
    /// <param name="y"></param> The y coordinate
    void PushUpBlockPiece(int x, int y)
    {
        if (position.Y + y >= 0)
        {
            grid.PullCellsUp((new Point(position.X + x, position.Y + y)));
            TetrisGame.EffectsManager.NewEffect(grid.GetPositionOfCell(new Point(position.X + x, grid.Height - 1)), 1f * MathF.PI, 800f, Color.White, grid.Height - (position.Y + y) - 1);
        }
    }

    /// <summary>
    /// Sets the offset back to zero.
    /// </summary>
    public void MoveToSpawnPosition()
    {
        offset = Vector2.Zero;
    }

    /// <summary>
    /// Rotating the shape clockwise by aplying this formula
    /// x = maximum y position - y
    /// y = x
    /// </summary>
    public void RotateLeft()
    {
        //requires square grids for the shapes
        bool[,] newShape = new bool[Size, Size];
        //Looping over the shape.
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                //Applying the formula.
                newShape[x, y] = shape[Size - 1 - y, x];
            }
        }
        //Setting the shape and setting it back if the move was invalid.
        bool[,] originalShape = shape;
        shape = newShape;
        if (!MoveWasValid())
            shape = originalShape;
    }

    /// <summary>
    /// Rotating the shape counter clockwise by aplying this formula
    /// x = y
    /// y = maximum x position - x
    /// </summary>
    public void RotateRight()
    {
        //requires square grids for the shapes
        bool[,] newShape = new bool[Size, Size];
        //Looping over the shape.
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                //Applying the formula.
                newShape[x, y] = shape[y, Size - 1 - x];
            }
        }
        //Setting the shape and setting it back if the move was invalid.
        bool[,] originalShape = shape;
        shape = newShape;
        if (!MoveWasValid())
            shape = originalShape;
    }
    //Moves the block to the left.
    public void MoveLeft()
    {
        position.X--;
        if (!MoveWasValid())
            position.X++;
    }
    //Moves the block to the right.
    public void MoveRight()
    {
        position.X++;
        if (!MoveWasValid())
            position.X--;
    }
    //Moves the block up.
    public void MoveUp() 
    {
        //only called when everything moves up so no extra checks needed
        position.Y--;
    }

    //moves the block down once if possible and returns false when the block is placed as the block didn't move.
    public bool MoveDown()
    {
        position.Y++;
        if (!MoveWasValid())
        {
            position.Y--;
            PlaceBlock();
            gameWorld.NewBlocks();
            return false;
        }
        return true;
    }
    //Moves the block all the way down instantly.
    public void HardDrop()
    {
        while (MoveDown()) ;
    }
    //Is overriden to set the shape and position.
    public virtual void Reset()
    {
    }
    //Sets the offset.
    public void setOffset(Vector2 offset)
    {
        int furthestX = 0;
        int furthestY = 0;

        for(int x = 0; x < Size; x++) 
        {
            for(int y = 0; y < Size; y++)
            {
                if (shape[x, y])
                {
                    furthestX = x;
                    furthestY = y;
                }
            }
        }
        this.offset = offset - new Vector2(furthestX * texture.Width, furthestY * texture.Height) / 2 - new Vector2(position.X * texture.Width, position.Y * texture.Height);
    }
}
/// <summary>
/// This is the yellow 2X2 block
/// </summary>
internal class OBlock : Block
{
    public OBlock(TetrisGrid grid, GameWorld gameWorld, BlockType type) : base(grid, gameWorld, type)
    {
        color = TetrisGrid.CellType.yellow;
        Reset();

        setOffset(new Vector2(364, 155));
    }
    public override void Reset()
    {
        position = new Point(4, -2);
        shape = new bool[,] { { true, true }, { true, true } };
    }
}
//This is the lightblue straight block
internal class IBlock : Block
{
    public IBlock(TetrisGrid grid, GameWorld gameWorld, BlockType type) : base(grid, gameWorld, type)
    {
        Reset();
        color = TetrisGrid.CellType.lightBlue;

        setOffset(new Vector2(364, 155));
    }
    public override void Reset()
    {
        position = new Point(3, -2);
        shape = new bool[,] { { false, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, true, false, false } };
    }
}
//This is the purple T shaped block
internal class TBlock : Block
{
    public TBlock(TetrisGrid grid, GameWorld gameWorld, BlockType type) : base(grid, gameWorld, type)
    {
        Reset();
        color = TetrisGrid.CellType.purple;

        setOffset(new Vector2(364, 155));
    }
    public override void Reset()
    {
        position = new Point(3, -2);
        shape = new bool[,] { { false, true, false }, { true, true, false }, { false, true, false } };
    }
}
//This is the orange L shaped block
internal class LBlock : Block
{
    public LBlock(TetrisGrid grid, GameWorld gameWorld, BlockType type) : base(grid, gameWorld, type)
    {
        Reset();
        color = TetrisGrid.CellType.orange;

        setOffset(new Vector2(364, 155));
    }

    public override void Reset()
    {
        position = new Point(3, -2);
        shape = new bool[,] { { false, true, false }, { false, true, false }, { true, true, false } };
    }
}
//This is the dark blue J shaped block
internal class JBlock : Block
{
    public JBlock(TetrisGrid grid, GameWorld gameWorld, BlockType type) : base(grid, gameWorld, type)
    {
        Reset();
        color = TetrisGrid.CellType.darkBlue;

        setOffset(new Vector2(364, 155));
    }
    public override void Reset()
    {
        position = new Point(3, -2);
        shape = new bool[,] { { true, true, false }, { false, true, false }, { false, true, false } };
    }
}
//This is the green S shaped block
internal class SBlock : Block
{
    public SBlock(TetrisGrid grid, GameWorld gameWorld, BlockType type) : base(grid, gameWorld, type)
    {
        Reset();
        color = TetrisGrid.CellType.green;

        setOffset(new Vector2(364, 155));
    }
    public override void Reset()
    {
        position = new Point(3, -2);
        shape = new bool[,] { { false, true, false }, { true, true, false }, { true, false, false } };
    }
}
//This is the red Z shaped block
internal class ZBlock : Block
{
    public ZBlock(TetrisGrid grid, GameWorld gameWorld, BlockType type) : base(grid, gameWorld, type)
    {
        Reset();
        color = TetrisGrid.CellType.red;

        setOffset(new Vector2(364, 155));
    }
    public override void Reset()
    {
        position = new Point(3, -2);
        shape = new bool[,] { { true, false, false }, { true, true, false }, { false, true, false } };
    }
}
