// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// About Menu - Displays information about the game!
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
    public class AboutScene : Scene
    {
        // --------------------
        // Scene Data
        // --------------------
        private SpriteBatch spriteBatch = Shared.Batch;

        private Texture2D overlay;
        private Vector2 overlay_origin;

        private Texture2D image;
        private Vector2 image_origin;

        private Texture2D go_back;

        private Vector2 position;
        // --------------------

        public AboutScene()
            : base(Shared.Main)
        {
            // --------------------
            // Build Scene
            // --------------------
            overlay = Shared.IMG_Interface["dark_overlay"];
            overlay_origin = new Vector2(overlay.Width / 2, overlay.Height / 2);

            image = Shared.IMG_Interface["About_image"];
            image_origin = new Vector2(image.Width / 2, image.Height / 2);

            go_back = Shared.IMG_Interface["go_back"];
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
            // --------------------
            // Go Back Button
            // --------------------
            spriteBatch.Draw(
                go_back,
                new Vector2(Shared.RESOLUTION.X - 10 - go_back.Width, Shared.RESOLUTION.Y - 10 - go_back.Height),
                Color.White);
            // --------------------
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
