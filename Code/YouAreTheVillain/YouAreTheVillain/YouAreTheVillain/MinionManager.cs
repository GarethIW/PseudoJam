using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouAreTheVillain
{
    public class MinionManager
    {
        const int MAX_MINIONS = 50;

        public static Random randomNumber = new Random();
        
        public List<Minion> Minions = new List<Minion>();

        public List<Texture2D> SpriteSheets = new List<Texture2D>();

        public MinionManager()
        {
            Initialize();
        }

        public void Initialize()
        {
            for (int i = 0; i < MAX_MINIONS; i++)
                Minions.Add(new Minion());
        }

        public void LoadContent(ContentManager content)
        {
            SpriteSheets.Add(content.Load<Texture2D>("minion1"));
        }

        public void Add(Vector2 loc)
        {
            foreach (Minion m in Minions)
            {
                if (!m.Active)
                {
                    m.Spawn(loc);
                    break;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Minion m in Minions)
                m.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Minion m in Minions)
                m.Draw(spriteBatch);
        }
    }
}