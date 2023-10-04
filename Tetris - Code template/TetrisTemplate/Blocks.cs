using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisTemplate
{
    internal class Block
    {
        Texture2D texture;

        protected Vector2 position;
        protected Color color;
        protected bool[,] shape;

        int Size
        {
            get { return shape.GetLength(0); }
        }

        //final version protected
        protected Block(TetrisGrid grid)
        {
            texture = TetrisGame.ContentManager.Load<Texture2D>("block");
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
            shape = newShape;
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
            shape = newShape;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int width = 0; width < Size; width++)
            {
                for (int height = 0; height < Size; height++)
                {
                    if (shape[width, height] == true)
                        spriteBatch.Draw(texture, position + new Vector2(width * texture.Width, height * texture.Height), color);
                }
            }
        }
    }
    internal class OBlock : Block
    {
        public OBlock(TetrisGrid grid) : base(grid)
        {
            shape = new bool[,] { { true, true }, { true, true } };
            color = Color.Yellow;
            position = grid.GetPositionOfCell(4, -2);
        }
    }
    internal class IBlock : Block
    {
        public IBlock(TetrisGrid grid) : base(grid)
        {
            shape = new bool[,] { { false, true, false, false}, { false, true, false, false}, { false, true, false, false}, { false, true, false, false } };
            color = Color.LightBlue;
            position = grid.GetPositionOfCell(3, -2);
        }
    }
    internal class TBlock : Block
    {
        public TBlock(TetrisGrid grid) : base(grid)
        {
            shape = new bool[,] { { false, true, false}, { true, true, false}, { false, true, false} };
            color = Color.Purple;
            position = grid.GetPositionOfCell(3, -2);
        }
    }
    internal class LBlock : Block
    {
        public LBlock(TetrisGrid grid) : base(grid)
        {
            shape = new bool[,] { { false, true, false }, { false, true, false }, { true, true, false } };
            color = Color.Orange;
            position = grid.GetPositionOfCell(3, -2);
        }
    }
    internal class JBlock : Block
    {
        public JBlock(TetrisGrid grid) : base(grid)
        {
            shape = new bool[,] { { true, true, false }, { false, true, false }, { false, true, false } };
            color = Color.DarkBlue;
            position = grid.GetPositionOfCell(3, -2);
        }
    }
    internal class SBlock : Block
    {
        public SBlock(TetrisGrid grid) : base(grid)
        {
            shape = new bool[,] { { false, true, false }, { true, true, false }, { true, false, false } };
            color = Color.Green;
            position = grid.GetPositionOfCell(3, -2);
        }
    }
    internal class ZBlock : Block
    {
        public ZBlock(TetrisGrid grid) : base(grid)
        {
            shape = new bool[,] { { true, false, false }, { true, true, false }, { false, true, false } };
            color = Color.Red;
            position = grid.GetPositionOfCell(3, -2);
        }
    }
}
