using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;

namespace TiledLib
{
    public class Camera
    {
        public Vector2 Position;
        public int Width;
        public int Height;
        public Vector2 Target;
        public Rectangle ClampRect;

        float Speed = 0.5f;

        /// <summary>
        /// Initialise the camera, using the game map to define the boundaries
        /// </summary>
        /// <param name="vp">Graphics viewport</param>
        /// <param name="map">Game Map</param>
        public Camera(Viewport vp, Map map)
        {
            Position = new Vector2(0, 0);
            Width = vp.Width;
            Height = vp.Height;

            ClampRect = new Rectangle(0,0, map.Width * map.TileWidth, map.Height * map.TileHeight);

            if (map.Properties.Contains("CameraBoundsLeft"))
                ClampRect.X = Convert.ToInt32(map.Properties["CameraBoundsLeft"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsTop"))
                ClampRect.Y = Convert.ToInt32(map.Properties["CameraBoundsTop"]) * map.TileHeight;
            if (map.Properties.Contains("CameraBoundsWidth"))
                ClampRect.Width = Convert.ToInt32(map.Properties["CameraBoundsWidth"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsHeight"))
                ClampRect.Height = Convert.ToInt32(map.Properties["CameraBoundsHeight"]) * map.TileHeight;

            // Set initial position and target
            Position.X = ClampRect.X;
            Position.Y = ClampRect.Y;
            Target = new Vector2(ClampRect.X, ClampRect.Y);
        }

        /// <summary>
        /// Update the camera
        /// </summary>
        /// 
        public void Update(Viewport vp)
        {
            Update(Speed, vp);
        }
        public void Update(float speed, Viewport vp)
        {
            Width = vp.Width;
            Height = vp.Height;

            // Clamp target to map/camera bounds
            Target.X = MathHelper.Clamp(Target.X, ClampRect.X, ClampRect.Width - Width);
            Target.Y = MathHelper.Clamp(Target.Y, ClampRect.Y, ClampRect.Height - Height);

            Position.X = MathHelper.Clamp(Position.X, ClampRect.X, ClampRect.Width - Width);
            Position.Y = MathHelper.Clamp(Position.Y, ClampRect.Y, ClampRect.Height - Height);
            
            // Move camera toward target
            Position = Vector2.SmoothStep(Position, Target, speed);
        }
    }
}
