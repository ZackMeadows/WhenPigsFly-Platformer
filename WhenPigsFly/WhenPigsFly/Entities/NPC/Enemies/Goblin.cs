// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Goblin Enemy
// Created 12/06/2015
// ------------------------------

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhenPigsFly
{
    public class Goblin : NPC
    {
        // ---------------------
        // Goblin
        // ---------------------

        public Goblin(Vector2 input_position)
        {
            // Establish Basic Data
            // -----------------------
            this.SpriteSheet = Shared.IMG_Sprites["goblin"];
            this.Set_State(Shared.EntityState.IDLE);
            this.FrameSize = new Vector2(42, 42);
            this.Position = input_position;
            // -----------------------

            // Stats
            // -----------------------
            Eye_Offset = new Vector2(0, 0);
            Brain.target = Shared.Player;

            Attention_range = 6;
            Persistence = 3;
            Knockback = 6f;
            True_Damage = 5;
            MAX_Health_Points = 10;

            Walk_Speed = 2f;
            Run_Speed = 3f + (Shared.Randomizer.Next(2, 4)); // Varying RunSpeed ... for fun. Temporary.
            Jump_Strength = 11f;
            Team = Shared.Damage_Team.HOSTILE;

            Health_Points = MAX_Health_Points;
            Focus_Points = MAX_Focus_Points;

            Health_Cache = MAX_Health_Points;

            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // Animation Zone
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            // -----------------------
            #region Animations

            // Idle / Default Bound Box
            // -----------------------
            State_Boundary_Offset.Add(
                Shared.EntityState.IDLE,
                new Rectangle
                    (
                    0,
                    10,
                    34,
                    20
                    ));
            //Generate Animations based on supplied sheet
            // -----------------------

            int[] delay;
            // Idle State
            // -----------------------
            delay = new int[8];
            // -----------------------
            delay[0] = 20; // Delay Pre-Blink
            delay[2] = 20; // Delay Pre-Blink

            Animation_state.Add(
                Shared.EntityState.IDLE,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 1, 8, delay));
            // Idle / Default Bound Box
            // -----------------------

            // Walking State
            // -----------------------
            // Walking Delay
            Animation_state.Add(
                Shared.EntityState.WALKING,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 2, 6));

            // Jumping State
            // -----------------------
            Animation_state.Add(
                Shared.EntityState.JUMPING,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 3, 1, null, false));

            #endregion
            // -----------------------
        }

        // -------------------------------
        // Goblin AI
        // -------------------------------

        /// <summary>
        /// Overriden Goblin AI behavior
        /// </summary>
        public override void NPC_Behaviors()
        {
            if (Enabled)
            {
                // -------------------------------
                base.NPC_Behaviors();
                // -------------------------------

                if (!busy)
                {
                    if (!Brain.Pursue())
                    {
                        Brain.Return_To_Post();
                        Brain.Wander();
                    }
                }
            }
        }
        // -------------------------------
    }
}
