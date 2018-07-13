// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Splash Screen - Developer Ad
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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Splashscreen : Scene
    {
        // --------------------
        // Scene Data
        // --------------------
        private SpriteBatch spriteBatch = Shared.Batch;

        private Texture2D overlay;
        private Vector2 overlay_origin;

        private Texture2D image;
        private Vector2 image_origin;

        private Vector2 position;
        // --------------------

        // --------------------
        // Splash Data
        // --------------------
        private float fade_value = 0f;
        private Clock fader = new Clock();
        // --------------------
        public Splashscreen()
            : base(Shared.Main)
        {
            // --------------------
            // Build Scene
            // --------------------
            overlay = Shared.IMG_Interface["white_overlay"];
            overlay_origin = new Vector2(overlay.Width / 2, overlay.Height / 2);

            image = Shared.IMG_Interface["AurafluxLogo"];
            image_origin = new Vector2(image.Width / 2, image.Height / 2);
            // --------------------
            // Delay fade in
            fader.Time = 2f;
            // --------------------
        }
        /// <summary>
        /// Controls splashscreen fade
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // --------------------
            // Clock control
            if (fader.Time > 0f)
            {
                float elapsed_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                fader.Time -= elapsed_time;
            }
            else
                fader.Time = 0f;

            // --------------------
            // Allow Splash Skip
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Enter))
            {
                fader.Primary_Decision = 2;
                Shared.MainMenu.Show();
                this.Hide();
            }


            // --------------------
            // Do Fading
            if (fader.Time == 0)
            {
                if (fader.Primary_Decision == 0)
                {
                    fade_value += 0.01f;

                    if (fade_value >= 1)
                    {
                        fader.Primary_Decision = 1;
                        fader.Time = 2f;
                    }
                }
                else if (fader.Primary_Decision == 1)
                {
                    fade_value -= 0.01f;
                    if (fade_value <= 0)
                    {
                        fader.Primary_Decision = 2;
                        fader.Time = 1f;
                    }
                }
                else if (fader.Primary_Decision == 2)
                {
                    Shared.MainMenu.Show();
                    this.Hide();
                }
            }
            // --------------------
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // --------------------
            // Determine Position
            // --------------------
            position = new Vector2(Shared.RESOLUTION.X / 2, Shared.RESOLUTION.Y / 2);

            spriteBatch.Begin();
            // --------------------
            // Draw Overlay
            // --------------------
            spriteBatch.Draw(
                overlay,
                position,
                null,
                Color.White,
                0f,
                overlay_origin,
                3,
                SpriteEffects.None,
                0f);
            // --------------------
            // Draw Image
            // --------------------
            spriteBatch.Draw(
                image,
                position,
                null,
                new Color(
                    255 * (fade_value), 
                    255 * (fade_value), 
                    255 * (fade_value), 
                    fade_value),
                0f,
                image_origin,
                1,
                SpriteEffects.None,
                0f);
            // --------------------
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
