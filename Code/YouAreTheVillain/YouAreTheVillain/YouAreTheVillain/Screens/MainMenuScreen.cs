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
    public class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            
        }

        public override void LoadContent()
        {
            // Create our menu entries.
            MenuEntry campaignGameMenuEntry = new MenuEntry("START GAME");
            MenuEntry aboutGameMenuEntry = new MenuEntry("README DOT TXT");
            MenuEntry optionsMenuEntry = new MenuEntry("OPTIONS");
            MenuEntry exitMenuEntry = new MenuEntry("EXIT GAME");

            // Hook up menu event handlers.
            campaignGameMenuEntry.Selected += CampaignGameMenuEntrySelected;
            aboutGameMenuEntry.Selected += AboutGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += ExitMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(campaignGameMenuEntry);
            MenuEntries.Add(aboutGameMenuEntry);
            //MenuEntries.Add(optionsMenuEntry);
            if (!ScreenManager.IsPhone)
            {
                MenuEntries.Add(exitMenuEntry);
            }

            base.LoadContent();
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void CampaignGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameManager.Level = 1;
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
                               new GameplayScreen());
        }

        void AboutGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new AboutScreen(), e.PlayerIndex);
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
            ScreenManager.Game.Exit();
        }


        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
