// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Player Class
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
    /// Player Class : Handles all of the players stats and controls,
    /// As well as drawing and animation
    /// </summary>
    public class Player : Character
    {
        // ---------------------
        // Player Controls
        // ---------------------
        public Vector2 Respawn_Point;
        public float Respawn_Delay = 1f;
        public Clock Respawner = new Clock();
        public Clock Antispam = new Clock();

        private KeyboardState keyboard_cache;
        private MouseState mouse_cache;
        private float scroll_cache = 0;

        private Clock Cooldown_Clock = new Clock();
        // ---------------------
        // Flags
        // ---------------------
        public bool Holding_Jump = false;
        public bool Jump_lock = false;

        public float wall_jump_delay = 0.1f;
        public float wall_jump_timer = 0;

        // ---------------------
        // Boundaries
        // ---------------------
        private Rectangle Default_Bound_Offset = Rectangle.Empty;
        private Rectangle Crouch_Bound_Offset = Rectangle.Empty;


        // ---------------------
        /// <summary>
        /// Player Entity Class - All controls and stuff go here. Woo.
        /// </summary>
        /// <param name="game">Our Game</param>
        /// <param name="spriteBatch">The Spritebatch</param>
        public Player()
            : base()
        {
            // ---------------------
            this.spriteBatch = spriteBatch;
            this.SpriteSheet = Shared.IMG_Sprites["player"];
            Set_State(Shared.EntityState.IDLE);
            this.FrameSize = new Vector2(32, 32);
            // ---------------------
            // Stats
            // ---------------------
            Team = Shared.Damage_Team.FRIENDLY;
            Base_Inv_Time = 0.4f;


            // ---------------------
            #region Animations

            //Generate Animations based on supplied sheet
            // -----------------------
            int[] delay;
            // Idle State
            // -----------------------
            // Idle Delays
            delay = new int[7];
            // -----------------------
            delay[0] = 20; // Delay Pre-Blink
            delay[3] = 20; // Delay Pre-Glance
            delay[4] = 5; // Hold Close Glance
            delay[6] = 5; // Hold Far Glance

            Animation_state.Add(
                Shared.EntityState.IDLE,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 1, 7, delay));
            // Idle / Default Bound Box
            // -----------------------
            State_Boundary_Offset.Add(
                Shared.EntityState.IDLE,
                new Rectangle
                    (
                    3,
                    4,
                    20,
                    8
                    ));

            // Crouch State
            // -----------------------
            Animation_state.Add(
                Shared.EntityState.CROUCHING,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 2, 3, null, false));
            // Crouch Bound Box
            // -----------------------
            State_Boundary_Offset.Add(
                Shared.EntityState.CROUCHING,
                new Rectangle
                    (
                    3,
                    10,
                    20,
                    20
                    ));

            // Walking State
            // -----------------------
            // Walking Delay
            delay = new int[6];
            for (int item = 0; item < delay.Length; item++)
            { delay[item] = 1; }
            // -----------------------
            Animation_state.Add(
                Shared.EntityState.WALKING,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 3, 6, delay));
            // Running State
            Animation_state.Add(
                Shared.EntityState.RUNNING,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 3, 6));

            // Jump Wind State
            // -----------------------
            Animation_state.Add(
                Shared.EntityState.JUMP_WIND,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 4, 1, null, false));

            // Jumping State
            // -----------------------
            // Jumping Delay
            delay = new int[3];
            for (int item = 0; item < delay.Length; item++)
            { delay[item] = 1; }
            // -----------------------
            Animation_state.Add(
                Shared.EntityState.JUMPING,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 5, 3, delay));

            // Falling State
            // -----------------------
            // Falling Delay
            delay = new int[2];
            for (int item = 0; item < delay.Length; item++)
            { delay[item] = 1; }
            // -----------------------
            Animation_state.Add(
                Shared.EntityState.FALLING,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 6, 2, delay));

            // Basic Attack Animation
            // -----------------------

            // Side Attack
            delay = new int[1] { 1 };
            Animation_state.Add(
                Shared.EntityState.SIDE_ATTACK,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 4, 1, delay, false, 1));
            // Up Attack
            Animation_state.Add(
                Shared.EntityState.UP_ATTACK,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 4, 1, delay, false, 2));
            // Down Attack
            Animation_state.Add(
                Shared.EntityState.DOWN_ATTACK,
                AnimationHandler.Generate(SpriteSheet, FrameSize, 4, 1, delay, false, 3));

            // -----------------------

            #endregion
            // ---------------------


            // Basic Attack
            // -----------------------

            Attack Basic = new Attack();
            Basic.Damage = 2;
            Basic.Knockback = 2f;
            Basic.Lifespan = 1f;
            Basic.Cooldown = 0.4f;

            Basic.SFX = Shared.SFX_Sounds["Throw"];
            Basic.Effect_Name = "Dagger";
            Basic.Directional_Velocity = 11f;
            Basic.Source_Offset = new Vector2(0, 10);

            Basic.Damage_Type = Shared.Type.MUNDANE;
            Basic.Damage_Team = Shared.Damage_Team.FRIENDLY;
            Basic.Hit_Frame = 0;
            Basic.Source = this;
            Basic.Follow_Source = false;
            Basic.Inherit_Velocity = true;

            Attack_Roster.Add("basic", Basic);

            // Arcane Bolt
            // -----------------------

            Attack Arcane_Bolt = new Attack();
            Arcane_Bolt.Damage = 6;
            Arcane_Bolt.Focus_Cost = 3;
            Arcane_Bolt.Knockback = 15f;
            Arcane_Bolt.Lifespan = 1f;
            Arcane_Bolt.Cooldown = 0.5f;

            Arcane_Bolt.SFX = Shared.SFX_Sounds["arcane_bolt"];
            Arcane_Bolt.Effect_Name = "Arcane_Bolt";
            Arcane_Bolt.Directional_Velocity = 10f;
            Arcane_Bolt.Source_Offset = new Vector2(0, 10);

            Arcane_Bolt.Damage_Type = Shared.Type.ARCANE;
            Arcane_Bolt.Damage_Team = Shared.Damage_Team.FRIENDLY;
            Arcane_Bolt.Hit_Frame = 0;
            Arcane_Bolt.Source = this;
            Arcane_Bolt.Follow_Source = false;
            Arcane_Bolt.Inherit_Velocity = true;

            Attack_Roster.Add("Arcane_Bolt", Arcane_Bolt);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if(Enabled)
            {
                // -------------------------------
                MouseState mouse_interface = Mouse.GetState();
                KeyboardState keyboard_interface = Keyboard.GetState();

                // --------------------
                #region Respawn Control
                // --------------------
                // Respawn Clock
                // --------------------
                if (Is_dead())
                {
                    if (Respawner.Time > 0f)
                    {
                        float elapsed_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Respawner.Time -= elapsed_time;
                    }
                    else
                    {
                        Respawner.Time = 0f;
                        if (Is_dead())
                        {
                            if (keyboard_interface.IsKeyDown(Keys.Enter) && keyboard_cache.IsKeyUp(Keys.Enter))
                            {
                                Revive();
                            }
                        }
                    }
                }
                #endregion
                // --------------------

                // --------------------
                #region Anti Spam Clock
                // --------------------
                // Anti Spam Clock
                // --------------------

                    if (Antispam.Time > 0f)
                    {
                        float elapsed_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Antispam.Time -= elapsed_time;
                    }
                    else
                        Antispam.Time = 0f;

                #endregion
                // --------------------

                // -------------------------------
                //Player Controls
                // -------------------------------
                #region Controls
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Zooming 
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                float current_scroll = mouse_interface.ScrollWheelValue;
                if (current_scroll != scroll_cache)
                {
                    if (!Shared.ZOOM_LOCK)
                    {
                        Shared.ZOOM_LEVEL += (float)(Math.Round((current_scroll - scroll_cache)) * 0.005f);

                        if (Shared.ZOOM_LEVEL > Shared.MAX_ZOOM)
                        {
                            Shared.ZOOM_LEVEL = Shared.MAX_ZOOM;
                        }
                        if (Shared.ZOOM_LEVEL < Shared.MIN_ZOOM && Shared.TEMP_MIN_ZOOM == -1)
                        {
                            Shared.ZOOM_LEVEL = Shared.MIN_ZOOM;
                        }
                        else if (Shared.ZOOM_LEVEL < Shared.TEMP_MIN_ZOOM)
                        {
                            Shared.ZOOM_LEVEL = Shared.TEMP_MIN_ZOOM;
                        }
                    }
                    scroll_cache = current_scroll;
                }
                // -----------------------------------------------
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Idle 
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                Try_Idle();

                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Attack Swapping 
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                if (keyboard_interface.IsKeyDown(Keys.D1))
                    Equipped_Attack = "basic";

                if (keyboard_interface.IsKeyDown(Keys.D2))
                    Equipped_Attack = "Arcane_Bolt";

                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Crouching 
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                if (keyboard_interface.IsKeyDown(Shared.MOVE_Crouch))
                {
                    Try_Crouch();
                }
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Left Movement 
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                if (keyboard_interface.IsKeyDown(Shared.MOVE_Left))
                {
                    if (keyboard_interface.IsKeyDown(Shared.MOVE_Run))
                        Try_Run(Shared.Direction.LEFT);
                    else
                        Try_Walk(Shared.Direction.LEFT);
                }
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Right Movement 
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                if (keyboard_interface.IsKeyDown(Shared.MOVE_Right))
                {
                    if (keyboard_interface.IsKeyDown(Shared.MOVE_Run))
                        Try_Run(Shared.Direction.RIGHT);
                    else
                        Try_Walk(Shared.Direction.RIGHT);
                }
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Jumping 
                // ~~~~~~~~~~~~~~~~~~~~~~~~

                // ------------------------
                #region Jump Controls

                // This lil' toggle informs us of the player holding jump. However, if they release
                // jump midair, they'll be locked out until landing.
                if (keyboard_interface.IsKeyDown(Shared.MOVE_Jump) 
                    && !Jump_lock)
                    Holding_Jump = true;
                else
                {
                    Holding_Jump = false;
                    Jump_lock = false;

                    if (has_jumped)
                        Jump_lock = true;
                }

                if (has_jumped && !Holding_Jump)
                    Jump_lock = true;

                if (keyboard_interface.IsKeyDown(Shared.MOVE_Jump) 
                    && keyboard_interface.IsKeyDown(Shared.MOVE_Jump) != keyboard_cache.IsKeyDown(Shared.MOVE_Jump))
                {
                    Try_Jump();
                }

                if (keyboard_interface.IsKeyDown(Shared.MOVE_Jump) && has_jumped 
                    && keyboard_interface.IsKeyDown(Shared.MOVE_Jump) != keyboard_cache.IsKeyDown(Shared.MOVE_Jump))
                {
                    Try_Wall_Jump();
                }
                #endregion
                // ------------------------

                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Attacking
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                if (keyboard_interface.IsKeyDown(Shared.ATK_Right))
                {
                    ATK_Direction = Shared.Direction.RIGHT;
                    Try_Attack(Equipped_Attack);
                }

                if (keyboard_interface.IsKeyDown(Shared.ATK_Left))
                {
                    ATK_Direction = Shared.Direction.LEFT;
                    Try_Attack(Equipped_Attack);
                }

                if (keyboard_interface.IsKeyDown(Shared.ATK_Up))
                {
                    ATK_Direction = Shared.Direction.UP;
                    Try_Attack(Equipped_Attack);
                }

                if (keyboard_interface.IsKeyDown(Shared.ATK_Down))
                {
                    ATK_Direction = Shared.Direction.DOWN;
                    Try_Attack(Equipped_Attack);
                }

                #endregion
                // -------------------------------

                // ~~~~~~~~~~~~~~~~~~~~~~~~
                // Debugging 
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                if (keyboard_interface.IsKeyDown(Shared.KEY_Debug) && (keyboard_interface != keyboard_cache))
                {
                    Shared.Debug = !Shared.Debug;
                }

                if (Shared.Debug)
                {
                     Health_Points = MAX_Health_Points;
                     Focus_Points = MAX_Focus_Points;
                }
                // -------------------------------
                mouse_cache = mouse_interface;
                keyboard_cache = keyboard_interface;
                // -------------------------------

                Shared.DebugWindow.Message += "Player Location: X: " + Grid_Position.X + ", Y: " + Grid_Position.Y + "\n";
                Shared.DebugWindow.Message += "Player HP: " + Health_Points + "\n";
                Shared.DebugWindow.Message += "Active Effects: " + Particles.Active_Effects.Count() + "\n";

                // --------------------
                // Game Camera
                // --------------------
                Shared.Screen.Set_Zoom(Shared.ZOOM_LEVEL);
                Shared.Screen.Set_Focus(Shared.Player.Position);
                // --------------------
            }

            // -------------------------------
            // Base character updates
            base.Update(gameTime);
            // -------------------------------
        }

        // -------------------------------
        /// <summary>
        /// Revives the player
        /// </summary>
        /// <param name="penalty">Precentage penalty to revival health</param>
        public void Revive(float penalty = 0f)
        {
            if (Is_dead())
            {
                Shared.SFX_Sounds["Revive"].Play();

                Health_Points = (int)(MAX_Health_Points - (MAX_Health_Points * penalty));
                Focus_Points = (int)(MAX_Focus_Points - (MAX_Focus_Points * penalty));
                Position = Respawn_Point;
                Velocity = Vector2.Zero;
                External_Velocity = Vector2.Zero;
                dead = false;
            }
        }
        // -------------------------------

        /// <summary>
        /// Allows the player to wall jump!
        /// </summary>
        /// <param name="speed">The horizontal velocity of said wall jump</param>
        public void Wall_Jump(float speed)
        {
            // --------------------------
            // Jumping Action
            // --------------------------
            Shared.SFX_Sounds["Wall_Jump"].Play();
            Velocity.Y = (float)-(Jump_Strength * 1.25);
            External_Velocity.X += (speed * 2);
        }

        // ----------------------------------
        /// <summary>
        /// Try player wall jump
        /// </summary>
        public bool Try_Wall_Jump()
        {
            if (!dead && !busy && airborne)
            {
                // ----------------------------------
                // Wall Jumping Logic
                if (CollisionManager.Wall_Collision(this, Shared.Direction.LEFT)
                    && direction == Shared.Direction.RIGHT)
                {
                    Wall_Jump(7f);
                }
                else if (CollisionManager.Wall_Collision(this, Shared.Direction.RIGHT)
                        && direction == Shared.Direction.LEFT)
                {
                    Wall_Jump(-7f);
                }
                // ----------------------------------
            }
            return false;
        }
        // ----------------------------------

        // ----------------------------------
        /// <summary>
        /// Overrides Character Draw- does not disable character when dead, simple prevents drawing them.
        /// This allows player to continue interfacing
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (!dead)
                base.Draw(gameTime);
        }
        // ----------------------------------
    }
}
