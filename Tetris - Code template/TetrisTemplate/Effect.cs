
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Effect is used to make visual effects consisting off a texture that travels in a specified direction with additional optionls
/// </summary>
internal class Effect
{
    //a Vector2 storing the current position in world units off the effect object
    Vector2 position;

    // a Vector2 storing the current velocity off the effect object, the length of the Vector is the speed in world units per second
    Vector2 velocity;

    //a Vector2 storing the origin around which the texture should be rotated
    Vector2 origin;

    //a Color storing which color the texture should be draw with
    Color color;

    //the rotation stores the current rotation of the object in radials
    float rotation;

    //the rotation speed in radials per secon
    float rotationSpeed;

    //a float storing the amount of world units the effect has traveled
    float distanceTraveled;

    //a float storing the maximum amount of world units an effect is allowed to travel
    float distanceToLive;

    //the texture of the effect
    Texture2D texture;

    /// <summary>
    /// The constructor of Effect, initiliases all variables
    /// </summary>
    /// <param name="position">The position in world units at which the effect will start</param>
    /// <param name="direction">The direction the efect will move in and be rotated towards. Measured in radians. With 0 indicating downwards, 0.5 PI right, 1 PI up etc.</param>
    /// <param name="speed">The speed at which the effect will move, measured in world units per second</param>
    /// <param name="color">The color that gets applied to the texture of the effect</param>
    /// <param name="distanceToLive">The distance the effect will exist for, measured in grid units, 20 by default</param>
    /// <param name="textureName">The name of the texture the effect uses, "whoosh" by default</param>
    /// <param name="rotationSpeed">The speed the effects will rotate at, 0 by default, measured in radians per second</param>
    public Effect(Vector2 position, float direction, float speed, Color color, float distanceToLive = 20, string textureName = "whoosh", float rotationSpeed = 0)
    {
        texture = TetrisGame.ContentManager.Load<Texture2D>(textureName);
        origin = new Vector2(texture.Width, texture.Height) / 2;

        velocity = new Vector2(MathF.Sin(direction), MathF.Cos(direction));
        velocity.Normalize();
        velocity *= speed;
        this.position = position;

        rotation = direction;
        this.rotationSpeed = rotationSpeed;

        distanceTraveled = 0;
        this.distanceToLive = distanceToLive;

        this.color = color;
    }

    /// <summary>
    /// Updates the effect
    /// </summary>
    /// <param name="gameTime">An object with information about the time that has passed in the game.</param> 
    public void Update(GameTime gameTime)
    {
        //helper variable storing the elapsed game time
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        //updates the position and rotation by adding the velocity and rotationspeed
        position += velocity * deltaTime;
        rotation += rotationSpeed * deltaTime;

        //increases the total distancy traveled by the amount of world units traveled divided by the size of one grid cell
        distanceTraveled += velocity.Length() * deltaTime / texture.Width;

        //makes sure the effect gets removed when the effect has traveled further than it is allowed to
        if(distanceTraveled > distanceToLive)
        {
            TetrisGame.EffectsManager.RemoveEffect(this);
        }
    }

    /// <summary>
    /// Draws the texture
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing the effect</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position + origin, null, color, rotation, origin, 1f, SpriteEffects.None, 0);
    }

}

