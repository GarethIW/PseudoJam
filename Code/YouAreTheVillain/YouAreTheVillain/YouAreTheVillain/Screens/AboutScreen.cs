#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace YouAreTheVillain
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    public class AboutScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D buttonBG;
        List<Texture2D> minions = new List<Texture2D>();


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "YouAreTheVillainContent");

            buttonBG = content.Load<Texture2D>("buttons");
            minions.Add(content.Load<Texture2D>("minion1"));
            minions.Add(content.Load<Texture2D>("minion2"));
            minions.Add(content.Load<Texture2D>("minion3"));
            minions.Add(content.Load<Texture2D>("minion4"));
            minions.Add(content.Load<Texture2D>("minion5"));
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex pi;
            if(input.TapPosition.HasValue || input.IsMenuCancel(null, out pi) || input.IsNewButtonPress(Buttons.Back, null, out pi))
                ExitScreen();

            base.HandleInput(input);
        }




        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            Vector2 centerVect = new Vector2(fullscreen.Width/2,0);



            ScreenManager.FadeBackBufferToBlack(TransitionAlpha);

            spriteBatch.Begin();

            spriteBatch.DrawString(ScreenManager.Font, "You Are The Villain", centerVect + new Vector2(0, 50), Color.Yellow * TransitionAlpha, 0f, 
                                   ScreenManager.Font.MeasureString("You Are The Villain")/2, 1f, SpriteEffects.None,1);
            spriteBatch.DrawString(ScreenManager.Font, "By Paul Yendley and Gareth Williams", centerVect + new Vector2(0, 75), Color.Red * TransitionAlpha, 0f,
                                   ScreenManager.Font.MeasureString("By Paul Yendley and Gareth Williams") / 2, 1f, SpriteEffects.None, 1);

            spriteBatch.DrawString(ScreenManager.Font, "This game was created in 48 hours", centerVect + new Vector2(0, 120), Color.LightGray * TransitionAlpha, 0f,
                                   ScreenManager.Font.MeasureString("This game was created in 48 hours") / 2, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Between 29.12.2012 and 31.12.2012", centerVect + new Vector2(0, 145), Color.LightGray * TransitionAlpha, 0f,
                                   ScreenManager.Font.MeasureString("Between 29.12.2012 and 31.12.2012") / 2, 1f, SpriteEffects.None, 1);

            spriteBatch.DrawString(ScreenManager.Font, "How to Play", centerVect + new Vector2(0, 190), Color.Magenta * TransitionAlpha, 0f,
                                   ScreenManager.Font.MeasureString("How to Play") / 2, 1f, SpriteEffects.None, 1);

            spriteBatch.DrawString(ScreenManager.Font, "You are the Villain. Stop the hero from reaching the Princess\nby placing your minions throughout the level. Click and drag\nto navigate the level.", centerVect + new Vector2(0, 240), Color.LightGray * TransitionAlpha, 0f,
                                   ScreenManager.Font.MeasureString("You are the Villain. Stop the hero from reaching the Princess\nby placing your minions throughout the level.  Click and drag\nto navigate the level.") / 2, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Each Minion has a cooldown time. Click a button to select minion\nor use keys 1..4. Click on or above a platform to place a minion.", centerVect + new Vector2(0, 325), Color.LightGray * TransitionAlpha, 0f,
                                   ScreenManager.Font.MeasureString("Each Minion has a cooldown time. Click a button to select minion\nor use keys 1..4. Click on or above a platform to place a minion.") / 2, 1f, SpriteEffects.None, 1);

            centerVect.X -= 200;

            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(buttonBG, centerVect + new Vector2(-300, 380 + (i * 64)), new Rectangle(0, 0, 96, 96), Color.White * TransitionAlpha, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1);
                spriteBatch.Draw(minions[i], centerVect + new Vector2(-263, 415 + (i * 64)) + new Vector2(3,3), new Rectangle(0, 0, 64, 64), Color.Black * 0.4f* TransitionAlpha, 0f, new Vector2(32, 32), 0.7f, SpriteEffects.None, 1);
                spriteBatch.Draw(minions[i], centerVect + new Vector2(-263, 415 + (i * 64)), new Rectangle(0, 0, 64, 64), Color.White * TransitionAlpha, 0f, new Vector2(32, 32), 0.7f, SpriteEffects.None, 1);
            }

            spriteBatch.DrawString(ScreenManager.Font, "Kamikaze", centerVect + new Vector2(-220, 380), Color.Magenta * TransitionAlpha, 0f,
                                   Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Patroller", centerVect + new Vector2(-220, 380+64), Color.Magenta * TransitionAlpha, 0f,
                                   Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Fire Sorceress", centerVect + new Vector2(-220, 380+128), Color.Magenta * TransitionAlpha, 0f,
                                  Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Frost Sorceress", centerVect + new Vector2(-220, 380 + 192), Color.Magenta * TransitionAlpha, 0f,
                                  Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Eyeball", centerVect + new Vector2(-220, 380+256), Color.Magenta * TransitionAlpha, 0f,
                                   Vector2.Zero, 1f, SpriteEffects.None, 1);

            spriteBatch.DrawString(ScreenManager.Font, "Runs and falls off of platforms", centerVect + new Vector2(-220, 405), Color.LightGray * TransitionAlpha, 0f,
                                   Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Walks left and right sticking to a platform", centerVect + new Vector2(-220, 405 + 64), Color.LightGray * TransitionAlpha, 0f,
                                   Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Patrols and casts fireballs", centerVect + new Vector2(-220, 405 + 128), Color.LightGray * TransitionAlpha, 0f,
                                  Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Patrols and casts frost balls which restrict the heros movement", centerVect + new Vector2(-220, 405 + 192), Color.LightGray * TransitionAlpha, 0f,
                                  Vector2.Zero, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(ScreenManager.Font, "Flys directly left. Can fly through platforms.", centerVect + new Vector2(-220, 405 + 256), Color.LightGray * TransitionAlpha, 0f,
                                   Vector2.Zero, 1f, SpriteEffects.None, 1);

            spriteBatch.End();


        }


        #endregion
    }
}
