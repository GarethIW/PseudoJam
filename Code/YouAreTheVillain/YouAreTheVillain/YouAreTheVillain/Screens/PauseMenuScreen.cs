#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace YouAreTheVillain
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    public class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        PauseBackgroundScreen BGScreen;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public PauseMenuScreen(PauseBackgroundScreen pauseBG)
            : base("Pause")
        {
            BGScreen = pauseBG;
            IsPopup = true;
        }

        public override void LoadContent()
        {
            MenuEntry resumeGameMenuEntry;
            // Create our menu entries.
            if(GameManager.Hero.ReachedPrincess)
                resumeGameMenuEntry = new MenuEntry("Try Again");
            else if (GameManager.Hero.HP<=0)
            {
                if(GameManager.Level<2)
                    resumeGameMenuEntry = new MenuEntry("Next Level");
                else
                    resumeGameMenuEntry = new MenuEntry("Start Again");
            }
            else
                resumeGameMenuEntry = new MenuEntry("Resume");



            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit to Title");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += ResumeGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += ExitMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            //MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            base.LoadContent();
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void ResumeGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (GameManager.Hero.HP<=0 && GameManager.Level<2) GameManager.Level++;
            else
                if (GameManager.Hero.HP <= 0 && GameManager.Level == 2) GameManager.Level = 0;

            if (GameManager.Hero.HP <= 0 || GameManager.Hero.ReachedPrincess)
            {
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
                               new GameplayScreen());
            }
            else
            {
                BGScreen.ExitScreen();
                ExitScreen();
            }
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void ExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen(),
                               new MainMenuScreen());
        }


        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            BGScreen.ExitScreen();
            ExitScreen();
        }


        #endregion
    }
}
