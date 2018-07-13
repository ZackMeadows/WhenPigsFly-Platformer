// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Main Menu
// Created 11/27/2015
// ------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WhenPigsFly
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenu : Scene
    {
        public MenuSelector Menu;

        private SpriteBatch spriteBatch = Shared.Batch;
        string[] menuItems = {
                                 "Begin Adventure",
                                 "About the Game",  
                                 "Controls",
                                 "Credits",
                                 "Exit"
                             };
        public MainMenu()
            : base(Shared.Main)
        {
            Menu = new MenuSelector(
                Shared.Main.Content.Load<SpriteFont>("Fonts/Pigsfont"),
                Shared.Main.Content.Load<SpriteFont>("Fonts/Pigsfont"),
                menuItems
                );
            this.Components.Add(Menu);
        }
    }
}
