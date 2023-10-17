using System.Collections.Generic;
using System.Linq;

namespace TetrisTemplate
{
    internal class RandomBag
    {
        GameWorld gameWorld;

        byte bagPointer;

        byte[] bag;

        List<byte> choicePool;

        public RandomBag(GameWorld gameWorld)
        {
            this.gameWorld = gameWorld;
            bag = new byte[7];
            choicePool = new List<byte>(7);
            Refill();
        }
        public Block NextBlock(TetrisGrid grid)
        {
            //Refill the bag if all elements have been used
            if (bagPointer >= bag.Length)
                Refill();

            //return the block based on the value in the bag and then increment the bag pointer to point to the next value
            switch(bag[bagPointer++])
            {
                case 0:
                    return new OBlock(grid, gameWorld);
                case 1:
                    return new IBlock(grid, gameWorld);
                case 2:
                    return new TBlock(grid, gameWorld);
                case 3:
                    return new LBlock(grid, gameWorld);
                case 4:
                    return new JBlock(grid, gameWorld);
                case 5:
                    return new SBlock(grid, gameWorld);
                case 6:
                    return new ZBlock(grid, gameWorld);
                default:
                    return null;
            }
        }
        void Refill()
        {
            //Refill pool with values to choose from
            for (byte i = 0; i < 7; i++)
            {
                choicePool.Add(i);
            }

            //fill the bag randomly without duplicates
            for (byte i = 0; i < bag.Length; i++) 
            {
                int randomIndex = GameWorld.Random.Next(0, choicePool.Count);
                bag[i] = choicePool.ElementAt(randomIndex);
                choicePool.RemoveAt(randomIndex);
            }

            //reset the bag pointer
            bagPointer = 0;
        }
    }
}
