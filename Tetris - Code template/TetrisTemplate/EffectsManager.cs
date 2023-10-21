using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisTemplate
{
    internal class EffectsManager
    {
        List<Effect> effects = new List<Effect>();
        List<Effect> toBeRemoved = new List<Effect>();

        public void Update(GameTime gameTime)
        {
            foreach (Effect effect in effects) 
            {
                effect.Update(gameTime);
            }
            foreach (Effect effect in toBeRemoved)
            {
                effects.Remove(effect);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Effect effect in effects)
            {
                effect.Draw(spriteBatch);
            }
        }

        public void NewEffect(Vector2 position, float direction, float speed, Color color, float distanceToLive = 20, string textureName = "whoosh", float rotationSpeed = 0f)
        {
            effects.Add(new Effect(position, direction, speed, this, color, distanceToLive, textureName, rotationSpeed));
        }

        public void RemoveEffect(Effect effect)
        {
            toBeRemoved.Add(effect);
        }
        public void Reset()
        {
            effects = new List<Effect>();
        }
    }
}
