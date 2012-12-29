using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using TiledLib;

namespace YouAreTheVillain
{
    public class Hero
    {
        public Vector2 Position;
        public Vector2 Velocity = new Vector2(3,0);

        Vector2 SpawnPoint;
        Vector2 Gravity = new Vector2(0, 0.5f);

        Vector2 frameSize = new Vector2(64, 64);
        Texture2D spriteSheet;

        public Hero()
        { }

        public void Initialize() 
        {
            // Try to find a spawn point
            var layer = GameManager.Map.Layers.Where(l => l.Name == "Spawn").First();
            if (layer!=null)
            {
                MapObjectLayer objectlayer = layer as MapObjectLayer;

                foreach (MapObject o in objectlayer.Objects)
                {
                    SpawnPoint = new Vector2(o.Location.Center.X, o.Location.Center.Y);
                    Position = SpawnPoint;
                }
            }
        }

        public void LoadContent(ContentManager content)
        {
            spriteSheet = content.Load<Texture2D>("hero");
        }

        public void Update(GameTime gameTime)
        {
            Velocity += Gravity;

            CollisionCheck();
            Position += Velocity;

            if (Position.Y > GameManager.Map.Height*GameManager.Map.TileHeight)
            {
                Position = SpawnPoint;
                Velocity = new Vector2(3, 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, Position - GameManager.Camera.Position, null, Color.White, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
        }

        bool CollisionCheck()
        {
            bool collided = false;

            var t = GameManager.Map.Layers.Where(l => l.Name == "FG").First();
            TileLayer tileLayer = t as TileLayer;

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Point tilePos = new Point((int)((Position.X + (x * (frameSize.X / 2))) / GameManager.Map.TileWidth), (int)((Position.Y + (y * (frameSize.Y / 2))) / GameManager.Map.TileHeight));

                    if (tilePos.X < tileLayer.Tiles.GetLowerBound(0) || tilePos.X > tileLayer.Tiles.GetUpperBound(0)) continue;
                    if (tilePos.Y < tileLayer.Tiles.GetLowerBound(1) || tilePos.Y > tileLayer.Tiles.GetUpperBound(1)) continue;

                    if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                    {
                        collided = true;
                        if (x > 0 && y==0 && Velocity.X > 0) Velocity.X = 0;
                        if (x < 0 && y== 0 && Velocity.X < 0) Velocity.X = 0;
                        if (y > 0 && Velocity.Y > 0) Velocity.Y = 0;
                        if (y < 0 && Velocity.Y < 0) Velocity.Y = 0;
                    }
                }
            }

            return collided;
        }
    }
}
