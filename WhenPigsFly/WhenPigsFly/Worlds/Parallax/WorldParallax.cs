// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// World Parallax Class
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
    /// World Parallax's are a group of images that dynamically move along the games background. World
    /// </summary>
    public class WorldParallax : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // ---------------------
        // Draw Info & Parallax List
        // ---------------------
        private SpriteBatch spriteBatch = Shared.Batch;
        private List<ParallaxImage> parallax_images;
        // ---------------------

        public List<ParallaxImage> Parallax_images
        {
            get { return parallax_images; }
            set { parallax_images = value; }
        }

        /// <summary>
        /// World Parallax Default Constructor
        /// </summary>
        public WorldParallax()
            : base(Shared.Main)
        {
            parallax_images = new List<ParallaxImage>();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // ---------------------
            foreach (ParallaxImage parallax in parallax_images)
            {
                parallax.Update(gameTime);
            }
            // ---------------------
            Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // ---------------------
            spriteBatch.Begin
                    (
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone,
                    null,
                    Shared.Screen.Get_Camera()
                    );
            foreach (ParallaxImage parallax in parallax_images)
            {
                parallax.Draw_Parallax(spriteBatch);
            }
            spriteBatch.End();
            // ---------------------
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // World Parallax Production Here
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Call to produce all the available preset parallax
        /// </summary>
        public static void Create_WorldParallax()
        {
            // ---------------------
            #region Main Menu
            WorldParallax Menu = new WorldParallax();

            ParallaxImage menu_sky = new ParallaxImage
                (
                Shared.IMG_Parallax["sky"], 
                Vector2.Zero,
                0f
                );
            ParallaxImage menu_mountains = new ParallaxImage
                (
                Shared.IMG_Interface["MenuMountains"],
                new Vector2(-100, -50),
                0.1f,
                autoscroll: true,
                autoscroll_speed: -0.4f,
                camera_affected: false
                );
            ParallaxImage menu_clouds3 = new ParallaxImage
                (
                Shared.IMG_Parallax["clouds2"],
                new Vector2(0, 0),
                0.1f,
                0.5f,
                autoscroll: true,
                autoscroll_speed: -0.75f
                );
            ParallaxImage menu_clouds2 = new ParallaxImage
                (
                Shared.IMG_Parallax["clouds2"],
                new Vector2(0, -20),
                0.1f,
                0.5f,
                autoscroll: true,
                autoscroll_speed: -1f
                );
            ParallaxImage menu_clouds1 = new ParallaxImage
                (
                Shared.IMG_Parallax["clouds1"],
                new Vector2(100, 0),
                0.2f,
                0.5f,
                autoscroll: true,
                autoscroll_speed: -1.5f
                );

            Menu.Parallax_images.Add(menu_sky);
            Menu.Parallax_images.Add(menu_clouds3);
            Menu.Parallax_images.Add(menu_mountains);
            Menu.Parallax_images.Add(menu_clouds2);
            Menu.Parallax_images.Add(menu_clouds1);
            Shared.World_Parallax.Add("Menu", Menu);
            #endregion
            // ---------------------

            // ---------------------
            #region The Overworld
            WorldParallax Overworld = new WorldParallax();

            ParallaxImage sky = new ParallaxImage
                (
                Shared.IMG_Parallax["sky"],
                Vector2.Zero,
                0f
                );
            ParallaxImage mountains = new ParallaxImage
                (
                Shared.IMG_Parallax["far_mountains"],
                new Vector2(0, -180),
                0.2f
                );
            ParallaxImage hills = new ParallaxImage
                (
                Shared.IMG_Parallax["hills"],
                new Vector2(100, -550),
                0.5f
                );
            ParallaxImage clouds2 = new ParallaxImage
                (
                Shared.IMG_Parallax["clouds2"],
                new Vector2(0, -50),
                0.1f,
                autoscroll: true,
                autoscroll_speed: 0.5f
                );
            ParallaxImage clouds1 = new ParallaxImage
                (
                Shared.IMG_Parallax["clouds1"],
                new Vector2(0, -150),
                0.2f,
                autoscroll: true,
                autoscroll_speed: 1f
                );

            Overworld.Parallax_images.Add(sky);
            Overworld.Parallax_images.Add(clouds2);
            Overworld.Parallax_images.Add(mountains);
            Overworld.Parallax_images.Add(hills);
            Overworld.Parallax_images.Add(clouds1);

            Shared.World_Parallax.Add("Overworld", Overworld);
            #endregion
            // ---------------------
        }
        // ---------------------
    }
}
