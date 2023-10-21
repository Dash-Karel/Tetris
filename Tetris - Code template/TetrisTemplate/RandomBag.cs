using System.Collections.Generic;

/// <summary>
/// //Random bag is a class used to create a queue of random blocks of 7 that contains all blocks and resets every 7.
/// This is to make the game more predictable and to enable more strategy.
/// </summary>
internal class RandomBag
{
    //used as a reference to a gameWorld so it can be passed on to the constructor of each block
    GameWorld gameWorld;

    //a pointer that indicates at which position in the bag array the NextBlock method should look
    int bagPointer;

    //an array that stores ints that indicate a block type. It is filled with each block type at a random position
    int[] bag;


    //a list that stores ints, which again indicate a block type, to help fill in the bag array.
    List<int> choicePool;


    //The chance that a special block can spawn in percentages
    const float specialBlockChance = 75;

    public RandomBag(GameWorld gameWorld)
    {
        //store the reference to a gameWorld
        this.gameWorld = gameWorld;
        
        //initiliase the array and list with a size of 7
        bag = new int[7];
        choicePool = new List<int>(7);

        //make sure the bag is full and can be used
        Refill();
    }

    public Block NextBlock(TetrisGrid grid)
    {
        //Refill the bag if all elements have been used
        if (bagPointer >= bag.Length)
            Refill();

        //Determine whether a special block should be spawned by checking if special block mode is active and comparing a random value to the specialBlockChance and then pick a random special type
        Block.BlockType type = Block.BlockType.normal;
        if (TetrisGame.UseSpecialBlocks && GameWorld.Random.NextSingle() * 100f < specialBlockChance)
            type = (Block.BlockType)(GameWorld.Random.Next(3) + 1);

        //return the block based on the value in the bag and then increment the bag pointer to point to the next value
        switch (bag[bagPointer++])
        {
            case 0:
                return new OBlock(grid, gameWorld, type);
            case 1:
                return new IBlock(grid, gameWorld, type);
            case 2:
                return new TBlock(grid, gameWorld, type);
            case 3:
                return new LBlock(grid, gameWorld, type);
            case 4:
                return new JBlock(grid, gameWorld, type);
            case 5:
                return new SBlock(grid, gameWorld, type);
            case 6:
                return new ZBlock(grid, gameWorld, type);
            default:
                return null;
        }
    }
    void Refill()
    {
        //Refill the choice pool with values to choose from
        for (int i = 0; i < 7; i++)
        {
            choicePool.Add(i);
        }

        //Fill the bag randomly without duplicates, this is done by picking a random inex of the choice pool and storing that value in bag
        //At this point it is the same as getting a random value between 0 and 6
        //Next that value is removed from the choice pool so it can't be selected again, to avoid duplicates
        //After a while every value of the choice pool is removed and stored in the bag at a random position, thus achieving the effect of a queue with exactly one of every block in a random order.
        for (int i = 0; i < bag.Length; i++)
        {
            int randomIndex = GameWorld.Random.Next(0, choicePool.Count);
            bag[i] = choicePool[randomIndex];
            choicePool.RemoveAt(randomIndex);
        }

        //reset the bag pointer
        bagPointer = 0;
    }
}
