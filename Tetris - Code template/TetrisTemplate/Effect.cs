
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TetrisTemplate;

internal class Effect
{
    EffectsManager effectManager;

    Vector2 position;
    Vector2 velocity;
    Vector2 origin;

    //direction as a rotation in radials
    float rotation;
    float rotationSpeed;

    float distanceTraveled;
    float distanceToLive;

    Texture2D texture;
    
    public Effect(Vector2 position, float direction, float speed, EffectsManager effectManager, float distanceToLive = 20, string textureName = "whoosh", float rotationSpeed = 0)
    {
        texture = TetrisGame.ContentManager.Load<Texture2D>(textureName);
        origin = new Vector2(texture.Width, texture.Height) / 2;

        this.effectManager = effectManager;

        velocity = new Vector2(MathF.Sin(direction), MathF.Cos(direction));
        velocity.Normalize();
        velocity *= speed;
        this.position = position;

        rotation = direction;
        this.rotationSpeed = rotationSpeed;

        distanceTraveled = 0;
        this.distanceToLive = distanceToLive;
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        position += velocity * deltaTime;
        rotation += rotationSpeed * deltaTime;

        distanceTraveled += velocity.Length() * deltaTime / texture.Width;

        if(distanceTraveled > distanceToLive)
        {
            effectManager.RemoveEffect(this);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position + origin, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
    }

}

