// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// NPC Class
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
    /// The NPC class is the backbone to all NPC's, and their thought process
    /// </summary>
    public abstract class NPC : Character
    {
        // ---------------------
        // Stats
        // ---------------------
        public int Jump_Height = 3; // Default Jump height- ... not actually accurate measurement of jump height.
        public int Attention_range = 5; // Default NPC sight range
        public int Wander_range = 5; // Default NPC wander range
        public int Persistence = 3;

        public Vector2 Post_Position; // The position an NPC should attempt to be close to.

        // ---------------------
        // Boundaries & Offsets
        // ---------------------
        public Vector2 Eye_Offset = new Vector2(0, -1); // The offset position off this NPC's eyes from its grid position
        // ---------------------

        // ---------------------
        // AI Logic
        // ---------------------
        protected int Health_Cache;

        public Effect health_container;
        public Effect health_bar;

        protected Brain Brain;

        // ---------------------
        // Flags
        // ---------------------
        // TODO

        /// <summary>
        /// The NPC class contains essential methods & data that bring functional life to all non player character entities
        /// </summary>
        public NPC()
            : base()
        {
            // Functional Data
            // -----------------------
            Health_Cache = Health_Points;
            Brain = new Brain(this);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                // -----------------------
                // Health Clock Control
                if (Health_Points != Health_Cache) // Then health has changed- so we should display our healthbar.
                    Show_Health();
                Health_Cache = Health_Points;
                // -----------------------

                NPC_Behaviors();
                Brain.Think(gameTime);
            }

            base.Update(gameTime);
        }

        // NPC Action Controllers
        //
        // Varying Methods allow NPC's to perform dynamic actions
        // & behaviors
        // ----------------------------------

        /// <summary>
        /// Overrideable method lets any new NPC type set its own custom behavioral patterns
        /// </summary>
        public virtual void NPC_Behaviors()
        {
            if (Final_frame) // Clears Attacks info
                Try_Attack_Clear();

            Try_Idle(); // Prevents frame stick. Ensure base runs within overriden NPC Behavior
        }
        // ----------------------------------

        // ----------------------------------
        /// <summary>
        /// Shows this NPC's healthbar
        /// </summary>
        public void Show_Health()
        {
            // ----------------------------------
            // Health Container. Remove old if not null
            if (health_container != null)
                Particles.Active_Effects.Remove(health_container);

            Particles.Do_Effect(
            "HPcontainer",
            4,
            new Vector2(0, -((Get_Bounds().Height) / 2 + Particles.Effects["HPcontainer"].Dimensions.Y)),
            Vector2.Zero,
            true,
            this
            );
            // ----------------------------------
            // Health Bar. Remove old if not null
            if (health_bar != null)
                Particles.Active_Effects.Remove(health_bar);

            Particles.Do_Effect(
            "HP",
            4,
            new Vector2(0, -((Get_Bounds().Height) / 2 + Particles.Effects["HP"].Dimensions.Y)),
            Vector2.Zero,
            true,
            this
            ); 
        }
        // ----------------------------------
    }
}
