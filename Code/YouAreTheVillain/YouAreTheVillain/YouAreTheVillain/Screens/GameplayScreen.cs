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
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "YouAreTheVillainContent");

            //gameFont = content.Load<SpriteFont>("gamefont");

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
                                        gameMinionManager.Add(new Vector2((tilePos.X * 64) + 32, (tilePos.Y * 64) - 32));
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

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition >= 0f)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }


        #endregion
    }
}
