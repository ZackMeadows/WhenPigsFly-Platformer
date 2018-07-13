// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// GUI Element Class
// Created 12/13/2015
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
    /// GUI elements encompass all components that will lay atop the game screen to show data and information to the player.
    /// </summary>
    public class GUI_Element : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // ----------------------
        // Data
        // ----------------------
        private SpriteBatch spriteBatch = Shared.Batch;
        private Texture2D window;
        private Vector2 position;

        public string name = "Unnamed";
        public Color overlay_color = Color.White;
        // ----------------------

        // ----------------------
        #region Getters
        public Texture2D Window
        {
            get { return window; }
        }

        public Vector2 Position
        {
            get { return position; }
        }
        #endregion
        // ----------------------

        /// <summary>
        /// Basic GUI element constructor
        /// </summary>
        public GUI_Element(Texture2D window, Vector2 position)
            : base(Shared.Main)
        {
            this.window = window;
            this.position = position;
        }

        // ----------------------
        /// <summary>
        /// Draws the GUI element
        /// </summary>
        /// <param name="gameTime">Snapshot of gameTime</param>
        public override void Draw(GameTime gameTime)
        {

            Rectangle bar_dimension = new Rectangle(0, 0, window.Width, window.Height);

            // Stat bar displays
            // --------------------------

            if (name == "healthbar")
            {
            // Handle color 
            // --------------------------
            float percentage = ((float)Shared.Player.Health_Points / Shared.Player.MAX_Health_Points);
            if (percentage > 0.5f)
                overlay_color = new Color(165, 235, 126);
            else if (percentage > 0.25f)
                overlay_color = new Color(235, 220, 90);
            else if (percentage <= 0.25f)
                overlay_color = new Color(210, 64, 49);
            // --------------------------
            // Handle size
            bar_dimension.Width = (int)(bar_dimension.Width * percentage);

            if (bar_dimension.Width < 0)
                bar_dimension.Width = 0;
            // --------------------------
            }

            if (name == "focusbar")
            {
                // Handle size 
                // --------------------------
                float percentage = ((float)Shared.Player.Focus_Points / Shared.Player.MAX_Focus_Points);
                // --------------------------
                bar_dimension.Width = (int)(bar_dimension.Width * percentage);

                if (bar_dimension.Width < 0)
                    bar_dimension.Width = 0;
                // --------------------------
            }

            // Equip Swap
            // --------------------------
            if (name == "attack_bar")
            {
                // Swap image 
                // --------------------------
                if (Shared.Player.Equipped_Attack == "basic")
                    window = Shared.IMG_Interface["basic_active"];
                else if (Shared.Player.Equipped_Attack == "Arcane_Bolt")
                    window = Shared.IMG_Interface["arcane_active"];
                else
                    window = Shared.IMG_Interface["basic_active"];
                // --------------------------
            }

            spriteBatch.Begin
                (
                SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone
                );
            spriteBatch.Draw
                (
                window,
                position,
                bar_dimension,
                overlay_color,
                0f,
                new Vector2(0,0),
                Shared.Pixel_Scale,
                SpriteEffects.None,
                0
                );
            spriteBatch.End();
            base.Draw(gameTime);
        }
        // ----------------------
    }
}
