// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Dead Scene
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
    public class DeadScene : Scene
    {
        // --------------------
        // Scene Data
        // --------------------
        private SpriteBatch spriteBatch = Shared.Batch;

        private Texture2D overlay;
        private Vector2 overlay_origin;

        private Texture2D image;
        private Vector2 image_origin;

        private Texture2D image_two;

        private Vector2 position;
        // --------------------
        public DeadScene()
            : base(Shared.Main)
        {
            // --------------------
            // Build Scene
            // --------------------
            overlay = Shared.IMG_Interface["dark_overlay"];
            overlay_origin = new Vector2(overlay.Width / 2, overlay.Height / 2);

            image = Shared.IMG_Interface["Death_Screen"];
            image_two = Shared.IMG_Interface["Death_Screen_Continue"];
            image_origin = new Vector2(image.Width / 2, image.Height / 2);
            // --------------------
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
            if (Shared.Player.Respawner.Time != 0f)
            {
                spriteBatch.Draw(
                    image,
                    position,
                    null,
                    Color.White,
                    0f,
                    image_origin,
                    1,
                    SpriteEffects.None,
                    0f);
            }
            else
            {
                spriteBatch.Draw(
                    image_two,
                    position,
                    null,
                    Color.White,
                    0f,
                    image_origin,
                    1,
                    SpriteEffects.None,
                    0f);
            }
            // --------------------
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
