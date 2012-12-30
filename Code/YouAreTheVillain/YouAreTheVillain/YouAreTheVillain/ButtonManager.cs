using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouAreTheVillain
{
    public class Button
    {
        public double CurrentCoolDown = 0;
        public double CoolDown = 1000;
        public int MinionType;

        public Button(int type, double cd)
        {
            MinionType = type;
            CoolDown = cd;
            CurrentCoolDown = cd;
        }

        public void Update(GameTime gameTime)
        {
            if (CoolDown > 0) CoolDown -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }

    public class ButtonManager
    {
        public List<Button> Buttons = new List<Button>();

        public Vector2 Position;

        Vector2 buttonSize = new Vector2(96, 96);

        public ButtonManager()
        {
            Initialize();
        }

        public void Initialize()
        {
            Buttons.Add(new Button(0, 1000));
            Buttons.Add(new Button(1, 3000));
            Buttons.Add(new Button(2, 5000));
            Buttons.Add(new Button(3, 10000));

            Position = new Vector2((GameManager.Camera.Width / 2) - ((Buttons.Count*buttonSize.X)/2), GameManager.Camera.Height - buttonSize.Y);
        }

        public void LoadContent(ContentManager content)
        {
           
        }

        public void Update(GameTime gameTime)
        {
            foreach (Button b in Buttons)
                b.Update(gameTime);
        }

        public bool HandleInput(Vector2 tapPos)
        {
            bool handled = false;

            return handled;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPos = Position;
            foreach (Button b in Buttons)
            {
                spriteBatch.Draw(GameManager.MinionManager.SpriteSheets[b.MinionType], drawPos + (buttonSize / 2), new Rectangle(0, 0, 64, 64), Color.White, 0f, new Vector2(32,32), 1f, SpriteEffects.None, 1);
                drawPos += new Vector2(buttonSize.X, 0);
            }
        }
    }
}
