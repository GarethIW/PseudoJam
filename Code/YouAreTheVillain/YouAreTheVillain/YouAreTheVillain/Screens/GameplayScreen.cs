#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;
#endregion

namespace YouAreTheVillain
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Map gameMap;
        Camera gameCamera;
        Hero gameHero;
        MinionManager gameMinionManager;

        Vector2 HeartsPos = new Vector2(50, 50);
        Vector2 floatingHeartPos = new Vector2(0, -100);
        Texture2D texHearts;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            IsStubbourn = true;

            EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.FreeDrag | Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "YouAreTheVillainContent");

            gameFont = content.Load<SpriteFont>("menufont");
            texHearts = content.Load<Texture2D>("hearts");

            gameMap = content.Load<Map>("rockmap");
            gameCamera = new Camera(ScreenManager.GraphicsDevice.Viewport, gameMap);

            gameMinionManager = new MinionManager();
            gameMinionManager.LoadContent(content);

            GameManager.Map = gameMap;
            GameManager.Camera = gameCamera;
            GameManager.MinionManager = gameMinionManager;

            gameHero = new Hero();
            gameHero.LoadContent(content);
            gameHero.Initialize();

            gameCamera.Target = gameHero.Position;

            GameManager.Hero = gameHero;

            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                gameCamera.Update(ScreenManager.GraphicsDevice.Viewport);
                gameHero.Update(gameTime);
                gameMinionManager.Update(gameTime);
            }

            floatingHeartPos -= new Vector2(0, 1f);

            if (gameHero.painAlpha == 1f)
                floatingHeartPos = HeartsPos + new Vector2((40 * (gameHero.HP)), 0);
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            PlayerIndex player;
            if (input.IsPauseGame(ControllingPlayer))
            {
                PauseBackgroundScreen pauseBG = new PauseBackgroundScreen();
                ScreenManager.AddScreen(pauseBG, ControllingPlayer);
                ScreenManager.AddScreen(new PauseMenuScreen(pauseBG), ControllingPlayer);
                
            }
        
            if(IsActive)
            {
                if (input.MouseDragging)
                {
                    gameCamera.Target -= input.MouseDelta;
                    //gameCamera.cl
                }

                if (input.DragGesture.HasValue)
                {
                    gameCamera.Target -= input.DragGesture.Value.Delta;
                }

                if (input.TapPosition.HasValue)
                {
                    Vector2 tapPos = input.TapPosition.Value;

                    tapPos += gameCamera.Position;

                    Point tilePos = new Point((int)tapPos.X / 64, (int)tapPos.Y / 64);

                    var t = gameMap.Layers.Where(l => l.Name == "FG").First();
                    TileLayer tileLayer = t as TileLayer;

                    bool found = false;

                    int type = MinionManager.randomNumber.Next(2);

                    while (!found)
                    {
                        if (tilePos.X >= tileLayer.Tiles.GetLowerBound(0) || tilePos.X <= tileLayer.Tiles.GetUpperBound(0) &&
                            tilePos.Y >= tileLayer.Tiles.GetLowerBound(1) || tilePos.Y <= tileLayer.Tiles.GetUpperBound(1))
                        {
                            if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                            {
                                if (tilePos.Y - 1 >= tileLayer.Tiles.GetLowerBound(1))
                                {
                                    if (tileLayer.Tiles[tilePos.X, tilePos.Y - 1] == null)
                                    {
                                        gameMinionManager.Add(new Vector2((tilePos.X * 64) + 32, (tilePos.Y * 64) - 32), type);
                                        found = true;
                                    }
                                    else tilePos.Y -= 1;
                                }
                                else found = true;
                            }
                            else found = true;
                        }
                        else found = true;
                    }
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            gameMap.DrawLayer(spriteBatch, "FG", gameCamera);

            gameHero.Draw(spriteBatch);
            gameMinionManager.Draw(spriteBatch);

            if (gameHero.SpawnTime > 0)
            {
                spriteBatch.DrawString(gameFont, "Hero spawning in " + (int)(gameHero.SpawnTime / 1000), new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 100), Color.White, 0f, gameFont.MeasureString("Hero spawning in " + (int)(gameHero.SpawnTime / 1000)) / 2, 1f, SpriteEffects.None, 1);
            }
            if (gameHero.HP <= 0)
            {
                spriteBatch.DrawString(gameFont, "Congratulations\nYou defeated the Hero", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 100), Color.White, 0f, gameFont.MeasureString("Congratulations\nYou defeated the Hero") / 2, 1f, SpriteEffects.None, 1);
            }

            spriteBatch.Draw(texHearts, floatingHeartPos, new Rectangle(64, 0, 32, 32), Color.White);

            for (int i = 0; i < gameHero.MaxHP; i++)
            {
                if (gameHero.HP >= i + 1)
                {
                    spriteBatch.Draw(texHearts, HeartsPos + new Vector2(i * 40, 0), new Rectangle(0, 0, 32, 32), Color.White);
                }
                else
                {
                    spriteBatch.Draw(texHearts, HeartsPos + new Vector2(i * 40, 0), new Rectangle(32, 0, 32, 32), Color.White);
                }
            }

            

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition >= 0f)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }


        #endregion
    }
}
