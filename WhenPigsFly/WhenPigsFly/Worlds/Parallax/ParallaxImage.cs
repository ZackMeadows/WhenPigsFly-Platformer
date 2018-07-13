// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Parallax Image Class
// Created 12/02/2015
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
    /// Parallax images are the individual peices of a full World Parallax object
    /// </summary>
    public class ParallaxImage : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // ---------------------
        // Essentials
        // ---------------------
        private Texture2D image;
        private Vector2 position_1 = Vector2.Zero;
        private Vector2 position_2 = Vector2.Zero;

        private int wrap_counter = 0;
        private float scale = 1f;

        // ---------------------
        // Configuration
        // ---------------------
        private bool affected_by_camera = true;
        private float scale_offset;
        private Vector2 position_offset;
        private float proximity;
        private bool autoscroll;
        private float autoscroll_speed;

        private float autoscroll_counter;


        // ---------------------
        #region Getters & Setters
        public Texture2D Image
        { get { return image; } }
        public Vector2 Position
        { get { return position_1; } }
        #endregion
        // ---------------------

        public ParallaxImage(
            Texture2D image,
            Vector2 position_offset,
            float proximity = 1.0f,
            float scale_offset = 0f,
            bool autoscroll = false,
            float autoscroll_speed = 0f,
            bool camera_affected = true
            )
            : base(Shared.Main)
        {
            this.image = image;

            this.position_offset = position_offset;
            this.proximity = proximity;
            this.scale_offset = scale_offset;
            this.autoscroll = autoscroll;
            this.autoscroll_speed = autoscroll_speed;
            this.affected_by_camera = camera_affected;
        }


        /// <summary>
        /// Shifts and Scrolls the Parallax Image according to input attributes like offset & zoom
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // ---------------------
            // Gather some crucial data & screen space
            Vector2 Screen_Dimension = new Vector2
                (
                Shared.Screen.Get_WorldSpace().X + Shared.Screen.Get_WorldSpace().Width,
                Shared.Screen.Get_WorldSpace().Y + Shared.Screen.Get_WorldSpace().Height
                );

            Vector2 Screen_Lock = new Vector2
                (
                Shared.Screen.Get_WorldSpace().X + Shared.Screen.Get_WorldSpace().Width / 2,
                Shared.Screen.Get_WorldSpace().Y + Shared.Screen.Get_WorldSpace().Height / 2
                );

            Vector2 Relation = new Vector2
                (
                Screen_Lock.X * proximity,
                Screen_Lock.Y * proximity
                );
            // ---------------------

            // ---------------------
            float Scaled_Image = image.Width * ((scale - scale_offset));
            float Wrap_Offset = wrap_counter * Scaled_Image;
            // ---------------------

            // ---------------------
            // Refresh parallax position based on influencing data
            position_1.X = Screen_Lock.X - Relation.X + autoscroll_counter - Wrap_Offset - (position_offset.X * Shared.Pixel_Scale);
            position_1.Y = Screen_Lock.Y - Relation.Y - (position_offset.Y * Shared.Pixel_Scale);
            position_2.X = position_1.X + Scaled_Image;
            position_2.Y = position_1.Y;
            // ---------------------

            // ---------------------
            // Ensure the two images remain wrapped around the camera
            if (
                !((position_1.X + (Scaled_Image / 2) > Screen_Dimension.X)
                &&
                (position_1.X - (Scaled_Image / 2) < Shared.Screen.Get_WorldSpace().X))
                &&
                !((position_2.X + (Scaled_Image / 2) > Screen_Dimension.X)
                &&
                (position_2.X - (Scaled_Image / 2) < Shared.Screen.Get_WorldSpace().X))
                )
            {
                if (((position_1.X + (Scaled_Image / 2)) > Screen_Dimension.X)
                    && position_2.X > Shared.Screen.Get_WorldSpace().X)
                {
                    wrap_counter++;
                }
                if (((position_2.X - (Scaled_Image / 2)) < Shared.Screen.Get_WorldSpace().X)
                    && position_1.X < Screen_Dimension.X)
                {
                    wrap_counter--;
                }
            }
            // ---------------------

            // ---------------------
            // Ugly fix that assures me autoscroll won't be going completely redonk
            if (autoscroll)
                autoscroll_counter += autoscroll_speed;
            if (autoscroll_counter > 9999999)
                autoscroll_counter = 0;
            // ---------------------
        }
        // ---------------------

        /// <summary>
        /// Basic Draw method called only by owning World Parallax
        /// </summary>
        public void Draw_Parallax(SpriteBatch spriteBatch)
        {
            // ---------------------
            Rectangle source = new Rectangle
                (0, 0, image.Width, image.Height);

            Vector2 origin = new Vector2
            (
            (image.Width / 2),
            (image.Height / 2)
            );
            // ---------------------
            spriteBatch.Draw
                    (
                    image,
                    position_1,
                    source,
                    Color.White,
                    0f,
                    origin,
                    ((scale - scale_offset)),
                    SpriteEffects.None,
                    0
                    );
            spriteBatch.Draw
                    (
                    image,
                    position_2,
                    source,
                    Color.White,
                    0,
                    origin,
                    ((scale - scale_offset)),
                    SpriteEffects.None,
                    0
                    );
            // ---------------------
        }
    }
}
