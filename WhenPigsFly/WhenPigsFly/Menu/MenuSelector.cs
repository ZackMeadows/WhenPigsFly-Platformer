// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Menu Component
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
    public class MenuSelector : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch spriteBatch = Shared.Batch;
        private SpriteFont regularFont, hilightFont;
        private List<string> menuItems;
        public int SelectedIndex = 0;

        private Vector2 position;
        private Color regularColor = Color.Gray;
        private Color hilightColor = Color.White;

        private Texture2D inactive;
        private Texture2D active;

        private KeyboardState oldState;

        public MenuSelector(
            SpriteFont regularFont,
            SpriteFont hilightFont,
            string[] menus)
            : base(Shared.Main)
        {
            this.regularFont = regularFont;
            this.hilightFont = hilightFont;
            menuItems = menus.ToList();
            position = new Vector2(10, Shared.RESOLUTION.Y - 300);

            inactive = Shared.IMG_Interface["option_inactive"];
            active = Shared.IMG_Interface["option_active"];
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S) 
                || ks.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
            {
                SelectedIndex++;
                Shared.SFX_Sounds["Select"].Play();
                if (SelectedIndex == menuItems.Count)
                {
                    SelectedIndex = 0;
                }
            }
            else if (ks.IsKeyDown(Keys.W) && oldState.IsKeyUp(Keys.W) 
                || ks.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
            {
                SelectedIndex--;
                Shared.SFX_Sounds["Select"].Play();
                if (SelectedIndex == -1)
                {
                    SelectedIndex = menuItems.Count - 1;
                }
            }
            oldState = ks;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the main menu
            // -----------------------------------------
            Vector2 tempPos = position;
            tempPos.X = 0;

            // Draw the box underlays first
            // -----------------------------------------
            spriteBatch.Begin();
            for (int item = menuItems.Count - 1; item >= 0; item--)
            {
                if (SelectedIndex == item)
                {
                    spriteBatch.Draw(active, tempPos, Color.White);
                    tempPos.Y -= active.Height;
                }
                else
                {
                    spriteBatch.Draw(inactive, tempPos, Color.White);
                    tempPos.Y -= active.Height;
                }
            }

            // Next comes the actual text
            // -----------------------------------------
            tempPos = position;
            for (int item = menuItems.Count - 1; item >= 0; item--)
            {
                if (SelectedIndex == item)
                {
                    spriteBatch.DrawString(hilightFont, menuItems[item], tempPos, hilightColor);
                    tempPos.Y -= inactive.Height;
                }
                else
                {
                    spriteBatch.DrawString(regularFont, menuItems[item], tempPos, regularColor);
                    tempPos.Y -= inactive.Height;
                }
            }
            // -----------------------------------------
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
