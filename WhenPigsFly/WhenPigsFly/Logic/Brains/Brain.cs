// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Basic Brain Class
// Created 11/30/2015
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
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Brain Stem
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #region Brain Stem (Variables, Declaration, and Thought Hub

        /// <summary>
        /// Basic brain class is the most basic functionality system for all NPCs
        /// </summary>
        public class Brain : Microsoft.Xna.Framework.GameComponent
        {
            // ---------------------
            // Essentials
            // ---------------------
            private Random random;
            private NPC host;

            // ---------------------
            // Timers
            // ---------------------
            public Dictionary<string, Clock> Timers;

            private Clock Pursue_Timer = new Clock();
            private Clock Wander_Timer = new Clock();
            private Clock Attack_Timer = new Clock();

            // ---------------------
            // Flags
            // ---------------------

            // ---------------------
            // Logical Data
            // ---------------------
            public Character target = null;

            /// <summary>
            /// The basic brain class - a simple thought AI that gives NPC's an essential kit of
            /// decision making tools
            /// </summary>
            /// <param name="host">The host NPC</param>
            public Brain(NPC host)
                : base(Shared.Main)
            {
                this.host = host;
                random = Shared.Randomizer;

                // Initialize necessary timers
                // -------------------------------
                Timers = new Dictionary<string, Clock>();

                Timers.Add("Pursue", Pursue_Timer);
                Timers.Add("Attack", Attack_Timer);
                Timers.Add("Wander", Wander_Timer);
                // -------------------------------
            }
            // -------------------------------

            /// <summary>
            /// 'Think' houses all of the brain classes processing abilities. Ticks with every update of its host
            /// </summary>
            /// <param name="gameTime">A snapshot of game time</param>
            public  void Think(GameTime gameTime)
            {
                // -------------------------------
                // Target Acquisition
                // -------------------------------

                // TO DO


                // -------------------------------
                // Run through Timers
                // -------------------------------
                foreach (KeyValuePair<string, Clock> timer in Timers)
                {
                    if (timer.Value.Time > 0f)
                    {
                        float elapsed_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Timers[timer.Key].Time -= elapsed_time;
                    }
                    else
                    {
                        Timers[timer.Key].Time = 0f;
                    }
                }
                // -------------------------------
            }
            #endregion

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Behavior Actions
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            /// <summary>
            /// Pursuit behavior - NPC chases target entity
            /// </summary>
            /// <returns>Returns true if able to chase target</returns>
            public bool Pursue()
            {
                // -------------------------------
                // Reference timer for ease of acess
                Clock pursue_timer = Timers["Pursue"];
                // -------------------------------
                if (target != null && !target.Is_dead())
                {
                    // -------------------------------
                    // Save result of Sight & Range check
                    bool Sight_And_Range_Check = Sight_And_Range(target);
                    // -------------------------------
                    if (!Sight_And_Range_Check && pursue_timer.Primary_Decision == 1)
                        pursue_timer.Time = host.Persistence;

                    if (Sight_And_Range_Check || pursue_timer.Time > 0f)
                    {
                        // -------------------------------
                        if (pursue_timer.Secondary_Decision == 0)
                        {
                            // -------------------------------
                            Shared.SFX_Sounds["ALERT"].Play();
                            Particles.Do_Effect(
                                "Alert",
                                1,
                                new Vector2(0, -((host.Get_Bounds().Height) / 2 + Particles.Effects["Alert"].Dimensions.Y)),
                                Vector2.Zero,
                                true,
                                host); 
                            // Ensures the alert emote only plays once per sighting
                            pursue_timer.Secondary_Decision = 1;
                            // -------------------------------
                        }
                        // -------------------------------
                        if (Sight_And_Range_Check)
                            pursue_timer.Primary_Decision = 1;
                        else
                            pursue_timer.Primary_Decision = 0;

                        // x_offset has our pursuer try to run past their target instead of atop them.
                        // -------------------------------
                        int x_offset = 1;
                        if (host.Direction == Shared.Direction.LEFT)
                            x_offset = -1;
                        // -------------------------------
                        Vector2 target_dest = new Vector2
                        (
                        target.Grid_Position.X + x_offset,
                        target.Grid_Position.Y
                        );

                        if (host.Grid_Position == target_dest)
                            Turn_Around();

                        if (host.Direction == Shared.Direction.RIGHT
                            && host.Grid_Position.X > target.Grid_Position.X)
                            Turn_Around();
                        else if (host.Direction == Shared.Direction.LEFT
                            && host.Grid_Position.X < target.Grid_Position.X)
                            Turn_Around();

                        Chase(target_dest, "run");
                        // -------------------------------
                        return true;
                    }
                    else if(pursue_timer.Secondary_Decision == 1)
                    {
                        // -------------------------------
                        Shared.SFX_Sounds["CONFUSED"].Play();
                        Particles.Do_Effect(
                            "Confused",
                            2,
                            new Vector2(0, -((host.Get_Bounds().Height / 2) + Particles.Effects["Alert"].Dimensions.Y)),
                            Vector2.Zero,
                            true,
                            host); 
                        // Ensures the confused emote only plays once per sighting
                        pursue_timer.Secondary_Decision = 0;
                        // -------------------------------
                    }
                }
                else // Target is null! They don't exist no more! Or never did! Set to defaults.
                {
                    pursue_timer.Time = 0f;
                    pursue_timer.Primary_Decision = 0;
                    pursue_timer.Secondary_Decision = 0;
                }
                return false;
            }
            // ----------------------------------

            /// <summary>
            /// Wander behavior- brings the NPC to life by having them walk and turn around randomly
            /// </summary>
            public void Wander()
            {
                // -------------------------------
                // Reference timer for ease of acess
                Clock inner_timer = Timers["Wander"];
                // -------------------------------

                if (inner_timer.Time <= 0f)
                {
                    // -------------------------------
                    // Pull a random behavior, for a random duration
                    inner_timer.Primary_Decision = random.Next(0, 3);
                    inner_timer.Time = random.Next(1, 4);
                    // -------------------------------
                }
                // -------------------------------

                if (inner_timer.Primary_Decision == 2) // Move around decision
                    Safe_Walk(host.Direction);

                if (inner_timer.Primary_Decision == 1) // Turn, No movement decision
                {
                    Turn_Around();
                    inner_timer.Primary_Decision = 0; // Makes sure they don't just keep spinning!!
                }
            }
            // ----------------------------------

            // ----------------------------------
            /// <summary>
            /// Moves the host NPC back to a distance around its post position
            /// </summary>
            public void Return_To_Post()
            {
                float distance_from_post_X = Math.Abs((host.Grid_Position.X - host.Post_Position.X));
                //float distance_from_post_Y = Math.Abs((host.Grid_Position.Y - host.Post_Position.Y));

                if (distance_from_post_X > host.Wander_range) // || distance_from_post_Y > host.Wander_range)
                    Smart_Movement(host.Post_Position);
            }
            // ----------------------------------

            /// <summary>
            /// A very simple method that just turns the host around
            /// </summary>
            public void Turn_Around()
            {
                if (host.Direction == Shared.Direction.LEFT)
                    host.Direction = Shared.Direction.RIGHT;
                else
                    host.Direction = Shared.Direction.LEFT;
            }
            // ----------------------------------


            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // Logic Checks & Decision Making
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            #region Logic Checks
            // ----------------------------------
            /// <summary>
            /// Can Chase check
            /// </summary>
            public bool Sight_And_Range(Character target)
            {
                if (target != null)
                {
                    Shared.Direction range_direction;
                    bool in_range;

                    CollisionManager.In_Range(
                        host,
                        target,
                        host.Attention_range,
                        out in_range,
                        out range_direction);
                    if (in_range)
                    {
                        if (range_direction == host.Direction || target.Grid_Position == host.Grid_Position)
                        {
                            if (CollisionManager.Line_Of_Sight(host, target, host.Attention_range))
                            {
                                return true; // NPC is facing the target, and has LoS and range. CHASE! - Or, they're right on top of us.
                            }
                        }
                    }
                }
                return false;
            }
            // ----------------------------------

            // ----------------------------------
            /// <summary>
            /// Smart Navigation guides the host around terrain to a specified grid destination
            /// </summary>
            /// <param name="destination">The destination our host wants to move to</param>
            public void Smart_Movement(Vector2 destination, string movement = "walk")
            {
                // ----------------------------------
                bool horizontal_tracking = false;

                Vector2 source_pos = host.Grid_Position;
                Shared.Direction dest_direction = Shared.Direction.NONE;

                // ----------------------------------
                // If the destination X is greater, it's too the right
                if (destination.X > source_pos.X)
                {
                    host.Direction = Shared.Direction.RIGHT;
                    dest_direction = Shared.Direction.RIGHT;
                    horizontal_tracking = true;
                }
                // ----------------------------------
                // Less than, it's too the left.
                else if (destination.X < source_pos.X)
                {
                    host.Direction = Shared.Direction.LEFT;
                    dest_direction = Shared.Direction.LEFT;
                    horizontal_tracking = true;
                }

                // ----------------------------------
                // Begin tracking toward horizontal destination
                if (horizontal_tracking)
                {
                    if (CollisionManager.Safe_Step(host))
                    {
                        if (movement == "run")
                            host.Try_Run(dest_direction);
                        else
                            host.Try_Walk(dest_direction);
                    }
                    else if (CollisionManager.Safe_Drop(host))
                    {
                        if (movement == "run")
                            host.Try_Run(dest_direction);
                        else
                            host.Try_Walk(dest_direction);
                    }
                    // ----------------------------------
                    // We've hit a wall. Jump to see if that resolves our oh so
                    // complicated dilemma 
                    if (CollisionManager.Wall_Collision(host, dest_direction))
                    {
                        if (Shared.Active_World.Get_Terrain_At((int)destination.X, (int)destination.Y) == null)
                        {
                            if (CollisionManager.Jumpable_Terrain(host))
                                host.Try_Jump();
                        }
                    }
                }
            }

            // ----------------------------------
            /// <summary>
            /// Chase is similar to Smart Movement, but much more reckless.
            /// </summary>
            /// <param name="destination">The destination our host wants to move to</param>
            public void Chase(Vector2 destination, string movement = "walk")
            {
                // ----------------------------------
                bool horizontal_tracking = false;

                Vector2 source_pos = host.Grid_Position;
                Shared.Direction dest_direction = Shared.Direction.NONE;

                // ----------------------------------
                // If the destination X is greater, it's too the right
                if (destination.X > source_pos.X)
                {
                    host.Direction = Shared.Direction.RIGHT;
                    dest_direction = Shared.Direction.RIGHT;
                    horizontal_tracking = true;
                }
                // ----------------------------------
                // Less than, it's too the left.
                else if (destination.X < source_pos.X)
                {
                    host.Direction = Shared.Direction.LEFT;
                    dest_direction = Shared.Direction.LEFT;
                    horizontal_tracking = true;
                }

                // ----------------------------------
                // Begin tracking toward horizontal destination
                if (horizontal_tracking)
                {
                    if (!CollisionManager.Safe_Step(host) && destination.Y <= host.Grid_Position.Y)
                        host.Try_Jump();

                    if (movement == "run")
                            host.Try_Run(dest_direction);
                        else
                            host.Try_Walk(dest_direction);
                    // ----------------------------------
                    // We've hit a wall. Jump to see if that resolves our oh so
                    // complicated dilemma 
                    if (CollisionManager.Wall_Collision(host, dest_direction))
                    {
                        if (Shared.Active_World.Get_Terrain_At((int)destination.X, (int)destination.Y) == null)
                        {
                            if (CollisionManager.Jumpable_Terrain(host))
                                host.Try_Jump();
                        }
                    }
                }
            }

            // ----------------------------------
            /// <summary>
            /// Safe walk logic- keeps NPC's from stepping to their DOOM
            /// decision making
            /// </summary>
            /// <param name="direction">The direction to walk</param>
            public bool Safe_Walk(Shared.Direction direction)
            {
                host.Direction = direction;

                if (CollisionManager.Safe_Step(host))
                {
                    if (CollisionManager.Wall_Collision(host, direction))
                    {
                        if (CollisionManager.Jumpable_Terrain(host))
                            host.Try_Jump();
                        else if (!host.Airborne)
                            Turn_Around();
                    }

                    host.Try_Walk(direction);
                    return true;
                }
                else if (CollisionManager.Safe_Drop(host))
                {
                    host.Try_Walk(direction);
                    return true;
                }

                return false;
            }
            // ----------------------------------
            #endregion

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }
}
