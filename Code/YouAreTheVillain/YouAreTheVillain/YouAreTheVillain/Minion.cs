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
    public class Minion
    {
        public bool Active = false;

        public Vector2 Position;
        public Vector2 Velocity = new Vector2(0,0);
        public Vector2 Direction = new Vector2(0, 0);

        float spawnAlpha = 0f;

        Vector2 SpawnPoint;
        Vector2 Gravity = new Vector2(0, 0.5f);

        Vector2 frameSize = new Vector2(64, 64);

        public Minion()
        { }

        public void Spawn(Vector2 loc) 
        {
            Active = true;
            spawnAlpha = 0f;
            Position = loc;
            Direction = new Vector2(-1, 0);
            Velocity = Vector2.Zero;
        }

        

        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            if (spawnAlpha < 1f) spawnAlpha += 0.1f;

            CollisionCheck();
            //JumpsCheck();

            Velocity = Vector2.Clamp(Velocity, new Vector2(-4, -15), new Vector2(4, 15));
            
           

            if (Position.Y > GameManager.Map.Height*GameManager.Map.TileHeight)
            {
                Active = false;
                //Position = SpawnPoint;
               // Velocity = new Vector2(3, 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;

            spriteBatch.Draw(GameManager.MinionManager.SpriteSheets[0], Position - GameManager.Camera.Position, null, Color.White * spawnAlpha, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
        }

        bool CollisionCheck()
        {
            bool collidedx = false;
            bool collidedy = false;

            bool changedir = false;

            var t = GameManager.Map.Layers.Where(l => l.Name == "FG").First();
            TileLayer tileLayer = t as TileLayer;

            int x, y;

            // Check left
            x=-1;
            for (y = 0; y <= 1; y++)
            {
                Point tilePos = new Point((int)((Position.X + (x * ((frameSize.X / 2))) + (x*-6)) / GameManager.Map.TileWidth), (int)((Position.Y + (y * ((frameSize.Y / 2)))) / GameManager.Map.TileHeight));

                if (tilePos.X < tileLayer.Tiles.GetLowerBound(0) || tilePos.X > tileLayer.Tiles.GetUpperBound(0)) continue;
                if (tilePos.Y < tileLayer.Tiles.GetLowerBound(1) || tilePos.Y > tileLayer.Tiles.GetUpperBound(1)) continue;

                if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                {
                    if(Velocity.X<0) collidedx = true;
                    //Velocity.X = 0;

                    if (y == 0 && Velocity.X<0) changedir = true;
                }
                
            }

            // Check right
            x = 1;
            for (y = 0; y <= 1; y++)
            {
                Point tilePos = new Point((int)((Position.X + (x * ((frameSize.X / 2))) + (x * -6)) / GameManager.Map.TileWidth), (int)((Position.Y + (y * ((frameSize.Y / 2)))) / GameManager.Map.TileHeight));

                if (tilePos.X < tileLayer.Tiles.GetLowerBound(0) || tilePos.X > tileLayer.Tiles.GetUpperBound(0)) continue;
                if (tilePos.Y < tileLayer.Tiles.GetLowerBound(1) || tilePos.Y > tileLayer.Tiles.GetUpperBound(1)) continue;

                if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                {
                    if (Velocity.X > 0) collidedx = true;
                    //Velocity.X = 0;

                    if (y == 0 && Velocity.X > 0) changedir = true;
                }

            }

            // Check down
            y = 1;
            for (x = -1; x <= 1; x++)
            {
                Point tilePos = new Point((int)((Position.X + (x * ((frameSize.X / 2))) + (x*-10)) / GameManager.Map.TileWidth), (int)((Position.Y + (y * ((frameSize.Y / 2)+2))) / GameManager.Map.TileHeight));

                if (tilePos.X < tileLayer.Tiles.GetLowerBound(0) || tilePos.X > tileLayer.Tiles.GetUpperBound(0)) continue;
                if (tilePos.Y < tileLayer.Tiles.GetLowerBound(1) || tilePos.Y > tileLayer.Tiles.GetUpperBound(1)) continue;

                if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                {
                    collidedy = true;
                    if (Velocity.Y > 0)
                    {
                        Velocity.Y = 0;
                        
                    }
                    Position.Y = ((tilePos.Y-1) * 64)+30;
                }

                

            }

            if (!collidedx)
            {
                Position.X += Velocity.X;
                Velocity += 0.5f * Direction;
            }

            if (changedir)
            {
                Direction = -Direction;
                //Position += Direction * 10f;
                Velocity.X = Direction.X * 4f;
                Position.X += Velocity.X;
            }

            if (!collidedy)
            {
                Velocity += Gravity;
                Position.Y += Velocity.Y;
            }

            return collidedx || collidedy;
        }

        void JumpsCheck()
        {
            var layer = GameManager.Map.Layers.Where(l => l.Name == "Jumps").First();
            if (layer != null)
            {
                MapObjectLayer objectlayer = layer as MapObjectLayer;

                foreach (MapObject o in objectlayer.Objects)
                {
                    if (o.Location.Contains(new Point((int)Position.X, (int)(Position.Y + (frameSize.Y)))))
                    {
                        if(MinionManager.randomNumber.Next(10)==1 || (o.Properties["MustJump"].ToLower()=="true"))
                        {
                            if(o.Type=="Full")
                                Velocity.Y=-13f;

                            if (o.Type == "Half")
                                Velocity.Y = -9f;
                         }
                    }
                }
            }
        }
    }
}
