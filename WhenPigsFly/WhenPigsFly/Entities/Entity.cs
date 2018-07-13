// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Entity Abstract Class
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
    /// The entity class is the base class which all game entities shall inherit from, including
    /// The player, enemies, items, hazards, objects, etc.
    /// </summary>
    public abstract class Entity : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // ---------------------
        // Drawing & Positioning
        // ---------------------
        protected SpriteBatch spriteBatch = Shared.Batch;
        private Texture2D spriteSheet;
        private Texture2D still_image;
        private Vector2 frameSize;
        private Color overlay_color = Color.White;

        protected float rotation = 0f;
        public Vector2 Position;
        private Vector2 origin;

        // ---------------------
        // Game Logic
        // ---------------------
        public Vector2 Grid_Position;
        protected Shared.Direction direction = Shared.Direction.RIGHT;

        protected Rectangle Boundary_Offset = Rectangle.Empty;

        // ---------------------
        #region Getters & Setters
        public Texture2D SpriteSheet
        { get { return spriteSheet; } set { spriteSheet = value; } }

        public Texture2D Still_image
        { get { return still_image; } set { still_image = value; } }

        public Vector2 FrameSize
        { get { return frameSize; } set { frameSize = value; } }
        public Color Overlay_Color
        { get { return overlay_color; } set { overlay_color = value; } }

        public Vector2 Origin
        {get { return origin; }set { origin = value; }}

        public Shared.Direction Direction
        { get { return direction; }set {direction = value; }}
        #endregion
        // ---------------------
        public Entity()
            : base(Shared.Main) 
        {
            Enabled = false;
        }

        /// <summary>
        /// Get Bounds is a necessaity for all entities- but function varies dependant on type. Therefore, declare it but do not structure it
        /// </summary>
        /// <returns>Parent returns empty rectangle</returns>
        public virtual Rectangle Get_Bounds()
        {
            // Overriden by children
            return Rectangle.Empty;
        }

        /// <summary>
        /// A debugging feature that's used to draw
        /// boxes around an entities bound box
        /// </summary>
        /// <param name="box">The Rectangle to draw around</param>
        /// <param name="border_width">Width of the box border</param>
        /// <param name="color">Color of the border</param>
        public void Draw_Box(Rectangle box, int border_width, Color color)
        {
            spriteBatch.Begin
                (
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                Shared.Screen.Get_Camera()
                );
            // Draw top line
            spriteBatch.Draw(Shared.Pixel, new Rectangle(box.X, box.Y, box.Width, border_width), color);

            // Draw left line
            spriteBatch.Draw(Shared.Pixel, new Rectangle(box.X, box.Y, border_width, box.Height), color);

            // Draw right line
            spriteBatch.Draw(Shared.Pixel, new Rectangle((box.X + box.Width - border_width),
                                            box.Y,
                                            border_width,
                                            box.Height), color);
            // Draw bottom line
            spriteBatch.Draw(Shared.Pixel, new Rectangle(box.X,
                                            box.Y + box.Height - border_width,
                                            box.Width,
                                            border_width), color);
            spriteBatch.End();
        }

        /// <summary>
        ///  Simple translation method that moves entities along the grid,
        ///  translating it according to zoom
        /// </summary>
        /// <param name="translation"> The Vector2 translation amount</param>
        public void Shift(Vector2 translation)
        {
            Position -= translation;
        }
        // -------------------------------------------
    }
}
