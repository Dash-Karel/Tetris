using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// EffectsManager takes care of updating and drawing all current effects, it contains methods to create and remove effects
/// </summary>
internal class EffectsManager
{
    //A list used to store all current Effect objects
    List<Effect> effects = new List<Effect>();

    //A list used to keep track of all the elements in the effects list that need to be removed
    List<Effect> toBeRemoved = new List<Effect>();

    /// <summary>
    /// Updates all the effects in the effects list and removes any effects that were marked to be removed
    /// </summary>
    /// <param name="gameTime">An object with information about the time that has passed in the game.</param>
    public void Update(GameTime gameTime)
    {
        //loops through all the effects and updates each of them
        foreach (Effect effect in effects)
        {
            effect.Update(gameTime);
        }
        
        //removes the effects that were marked to be removed
        foreach (Effect effect in toBeRemoved)
        {
            effects.Remove(effect);
        }
    }

    /// <summary>
    /// Draws all the effects in the effects list
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing sprites and text.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Effect effect in effects)
        {
            effect.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// Creates a new effect and add it to the list of effects
    /// </summary>
    /// <param name="position"></param>The position at which the effect will start
    /// <param name="direction"></param>The direction the efect will move in and be rotated towards. Measured in radians. With 0 indicating downwards, 0.5 PI right, 1 PI up etc.
    /// <param name="speed"></param>The speed at which the effect will move, measured in world units per second
    /// <param name="color"></param> The color that gets applied to the effect
    /// <param name="distanceToLive"></param> The distance the effect will exist for, measured in grid units
    /// <param name="textureName"></param> The name of the texture the effect uses, "whoosh" by default
    /// <param name="rotationSpeed"></param> The speed the effects will rotate at, 0 by default, measured in radians per second
    public void NewEffect(Vector2 position, float direction, float speed, Color color, float distanceToLive = 20, string textureName = "whoosh", float rotationSpeed = 0f)
    {
        effects.Add(new Effect(position, direction, speed, color, distanceToLive, textureName, rotationSpeed));
    }
    
    /// <summary>
    /// marks an effect to be removed from the list
    /// </summary>
    /// <param name="effect"></param>the effect that will be removed
    public void RemoveEffect(Effect effect)
    {
        //adds the effect to the list of Effects that will be removed
        toBeRemoved.Add(effect);
    }
    /// <summary>
    /// Reset the effect manager, meaning all effects will be remvoved
    /// </summary>
    public void Reset()
    {
        //removes all effect from the list
        effects = new List<Effect>();
    }
}
