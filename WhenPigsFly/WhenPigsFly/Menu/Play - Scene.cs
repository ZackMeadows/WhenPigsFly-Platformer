// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Play Scene - The Game
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
    public class PlayScene : Scene
    {
        // ------------------------------
        // Data
        private SpriteBatch spriteBatch = Shared.Batch;
        private World active_world;
        private List<GUI_Element> GUI = new List<GUI_Element>();
        // ------------------------------

        public PlayScene()
            : base(Shared.Main)
        {
            // ---------------------------------
            // Build GUI
            // ---------------------------------

            // Stat bar
            // ---------------------------------
            Texture2D stat_HUD = Shared.IMG_Interface["StatBar"];
            GUI_Element Stat_bar = new GUI_Element
                (
                stat_HUD, 
                new Vector2(10,10)
                );
            Stat_bar.name = "statbar";

            // Health
            // ---------------------------------
            Texture2D health_img = Shared.IMG_Interface["healthbar"];
            GUI_Element healthbar = new GUI_Element
                (
                health_img,
                new Vector2(Stat_bar.Position.X + (19 * Shared.Pixel_Scale), Stat_bar.Position.Y)
                );
            healthbar.name = "healthbar";

            // Focus
            // ---------------------------------
            Texture2D focus_img = Shared.IMG_Interface["focusbar"];
            GUI_Element focusbar = new GUI_Element
                (
                focus_img,
                new Vector2(Stat_bar.Position.X + (32 * Shared.Pixel_Scale), Stat_bar.Position.Y)
                );
            focusbar.name = "focusbar";
            // ---------------------------------
            // Attack bar
            // ---------------------------------
            Texture2D attack_img = Shared.IMG_Interface["basic_active"];
            GUI_Element attack_bar = new GUI_Element
                (
                attack_img,
                new Vector2(Stat_bar.Position.X + ((stat_HUD.Width + 2) * Shared.Pixel_Scale), Stat_bar.Position.Y)
                );
            attack_bar.name = "attack_bar";
            // ---------------------------------

            Shared.GUI.Add(Stat_bar);
            Shared.GUI.Add(healthbar);
            Shared.GUI.Add(focusbar);
            Shared.GUI.Add(attack_bar);

            // ---------------------------------
            // Create the active world
            // ---------------------------------
            string world = "w1";

            active_world = new World(new Vector2
                (
                Shared.IMG_Worlds[world].Width,
                Shared.IMG_Worlds[world].Height
                ));
            Components.Add(active_world);

            World.Generate(world, active_world);
            Shared.Active_World = active_world;
            // ---------------------------------
        }

        // ------------------------------
        /// <summary>
        /// Updates the Play Scene
        /// </summary>
        /// <param name="gameTime">Snapshot of game time</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        // ------------------------------
    }
}
