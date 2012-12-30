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
        static Random randomNumber = new Random();

        public Vector2 Position;
        public Vector2 Velocity = new Vector2(0,0);

        public int HP = 6;
        public float painAlpha = 0f;

        public double SpawnTime;
        float spawnAlpha = 0f;

        double animTime = 100;
        double currentFrameTime = 0;
        int animFrame = 1;
        int numFrames = 3;
        bool onGround = true;

        Vector2 SpawnPoint;
        Vector2 Gravity = new Vector2(0, 0.5f);

        Vector2 frameSize = new Vector2(64, 64);
        Texture2D spriteSheet;


        public Hero()
        { }

        public void Initialize() 
        {
            Vector2 chosenSpawn = Vector2.Zero;
            // Try to find a spawn point
            var layer = GameManager.Map.Layers.Where(l => l.Name == "Spawn").First();
            if (layer!=null)
            {
                MapObjectLayer objectlayer = layer as MapObjectLayer;

                foreach (MapObject o in objectlayer.Objects)
                {
                    if (chosenSpawn.X < o.Location.Center.X) chosenSpawn = new Vector2(o.Location.Center.X, o.Location.Center.Y);
                    
                }

                SpawnPoint = chosenSpawn;
                Position = SpawnPoint;
                Position.X = 0;
                spawnAlpha = 0f;
                SpawnTime = 4000;
            }
        }

        public void Respawn()
        {
            Vector2 chosenSpawn = Vector2.Zero;
            float furthestX = 0f;

            var layer = GameManager.Map.Layers.Where(l => l.Name == "Spawn").First();
            if (layer != null)
            {
                MapObjectLayer objectlayer = layer as MapObjectLayer;

                foreach (MapObject o in objectlayer.Objects)
                {
                    Vector2 pos = new Vector2(o.Location.Center.X, o.Location.Center.Y);

                    if (pos.X <= Position.X && pos.X>furthestX)
                    {
                        furthestX = pos.X;
                        chosenSpawn = pos;
                    }
                }

                SpawnPoint = chosenSpawn;
                Position = SpawnPoint;
                spawnAlpha = 0f;
            }
        }

        public void LoadContent(ContentManager content)
        {
            spriteSheet = content.Load<Texture2D>("hero");
        }

        public void Update(GameTime gameTime)
        {
            if (SpawnTime > 0)
            {
                SpawnTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }

            if (spawnAlpha < 1f) spawnAlpha += 0.1f;
            if (painAlpha > 0f) painAlpha -= 0.01f;

            CollisionCheck();

            if (HP <= 0)
            {
                Velocity.X = 0;
                return;
            }

            // Anim
            currentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (currentFrameTime >= animTime)
            {
                currentFrameTime = 0;
                animFrame++;
                if (animFrame > numFrames) animFrame = 1;
            }

            JumpsCheck();
            Combat();

            Velocity = Vector2.Clamp(Velocity, new Vector2(-3.5f, -15), new Vector2(3.5f, 15));
           

            if (Position.Y > GameManager.Map.Height*GameManager.Map.TileHeight)
            {
                Velocity = Vector2.Zero;
                Respawn();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (HP <= 0)
            {
                spriteBatch.Draw(spriteSheet, (Position + new Vector2(0,5)) - GameManager.Camera.Position, new Rectangle(4 * (int)frameSize.X, 0, (int)frameSize.X, (int)frameSize.Y), Color.White, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
                return;
            }

            if (onGround)
            {
                spriteBatch.Draw(spriteSheet, (Position + new Vector2(0, 6)) - GameManager.Camera.Position, new Rectangle(animFrame * (int)frameSize.X, 0, (int)frameSize.X, (int)frameSize.Y), Color.White * spawnAlpha, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
                if (painAlpha > 0f)
                {
                    spriteBatch.Draw(spriteSheet, (Position + new Vector2(0, 6)) - GameManager.Camera.Position, new Rectangle(animFrame * (int)frameSize.X, 64, (int)frameSize.X, (int)frameSize.Y), Color.White * painAlpha, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
                }
            }
            else
            {
                spriteBatch.Draw(spriteSheet, (Position + new Vector2(0, 6)) - GameManager.Camera.Position, new Rectangle(0 * (int)frameSize.X, 0, (int)frameSize.X, (int)frameSize.Y), Color.White * spawnAlpha, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
                if (painAlpha > 0f)
                {
                    spriteBatch.Draw(spriteSheet, (Position + new Vector2(0, 6)) - GameManager.Camera.Position, new Rectangle(0 * (int)frameSize.X, 64, (int)frameSize.X, (int)frameSize.Y), Color.White * painAlpha, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
                }
            }
        }

        void Combat()
        {
            // Check collision
            foreach (Minion m in GameManager.MinionManager.Minions)
            {
                if ((Position - m.Position).Length() < 64)
                {
                    if (painAlpha <= 0f)
                    {
                        HP -= 1;
                        painAlpha = 1f;
                    }
                }
            }
        }

        bool CollisionCheck()
        {
            bool collidedx = false;
            bool collidedy = false;

            var t = GameManager.Map.Layers.Where(l => l.Name == "FG").First();
            TileLayer tileLayer = t as TileLayer;

            int x, y;

            // Check left
            x=-1;
            for (y = 0; y <= 1; y++)
            {
                Point tilePos = new Point((int)((Position.X + (x * ((frameSize.X / 2)))) / GameManager.Map.TileWidth), (int)((Position.Y + (y * ((frameSize.Y / 2)))) / GameManager.Map.TileHeight));

                if (tilePos.X < tileLayer.Tiles.GetLowerBound(0) || tilePos.X > tileLayer.Tiles.GetUpperBound(0)) continue;
                if (tilePos.Y < tileLayer.Tiles.GetLowerBound(1) || tilePos.Y > tileLayer.Tiles.GetUpperBound(1)) continue;

                if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                {
                    if (Velocity.X < 0) collidedx = true;
                    //Velocity.X = 0;
                }
                
            }

            // Check right
            x = 1;
            for (y = 0; y <= 1; y++)
            {
                Point tilePos = new Point((int)((Position.X + (x * ((frameSize.X / 2)))) / GameManager.Map.TileWidth), (int)((Position.Y + (y * ((frameSize.Y / 2)))) / GameManager.Map.TileHeight));

                if (tilePos.X < tileLayer.Tiles.GetLowerBound(0) || tilePos.X > tileLayer.Tiles.GetUpperBound(0)) continue;
                if (tilePos.Y < tileLayer.Tiles.GetLowerBound(1) || tilePos.Y > tileLayer.Tiles.GetUpperBound(1)) continue;

                if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                {
                    if (Velocity.X > 0) collidedx = true;
                    //Velocity.X = 0;
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
                    Position.Y -= 1f;
                }
               
            }

            for (x = -1; x <= 1; x++)
            {
                Point tileP = new Point((int)((Position.X + (x * ((frameSize.X / 2)))) / GameManager.Map.TileWidth), (int)((Position.Y + (((frameSize.Y / 2) + 5))) / GameManager.Map.TileHeight));
                if (tileP.X >= tileLayer.Tiles.GetLowerBound(0) && tileP.X <= tileLayer.Tiles.GetUpperBound(0) &&
                    tileP.Y >= tileLayer.Tiles.GetLowerBound(1) && tileP.Y <= tileLayer.Tiles.GetUpperBound(1))
                {
                    if (tileLayer.Tiles[tileP.X, tileP.Y] == null)
                    {
                        onGround = false;
                    }
                    else onGround = true;
                }
            }

            if (!collidedx)
            {
                Position.X += Velocity.X;
                if (Velocity.X < 4) Velocity.X += 0.5f;
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
                    if (o.Location.Contains(new Point((int)Position.X, (int)(Position.Y + (frameSize.Y/2)))))
                    {
                        if(randomNumber.Next(10)==1 || (o.Properties["MustJump"].ToLower()=="true"))
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
