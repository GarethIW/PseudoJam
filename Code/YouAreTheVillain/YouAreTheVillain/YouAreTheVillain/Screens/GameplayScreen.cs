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
        ButtonManager gameButtonManager;
        ProjectileManager gameProjectileManager;
        ParallaxManager gameParallaxManager;

        Vector2 HeartsPos = new Vector2(100, 13);
        Vector2 floatingHeartPos = new Vector2(0, -100);

        double finishGameTimer = 0;
        bool shownFinishScreen = false;

        Texture2D texHearts;
        Texture2D texArrow;
        Texture2D texPrincess;
        Texture2D texTopBar;

        string mapName;

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
            texArrow = content.Load<Texture2D>("arrows");
            texPrincess = content.Load<Texture2D>("princess");
            texTopBar = content.Load<Texture2D>("topbar");

            switch (GameManager.Level)
            {
                case 0:
                    gameMap = content.Load<Map>("rockmap");
                    break;
                case 1:
                    gameMap = content.Load<Map>("dirtmap");
                    break;
                case 2:
                    gameMap = content.Load<Map>("castlemap");
                    break;
                default:
                    gameMap = content.Load<Map>("rockmap");
                    break;
            }
            gameCamera = new Camera(ScreenManager.GraphicsDevice.Viewport, gameMap);

            gameMinionManager = new MinionManager();
            gameMinionManager.LoadContent(content);

            gameProjectileManager = new ProjectileManager();
            gameProjectileManager.LoadContent(content);

            GameManager.Map = gameMap;
            GameManager.Camera = gameCamera;
            GameManager.MinionManager = gameMinionManager;
            GameManager.ProjectileManager = gameProjectileManager;

            gameHero = GameManager.Hero; //new Hero();
            gameHero.Initialize();
            gameHero.LoadContent(content);
            

            gameCamera.Target = gameHero.Position;

            //GameManager.Hero = gameHero;

            gameButtonManager = new ButtonManager();
            gameButtonManager.LoadContent(content);
            GameManager.ButtonManager = gameButtonManager;

            gameParallaxManager = new ParallaxManager(ScreenManager.GraphicsDevice.Viewport);
            gameParallaxManager.Layers.Add(new ParallaxLayer(content.Load<Texture2D>("background/sky"), Vector2.Zero, 0f));
            gameParallaxManager.Layers.Add(new ParallaxLayer(content.Load<Texture2D>("background/clouds1"), new Vector2(0, 50), -0.001f));
            gameParallaxManager.Layers.Add(new ParallaxLayer(content.Load<Texture2D>("background/clouds2"), new Vector2(0, 0), -0.005f));
            gameParallaxManager.Layers.Add(new ParallaxLayer(content.Load<Texture2D>("background/clouds3"), new Vector2(0, -50), -0.008f));
            gameParallaxManager.Layers.Add(new ParallaxLayer(content.Load<Texture2D>("background/mountains3"), new Vector2(0, 300), -0.02f));
            gameParallaxManager.Layers.Add(new ParallaxLayer(content.Load<Texture2D>("background/mountains2"), new Vector2(0, 100), -0.04f));
            gameParallaxManager.Layers.Add(new ParallaxLayer(content.Load<Texture2D>("background/mountains1"), new Vector2(0, 140), -0.07f));
            

            // Princess
            var layer = GameManager.Map.Layers.Where(l => l.Name == "Princess").First();
            if (layer != null)
            {
                MapObjectLayer objectlayer = layer as MapObjectLayer;

                foreach (MapObject o in objectlayer.Objects)
                    GameManager.princessPosition = new Vector2(o.Location.Center.X, o.Location.Center.Y);
            }

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
                gameProjectileManager.Update(gameTime);

                if (gameHero.HP <= 0 || gameHero.ReachedPrincess)
                {
                    finishGameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (finishGameTimer >= 3000 && !shownFinishScreen)
                    {
                        shownFinishScreen = true;
                        PauseBackgroundScreen pauseBG = new PauseBackgroundScreen();
                        ScreenManager.AddScreen(pauseBG, ControllingPlayer);
                        ScreenManager.AddScreen(new PauseMenuScreen(pauseBG), ControllingPlayer);
                    }
                }
            }

            floatingHeartPos -= new Vector2(0, 1f);

            if (gameHero.painAlpha == 1f)
                floatingHeartPos = HeartsPos + new Vector2((40 * (gameHero.HP)), 0);

            gameButtonManager.Update(gameTime);

            gameParallaxManager.Update(gameTime, gameCamera.Position);
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

                if (input.IsNewKeyPress(Keys.Space, null, out player)) gameHero.Position = gameHero.SpawnPoint;

                if (!gameButtonManager.HandleInput(input))
                {
                    if (input.TapPosition.HasValue)
                    {
                        Vector2 tapPos = input.TapPosition.Value;

                        tapPos += gameCamera.Position;

                        Point tilePos = new Point((int)tapPos.X / 64, (int)tapPos.Y / 64);

                        var t = gameMap.Layers.Where(l => l.Name == "FG").First();
                        TileLayer tileLayer = t as TileLayer;

                        bool found = false;

                        int type = gameButtonManager.SelectedButton;

                        if (gameButtonManager.Buttons[gameButtonManager.SelectedButton].CurrentCoolDown <= 0)
                        {

                            if (type != 4)
                            {
                                if (tilePos.X >= tileLayer.Tiles.GetLowerBound(0) || tilePos.X <= tileLayer.Tiles.GetUpperBound(0) &&
                                    tilePos.Y >= tileLayer.Tiles.GetLowerBound(1) || tilePos.Y <= tileLayer.Tiles.GetUpperBound(1))
                                {
                                    if (tileLayer.Tiles[tilePos.X, tilePos.Y] != null)
                                    {
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
                                                            gameButtonManager.Buttons[gameButtonManager.SelectedButton].CurrentCoolDown = gameButtonManager.Buttons[gameButtonManager.SelectedButton].CoolDown;
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
                                    else
                                    {
                                        while (!found)
                                        {
                                            if (tilePos.X >= tileLayer.Tiles.GetLowerBound(0) || tilePos.X <= tileLayer.Tiles.GetUpperBound(0) &&
                                                tilePos.Y >= tileLayer.Tiles.GetLowerBound(1) || tilePos.Y <= tileLayer.Tiles.GetUpperBound(1))
                                            {
                                                if (tileLayer.Tiles[tilePos.X, tilePos.Y] == null)
                                                {
                                                    if (tilePos.Y + 1 <= tileLayer.Tiles.GetUpperBound(1))
                                                    {
                                                        if (tileLayer.Tiles[tilePos.X, tilePos.Y + 1] != null)
                                                        {
                                                            gameMinionManager.Add(new Vector2((tilePos.X * 64) + 32, ((tilePos.Y + 1) * 64) - 32), type);
                                                            gameButtonManager.Buttons[gameButtonManager.SelectedButton].CurrentCoolDown = gameButtonManager.Buttons[gameButtonManager.SelectedButton].CoolDown;
                                                            found = true;
                                                        }
                                                        else tilePos.Y += 1;
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
                            else
                            {
                                gameMinionManager.Add(new Vector2((tilePos.X * 64) + 32, (tilePos.Y * 64) + 32), type);
                                gameButtonManager.Buttons[gameButtonManager.SelectedButton].CurrentCoolDown = gameButtonManager.Buttons[gameButtonManager.SelectedButton].CoolDown;
                            }
                        }
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
                                               Color.DarkGray, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            gameParallaxManager.Draw(spriteBatch);

            gameMap.DrawLayer(spriteBatch, "FG", gameCamera);

            gameHero.Draw(spriteBatch);
            gameProjectileManager.Draw(spriteBatch);
            gameMinionManager.Draw(spriteBatch);

            spriteBatch.Draw(texPrincess, GameManager.princessPosition - gameCamera.Position, null, Color.White, 0f, new Vector2(texPrincess.Width, texPrincess.Height) / 2, 1f, SpriteEffects.None, 1f);

            // HUD below this line ////////////////////////////////
            if (gameHero.SpawnTime > 0)
            {
                spriteBatch.DrawString(gameFont, "Hero spawning in " + (int)(gameHero.SpawnTime / 1000), new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 150) + new Vector2(2, 2), Color.Black, 0f, gameFont.MeasureString("Hero spawning in " + (int)(gameHero.SpawnTime / 1000)) / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(gameFont, "Hero spawning in " + (int)(gameHero.SpawnTime / 1000), new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 150), Color.White, 0f, gameFont.MeasureString("Hero spawning in " + (int)(gameHero.SpawnTime / 1000)) / 2, 1f, SpriteEffects.None, 1);
            }
            if (gameHero.HP <= 0)
            {
                spriteBatch.DrawString(gameFont, "Congratulations", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 150)+new Vector2(2,2), Color.Black, 0f, gameFont.MeasureString("Congratulations") / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(gameFont, "You defeated the Hero", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 175) + new Vector2(2, 2), Color.Black, 0f, gameFont.MeasureString("You defeated the Hero") / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(gameFont, "Congratulations", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 150), Color.White, 0f, gameFont.MeasureString("Congratulations") / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(gameFont, "You defeated the Hero", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 175), Color.White, 0f, gameFont.MeasureString("You defeated the Hero") / 2, 1f, SpriteEffects.None, 1);

            }
            if (gameHero.ReachedPrincess)
            {
                spriteBatch.DrawString(gameFont, "Oh No", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 150) + new Vector2(2, 2), Color.Black, 0f, gameFont.MeasureString("Oh No") / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(gameFont, "The Hero rescued the princess", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 175) + new Vector2(2, 2), Color.Black, 0f, gameFont.MeasureString("The Hero rescued the princess") / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(gameFont, "Oh No", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 150), Color.White, 0f, gameFont.MeasureString("Oh No") / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(gameFont, "The Hero rescued the princess", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 175), Color.White, 0f, gameFont.MeasureString("The Hero rescued the princess") / 2, 1f, SpriteEffects.None, 1);
            }

            if (gameHero.Position.X < gameCamera.Position.X)
            {
                spriteBatch.Draw(texArrow, new Vector2(100, MathHelper.Clamp(gameHero.Position.Y-gameCamera.Position.Y, 100, gameCamera.Height)), null, Color.White, 0f, new Vector2(texArrow.Width, texArrow.Height) / 2, 1f, SpriteEffects.FlipHorizontally, 1);
            }
            if (gameHero.Position.X > (gameCamera.Position.X + gameCamera.Width))
            {
                spriteBatch.Draw(texArrow, new Vector2(gameCamera.Width - 100, MathHelper.Clamp(gameHero.Position.Y - gameCamera.Position.Y, 100, gameCamera.Height)), null, Color.White, 0f, new Vector2(texArrow.Width, texArrow.Height) / 2, 1f, SpriteEffects.None, 1);
            }

            // HUD
            spriteBatch.Draw(texTopBar, Vector2.Zero, new Rectangle(0, 0, 12, 65), Color.White);
            for(int x=12;x<ScreenManager.GraphicsDevice.Viewport.Width;x+=72)
                spriteBatch.Draw(texTopBar, new Vector2(x,0), new Rectangle(12, 0, 72, 65), Color.White);
            spriteBatch.Draw(texTopBar, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width-12, 0), new Rectangle(84, 0, 12, 65), Color.White);

            spriteBatch.Draw(texTopBar, new Rectangle(0, 60, (int)(((float)ScreenManager.GraphicsDevice.Viewport.Width / (float)gameHero.XPTNL) * (float)gameHero.XP), 5), new Rectangle(0, 0, 1, 1), Color.Red);
            
            spriteBatch.Draw(texHearts, floatingHeartPos, new Rectangle(64, 0, 32, 32), Color.White);

            for (int i = 0; i < gameHero.MaxHP; i++)
            {
                spriteBatch.Draw(texHearts, HeartsPos + new Vector2(i * 40, 0) + new Vector2(2, 2), new Rectangle(32, 0, 32, 32), Color.Black * 0.4f);
                
                if (gameHero.HP >= i + 1)
                {
                    spriteBatch.Draw(texHearts, HeartsPos + new Vector2(i * 40, 0), new Rectangle(0, 0, 32, 32), Color.White);
                }
                else
                {
                    spriteBatch.Draw(texHearts, HeartsPos + new Vector2(i * 40, 0), new Rectangle(32, 0, 32, 32), Color.White);
                }
            }

            for (int i = 0; i < gameHero.MaxSwords; i++)
            {
                spriteBatch.Draw(texHearts, HeartsPos + new Vector2((gameHero.MaxHP * 40) + 50, 0) + new Vector2(i * 30, 0) + new Vector2(4,2), new Rectangle(96, 0, 32, 32), Color.Black * 0.4f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
                if(gameHero.numSwords>=i+1)
                    spriteBatch.Draw(texHearts, HeartsPos + new Vector2((gameHero.MaxHP * 40) + 50, 0) + new Vector2(i * 30, 0), new Rectangle(96, 0, 32, 32), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
                
            }

            spriteBatch.DrawString(gameFont, gameHero.Level.ToString(), HeartsPos + new Vector2(-40, 5) + new Vector2(2,2), Color.Black * 0.4f);
            spriteBatch.DrawString(gameFont, gameHero.Level.ToString(), HeartsPos + new Vector2(-40, 5), Color.White);

            gameButtonManager.Draw(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition >= 0f)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }


        #endregion
    }
}
