// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Abstract Game Scene Class
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
    public abstract class Scene : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public List<GameComponent> Components;

        // -------------------------
        /// <summary>
        /// Shows the scene
        /// </summary>
        public virtual void Show()
        {
            this.Enabled = true;
            this.Visible = true;
        }
        // -------------------------

        // -------------------------
        /// <summary>
        /// Hides the scene
        /// </summary>
        public virtual void Hide()
        {
            this.Enabled = false;
            this.Visible = false;
        }
        // -------------------------

        public Scene(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            Components = new List<GameComponent>();
            Hide();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            foreach (GameComponent item in Components)
            {
                if (item.Enabled)
                {
                    item.Update(gameTime);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawableGameComponent component = null;
            foreach (GameComponent item in Components)
            {
                if (item is DrawableGameComponent)
                {
                    component = (DrawableGameComponent)item;
                    if (component.Visible)
                    {
                        component.Draw(gameTime);   
                    }
                }
            }
            base.Draw(gameTime);
        }
    }
}
