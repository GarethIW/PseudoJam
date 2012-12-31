using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouAreTheVillain
{
    public class ParallaxLayer
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float ScrollSpeed;

        public ParallaxLayer(Texture2D tex, Vector2 pos, float speed)
        {
            Texture = tex;
            Position = pos;
            ScrollSpeed = speed;
        }
    }

    public class ParallaxManager
    {
        public List<ParallaxLayer> Layers = new List<ParallaxLayer>();

        Viewport viewport;

        Vector2 scrollPosition;

        public ParallaxManager(Viewport vp)
        {
            viewport = vp;
        }

        public void Update(GameTime gameTime, Vector2 scrollPos)
        {
            scrollPosition = scrollPos;

            foreach (ParallaxLayer l in Layers)
            {
                l.Position.X = scrollPos.X * l.ScrollSpeed;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (ParallaxLayer l in Layers)
            {
                
                spriteBatch.Draw(l.Texture, l.Position, null, Color.White);
                  
            }
        }
    }
}
