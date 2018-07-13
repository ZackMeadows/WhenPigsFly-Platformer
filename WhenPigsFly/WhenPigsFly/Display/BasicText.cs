// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Basic Text Class
// Created 12/01/2015
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
    ///  The basic text class is used, basically, draw text. Amazing!
    /// </summary>
    public class BasicText : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // ---------------------
        // Essentials
        // ---------------------
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Vector2 position;
        private Color color;
        private string message;
        // ---------------------

        public Vector2 Position
        {get { return position; }set { position = value; }}

        public string Message
        {get { return message; }set { message = value; }}

        /// <summary>
        /// The Basic Text constructor
        /// </summary>
        /// <param name="font">The font to be used</param>
        /// <param name="position">The position to draw the text</param>
        /// <param name="message">The message to display</param>
        /// <param name="color">The color of the font</param>
        public BasicText(SpriteFont font,Vector2 position,string message, Color color)
            : base(Shared.Main)
        {
            this.spriteBatch = Shared.Batch;
            this.font = font;
            this.position = position;
            this.message = message;
            this.color = color;
        }

        /// <summary>
        /// Draws the basic text
        /// </summary>
        /// <param name="gameTime">Gametime snapshot</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, position, color);
            spriteBatch.End();
        }
    }
}
