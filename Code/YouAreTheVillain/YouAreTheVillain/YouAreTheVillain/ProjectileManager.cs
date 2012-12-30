using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;

namespace YouAreTheVillain
{
    public class Projectile
    {
        public Vector2 Position;
        public Vector2 Speed;
        public bool OwnedByHero;
        public int Type;
        public bool Active;
        public bool StuckInWall = false;

        public void Spawn(Vector2 pos, Vector2 speed, bool heroowner, int type)
        {
            Active = true;
            Position = pos;
            Speed = speed;
            OwnedByHero = heroowner;
            Type = type;
        }
    }

    public class ProjectileManager
    {
        const int MAX_PROJECTILES = 500;

        public static Random randomNumber = new Random();
        
        public List<Projectile> Projectiles = new List<Projectile>();

        public Texture2D spriteSheet;

        Vector2 frameSize = new Vector2(64, 64);

        public ProjectileManager()
        {
            Initialize();
        }

        public void Initialize()
        {
            for (int i = 0; i < MAX_PROJECTILES; i++)
                Projectiles.Add(new Projectile());
        }

        public void LoadContent(ContentManager content)
        {
            spriteSheet = content.Load<Texture2D>("projectiles");
            
        }

        public void Add(Vector2 loc, Vector2 speed, bool ownerhero, int type)
        {
            foreach (Projectile p in Projectiles)
            {
                if (!p.Active && !p.StuckInWall)
                {
                    p.Spawn(loc, speed, ownerhero, type);
                    break;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            var t = GameManager.Map.Layers.Where(l => l.Name == "FG").First();
            TileLayer tileLayer = t as TileLayer;

            foreach (Projectile p in Projectiles)
            {
                if (p.Active)
                {
                    p.Position += p.Speed;
                    Point tilePos = new Point((int)((p.Position.X + (p.Speed.X * 2f)) / GameManager.Map.TileWidth), (int)((p.Position.Y) / GameManager.Map.TileHeight));
                    if (tilePos.X >= tileLayer.Tiles.GetLowerBound(0) && tilePos.X <= tileLayer.Tiles.GetUpperBound(0) &&
                        tilePos.Y >= tileLayer.Tiles.GetLowerBound(1) && tilePos.Y <= tileLayer.Tiles.GetUpperBound(1))
                    {
                        if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                        {
                            p.Active = false;
                            p.StuckInWall = true;
                        }
                    }

                    // do collision checks
                    if (p.OwnedByHero)
                    {
                        foreach (Minion m in GameManager.MinionManager.Minions)
                        {
                            if(m.Active && m.spawnAlpha>=1f)
                            {
                                if ((m.Position - p.Position).Length() < 32f)
                                {
                                    m.Active = false;
                                    p.Active = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Projectile p in Projectiles)
            {
                if(p.Active || p.StuckInWall)
                    spriteBatch.Draw(spriteSheet, p.Position - GameManager.Camera.Position, new Rectangle(p.Type * (int)frameSize.X, 0, (int)frameSize.X, (int)frameSize.Y), Color.White, 0f, frameSize / 2, 1f, SpriteEffects.None, 1);
            }
        }
    }
}
