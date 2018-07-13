// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Character Abstract Class
// Created 11/28/2015
// ------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhenPigsFly
{
    /// <summary>
    /// Character class extends Entity, holding vital flags & methods used by
    /// most character entities. Players, NPCS, Enemies, Etc.
    /// </summary>
    public abstract class Character : Entity
    {
        // ---------------------
        // Stats
        // ---------------------
        public int MAX_Health_Points = 20;
        public int Health_Points = 20;
        public int MAX_Focus_Points = 10;
        public int Focus_Points = 10;

        public int Strength = 5;
        public int Fortitude = 5;
        public int Intelligence = 5;

        public int Physical_Defence = 5;
        public int Arcane_Defence = 5;

        public int True_Damage = 1;
        public float Knockback = 0f;
        public Shared.Type Damage_type = Shared.Type.MUNDANE;
        public Shared.Damage_Team Team = Shared.Damage_Team.NEUTRAL;

        public float Walk_Speed = 5f;
        public float Run_Speed = 8f;
        public float Jump_Strength = 10f;

        public bool Invulnerable = false;
        public float Base_Inv_Time = 0.2f;
        private float inv_counter = 0f;

        // ---------------------
        // Animation
        // ---------------------
        private int anim_index = 0;
        private int anim_delay = 0;

        private Shared.EntityState State_Cache;

        public Shared.EntityState State;

        protected Dictionary<Shared.EntityState, Rectangle> State_Boundary_Offset
            = new Dictionary<Shared.EntityState, Rectangle>();

        protected Dictionary<Shared.EntityState, Animation> Animation_state
            = new Dictionary<Shared.EntityState, Animation>();

        public string Active_Attack = "none";
        protected Dictionary<string, Attack> Attack_Roster
            = new Dictionary<string, Attack>();

        public Shared.Direction ATK_Direction = Shared.Direction.NONE;
        public string Equipped_Attack = "basic";

        // ---------------------
        // Flags
        // ---------------------
        private bool animated = true;
        private bool final_frame = false;

        protected bool busy = false;
        protected bool dead = false;
        protected bool health_changed = false;

        protected bool idle = true;
        protected bool crouched = false;
        protected bool walking = false;
        protected bool running = false;
        protected bool winding_jump = false;
        protected bool has_jumped = false;
        protected bool jumping = false;
        protected bool airborne = false;
        private bool on_ground = false;
        private bool falling = false;
        protected bool flying = false;
        // ---------------------

        // ---------------------
        // Velocity & Physics 
        // ---------------------
        public Vector2 Velocity = Vector2.Zero;
        public Vector2 External_Velocity = Vector2.Zero;

        private float fall_time = 0;
        private float Airborne_Velocity = 0f;
        private const float Resistance = 0.90f;

        // ---------------------
        #region Getters & Setters

        public bool On_ground
        { get { return on_ground; } set { on_ground = value; } }
        public bool Airborne
        { get { return airborne; } }
        public bool Animated
        { get { return animated; }}
        public int Anim_index
        { get { return anim_index; } set { anim_index = value; } }
        public bool Final_frame
        {get { return final_frame; }}
        public Dictionary<string, Attack> Get_Attacks
        { get { return Attack_Roster; } }
        #endregion
        // ---------------------

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Physics & Movement
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // --------------------------
        #region Physics & Movement

        /// <summary>
        /// Applies physics to character entities every tick
        /// </summary>
        /// <param name="gameTime">Snapshot of gametime</param>
        public void Physics(GameTime gameTime)
        {

            // --------------------------
            // Basic Gravity Physics
            // --------------------------

            // --------------------------
            // Horizontal Trajectory Logic & Resistance
            // --------------------------

            // --------------------------
            // Log potential airborne velocity before leaving the ground
            if (!airborne)
                Airborne_Velocity = Velocity.X;
            // --------------------------
            if (!walking && !running && !airborne)
                Velocity.X -= Velocity.X * (Resistance);
            else
            {
                float x_speed = 0f;
                // --------------------------
                if (walking)
                    x_speed = Walk_Speed;
                if (running)
                    x_speed = Run_Speed;
                // --------------------------
                if (direction == Shared.Direction.LEFT)
                    x_speed = -x_speed;
                // --------------------------

                // If we're airborne, velocity works differently! Who knew!
                if(airborne)
                {
                    // --------------------------
                    Airborne_Velocity = Airborne_Velocity * 0.95f;
                    Airborne_Velocity += x_speed * 0.05f;

                    // --------------------------
                    //Ensure no ridiculous acceleration.
                    if (Math.Abs(Airborne_Velocity) > Math.Abs(x_speed) && x_speed != 0)
                        Airborne_Velocity = x_speed;

                    Velocity.X = Airborne_Velocity;
                    // --------------------------
                }
                else
                    Velocity.X = x_speed * (Resistance);
                // --------------------------

                // --------------------------
                // Set to false to prevent 'stick'
                walking = false;
                running = false;
                // --------------------------
            }

            // --------------------------
            // Apply External Velocity
            if (External_Velocity != Vector2.Zero)
            {
                if (Math.Abs(External_Velocity.X) < 0.05f)
                    External_Velocity.X = 0f;
                if (Math.Abs(External_Velocity.Y) < 0.05f)
                    External_Velocity.Y = 0f;

                Velocity += External_Velocity;
                External_Velocity = External_Velocity * 0.9f;
            }
            // --------------------------

            // If velocity becomes super tiny, just set to zero
            if (Math.Abs(Velocity.X) < 0.01f)
                Velocity.X = 0;
            // --------------------------

            //Always run ground collision test incase of floor no longer ... existing.
            // --------------------------
            On_ground = CollisionManager.Ground_Collision(this);
            if (On_ground)
            {
                airborne = false;
                if (Velocity.Y > 0f)
                {
                    Velocity.Y = 0f;
                    if (External_Velocity.Y > 0f)
                        External_Velocity.Y = 0f;
                }
            }
            else
                airborne = true;


            if (On_ground && !jumping)
            {
                falling = false;
                has_jumped = false;
            }

            if (Velocity.Y >= 0)
                jumping = false;

            // Jump Velocity Logic
            // --------------------------
            if (!On_ground)
            {
                // We're not on the ground, so we have to start
                // calculating our trajectory
                // ------------------------
                if (!falling)
                    falling = true;
                    fall_time = 0;
                // ------------------------
                if (falling)
                {
                    fall_time += (float)gameTime.ElapsedGameTime.Milliseconds / 650;

                    // ------------------------
                    // Special case for Player character, allowing jump hold & hover
                    if (this is Player)
                    {
                        Player player = (Player)this;
                        if (Velocity.Y > 0)
                        {
                            player.Holding_Jump = false;
                        }

                        if (player.Holding_Jump)
                        {
                            Velocity -= (Shared.Gravity * fall_time); 
                        }
                        else
                        {
                            Velocity -= ((Shared.Gravity * fall_time) * 2f);
                        }
                    }
                    // ------------------------
                    else
                    {
                        Velocity -= (Shared.Gravity * fall_time); 
                    }
                }
            }

            // --------------------------
            // Ensures proper state animations play
            // Depending on vertical velocity
            // --------------------------
            if (Velocity.Y < 0 && airborne)
            {
                Set_State(Shared.EntityState.JUMPING);
            }
            else if (Velocity.Y > 0 && airborne)
            {
                Set_State(Shared.EntityState.FALLING);
            }

            // Gravity = Maximum Falling Velocity for all entities
            // --------------------------
            if (Velocity.Y > -Shared.Gravity.Y)
            {
                Velocity.Y = -Shared.Gravity.Y;
            }

            // --------------------------
            // Wall Collisions
            // --------------------------
            if (Velocity.X != 0 || walking || running || airborne)
            {
                if (CollisionManager.Wall_Collision(this, Shared.Direction.RIGHT))
                {
                    if (Velocity.X > 0f)
                        Velocity.X = 0f;

                    if (External_Velocity.X > 0f)
                        External_Velocity.X = 0f;
                }

                if (CollisionManager.Wall_Collision(this, Shared.Direction.LEFT))
                {
                    if (Velocity.X < 0f)
                        Velocity.X = 0f;

                    if (External_Velocity.X < 0f)
                        External_Velocity.X = 0f;
                }
            }

            // --------------------------
            // Ceiling Check
            // --------------------------
            if (Velocity.Y < 0f)
            {
                if (CollisionManager.Ceiling_Collision(this))
                {
                    Velocity.Y = 0f;

                    if (External_Velocity.Y < 0f)
                        External_Velocity.Y = 0f;
                }
            }

            // --------------------------
            // Update Position
            // --------------------------

            Position += Velocity;
        }

        /// <summary>
        /// Causes the character to jump
        /// </summary>
        public void Jump()
        {
            // --------------------------
            // Jumping Action
            // --------------------------
            if (this is Player)
                Shared.SFX_Sounds["Jump"].Play();

            jumping = true;
            Velocity.Y = -(Jump_Strength);
        }
        #endregion
        // --------------------------

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Character Actions
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Varying Methods allow characters to change states
        // ----------------------------------
        #region Action Controllers

        // ----------------------------------
        /// <summary>
        /// Sets the entity state
        /// </summary>
        /// <param name="new_state">State to set</param>
        public void Set_State(Shared.EntityState new_state)
        {
            // Include all contingencies
            if (Active_Attack == "none")
            {
                State = new_state;
            }
        }
        // ----------------------------------
        /// <summary>
        /// Try entity attack clear - Clears Attack data
        /// </summary>
        public void Try_Attack_Clear()
        {
            Active_Attack = "none";
        }
        // ----------------------------------
        /// <summary>
        /// Try entity idle
        /// </summary>
        public bool Try_Idle()
        {
            if (!busy && Active_Attack == "none")
            {
                walking = false;
                running = false;
                Set_State(Shared.EntityState.IDLE);
                return true;
            }
            return false;
        }
        // ----------------------------------
        /// <summary>
        /// Try entity crouch
        /// </summary>
        public bool Try_Crouch()
        {
            if (!dead && !busy)
            {
                if (airborne)
                {
                    return false;
                }
                else
                {
                    Set_State(Shared.EntityState.CROUCHING);
                    return true;
                }
            }
            return false;
        }
        // ----------------------------------
        /// <summary>
        /// Try entity walk
        /// </summary>
        public virtual bool Try_Walk(Shared.Direction direction)
        {
            if (!dead && !busy)
            {
                walking = true;
                running = false;
                this.direction = direction;
                Set_State(Shared.EntityState.WALKING);
                return true;
            }
            return false;
        }
        // ----------------------------------
        /// <summary>
        /// Try entity run
        /// </summary>
        public bool Try_Run(Shared.Direction direction)
        {
            // Check all possible interferrence
            if (!dead && !busy)
            {
                walking = false;
                running = true;
                this.direction = direction;
                Set_State(Shared.EntityState.RUNNING);
                return true;
            }
            return false;
        }
        // ----------------------------------
        /// <summary>
        /// Try entity jump && Wall Jump
        /// </summary>
        public virtual bool Try_Jump()
        {
            if (!dead && !busy && !airborne)
            {
                if (!has_jumped)
                {
                    has_jumped = true;
                    Jump();
                    Set_State(Shared.EntityState.JUMPING);
                    return true;
                }
            }
            return false;
        }
        // ----------------------------------
        /// <summary>
        /// Try entity dead
        /// </summary>
        public void Try_Dead()
        {
            if (!dead)
            {
                Shared.SFX_Sounds["Overflow"].Play();

                Particles.Do_Bloodspurt(this, 20, 5);

                Particles.Do_Effect(
                    "Death",
                    3,
                    Vector2.Zero,
                    Vector2.Zero,
                    false,
                    this);

                if (this is Player)
                    Shared.Player.Respawner.Time = Shared.Player.Respawn_Delay;
            }
            dead = true;
        }
        // ----------------------------------
        /// <summary>
        /// Entity dead query
        /// </summary>
        public bool Is_dead()
        {
            return dead;
        }
        // ----------------------------------
        /// <summary>
        /// Try entity attack
        /// </summary>
        public bool Try_Attack(string attack = "basic")
        {
            if (Attack_Roster.ContainsKey(attack))
            {
                // Save the current attack for ease of access
                // ----------------------------------
                Attack current_attack = Attack_Roster[attack];
                // ----------------------------------

                // Check focus costs
                // ----------------------------------
                if (current_attack.Focus_Cost > 0)
                {
                    if(!(Focus_Points >= current_attack.Focus_Cost))
                    {
                        if(this is Player && Shared.Player.Antispam.Time == 0)
                        {
                            Shared.SFX_Sounds["No_Focus"].Play();
                            Shared.Player.Antispam.Time = 0.5f;
                        }
                        return false;
                    }
                }

                if (!dead && !busy && current_attack.Cooldown_timer == 0f)
                {
                    if (ATK_Direction != Shared.Direction.NONE)
                    {
                        // ----------------------------------
                        // Side Attacks
                        if (ATK_Direction == Shared.Direction.LEFT
                            || ATK_Direction == Shared.Direction.RIGHT)
                            Set_State(Shared.EntityState.SIDE_ATTACK);
                        // ----------------------------------
                        // Up Attacks
                        if (ATK_Direction == Shared.Direction.UP)
                            Set_State(Shared.EntityState.UP_ATTACK);
                        // ----------------------------------
                        // Down Attacks
                        if (ATK_Direction == Shared.Direction.DOWN)
                            Set_State(Shared.EntityState.DOWN_ATTACK);
                        // ----------------------------------
                    }
                    // ----------------------------------
                    // Attack isn't directional, just set to attack
                    else
                        Set_State(Shared.EntityState.ATTACKING);
                    // ----------------------------------

                    // ----------------------------------
                    // Do attacky things.
                    if (current_attack.SFX != null)
                        current_attack.SFX.Play();

                    current_attack.Cooldown_timer = current_attack.Cooldown;

                    Focus_Points -= current_attack.Focus_Cost;

                    Anim_index = 0;
                    Active_Attack = attack;

                    Handle_Attack_Projectiles();
                    return true;
                    // ----------------------------------
                }
            }
            return false;
        }
        // ----------------------------------
        /// <summary>
        /// Handles the limitation & spawning of attack projectiles
        /// </summary>
        public void Handle_Attack_Projectiles()
        {
            if (Active_Attack != "none")
            {
                if (Attack_Roster[Active_Attack].Hit_Frame == Anim_index)
                {
                    // Velocity needs some extra calculations.
                    // ----------------------------------
                    Vector2 new_velocity = Attack_Roster[Active_Attack].Velocity;

                    if (Attack_Roster[Active_Attack].Directional_Velocity != 0)
                    {
                        float directional_velocity = Attack_Roster[Active_Attack].Directional_Velocity;

                        if (ATK_Direction == Shared.Direction.LEFT)
                            new_velocity = new Vector2(-directional_velocity, 0);
                        if (ATK_Direction == Shared.Direction.RIGHT)
                            new_velocity = new Vector2(directional_velocity, 0);
                        if (ATK_Direction == Shared.Direction.UP)
                            new_velocity = new Vector2(0, -directional_velocity);
                        if (ATK_Direction == Shared.Direction.DOWN)
                            new_velocity = new Vector2(0, directional_velocity);
                    }
                    // ---------------------
                    // Inherit velocity
                    // ---------------------
                    if (Attack_Roster[Active_Attack].Inherit_Velocity)
                    {
                        // -----------------------
                        // This ensures you won't get super crappy backwards shots
                        if ((Velocity.X > 0f && new_velocity.X > 0f)
                            || (Velocity.X < 0f && new_velocity.X < 0f))
                                new_velocity.X += (Velocity.X);
                        else if(new_velocity.X == 0f)
                            new_velocity.X += Velocity.X;
                        // -----------------------
                        new_velocity.Y += (Velocity.Y / 2);
                    }
                    // ---------------------

                    Particles.Do_Projectile
                        (
                        Attack_Roster[Active_Attack].Effect_Name,
                        Attack_Roster[Active_Attack].Lifespan,
                        Attack_Roster[Active_Attack].Source_Offset,
                        new_velocity,
                        Attack_Roster[Active_Attack].Damage,
                        Attack_Roster[Active_Attack].Knockback,
                        Attack_Roster[Active_Attack].Damage_Team,
                        Attack_Roster[Active_Attack].Damage_Type,
                        Attack_Roster[Active_Attack].Hit_Frame,
                        Attack_Roster[Active_Attack].Follow_Source,
                        this
                        );
                    // ----------------------------------
                }
            }
        }
        // ----------------------------------
        /// <summary>
        /// Inflicts damage upon the character- if HP reaches 0, the character dies. Oh no!
        /// </summary>
        /// <param name="Attacker">The attacker</param>
        /// <param name="damage">The raw damage value to inflict</param>
        /// <param name="hit_direction">The direction in which this projectile was moving</param>
        /// <param name="type">The type of damage inflicted</param>
        /// <param name="projectile">Projectile sender</param>
        public void Damage(
            Character Attacker,
            int damage, 
            Vector2 hit_velocity, 
            Shared.Type type = Shared.Type.MUNDANE,
            Projectile projectile = null)
        {
            Health_Points -= damage;
            Set_Invulnerability();
            Particles.Do_Bloodspurt(this, 5);

            // Only allows the entity to be knocked and interrupted if this attacker is strong enough
            // -----------------------------------
            Interrupt();
            Shared.SFX_Sounds["Hit"].Play();
            // Determine Knockback
            // -----------------------------------
            float knock_value = 0f;
            if (projectile != null)
                knock_value = projectile.Knockback;
            else
                knock_value = Attacker.Knockback;

            if (knock_value > 50)
                knock_value = 50;

            if (knock_value < 0)
                knock_value = 0;


            Vector2 knockback_velocity = Vector2.Zero;

            knockback_velocity.X = knock_value;

            if (hit_velocity.X < 0f)
                knockback_velocity.X = -knock_value;

            // Ensures that we don't rack up a swap of hits and break the sound barrier
            // ----------------------------------
            if (!(Math.Abs(External_Velocity.X) >= Math.Abs(knockback_velocity.X)))
                External_Velocity.X += knockback_velocity.X;

            if (!(Math.Abs(External_Velocity.Y) >= Math.Abs(knockback_velocity.Y)))
                External_Velocity.Y += knockback_velocity.Y;
        }
        // ----------------------------------

        // ----------------------------------
        /// <summary>
        /// Sets the characters invulnerability timer
        /// </summary>
        public void Set_Invulnerability()
        {
            inv_counter = Base_Inv_Time; // Do checks, minimums and stat bonuses
        }
        // ----------------------------------

        // ----------------------------------
        /// <summary>
        /// Basic Invulnerability timer management
        /// </summary>
        /// <param name="gameTime">Snapshot of game time</param>
        public void Invulnerablity(GameTime gameTime)
        {
            if (inv_counter > 0f)
            {
                Invulnerable = true;
                float elapsed_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                inv_counter -= elapsed_time;
            }
            else
            {
                Invulnerable = false;
                inv_counter = 0f;
            }
        }
        // ----------------------------------

        // ----------------------------------
        /// <summary>
        /// Interrupts the character- forcing them to stop any current actions
        /// </summary>
        public void Interrupt()
        {
            // Include all possible contigencies
            if (this is Player)
            {
                Shared.Player.Jump_lock = true;
            }

            Set_State(Shared.EntityState.IDLE);
            busy = false;
            Active_Attack = "none";
        }
        // ----------------------------------

        #endregion
        // ----------------------------------

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Draw & Update & Bounds
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // ----------------------------------
        #region Draw & Update & Bounds
        /// <summary>
        /// Updates animation, position, logic, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // --------------------------
            // Disable entities that are not within the screen
            // Hooray for optimization.
            // --------------------------

            // Leniance radius lets specific groupds of entities load from a
            // determined bounds outside the screen
            // --------------------------

            Shared.Scene = Shared.Screen.Get_WorldSpace();

            float leniance_range = 15 * Shared.Block_Dimension;

            if (Get_Bounds().Top > Shared.Scene.Bottom + leniance_range)
                Enabled = false;
            else if (Get_Bounds().Bottom < Shared.Scene.Top - leniance_range)
                Enabled = false;
            else if (Get_Bounds().Left > Shared.Scene.Right + leniance_range)
                Enabled = false;
            else if (Get_Bounds().Right < Shared.Scene.Left - leniance_range)
                Enabled = false;
            else
                Enabled = true;

            if (Get_Bounds().Top >= Shared.Active_World.Size.Y)
            {
                if (!dead)
                {
                    Health_Points = 0;
                    Particles.Do_Bloodspurt(this, 50, 4, true);
                }
            }


            if (Enabled && !dead)
            {
                // -------------------------------
                // Cooldown Control
                // -------------------------------
                if (Attack_Roster.Count() != 0)
                {
                    foreach (KeyValuePair<string, Attack> attack in Attack_Roster)
                    {
                        if (attack.Value.Cooldown_timer > 0f)
                        {
                            float elapsed_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                            attack.Value.Cooldown_timer -= elapsed_time;
                        }
                        else
                            attack.Value.Cooldown_timer = 0f;
                    }
                }
                // -------------------------------


                // -------------------------------
                // 0 Health = Dead Character. Derp
                if (Health_Points <= 0)
                    Try_Dead();
                // -------------------------------

                Physics(gameTime); // Runs physics checks
                Invulnerablity(gameTime); // Runs invulnerability timer

                // Jitter Fix
                // ------------------------
                Position.X = (float)Math.Round(Position.X);
                Position.Y = (float)Math.Round(Position.Y);
                // ------------------------

                Grid_Position.X = (int)Math.Round((Position.X / Shared.Pixel_Scale) / Shared.Block_Dimension);
                Grid_Position.Y = (int)Math.Ceiling((Position.Y / Shared.Pixel_Scale) / Shared.Block_Dimension);

                // Ensures we will not run out of range of our animation list
                if (Animated)
                {
                    Shared.EntityState temp_state = State;
                    // --------------------------
                    // Make sure animation set exists
                    if (!Animation_state.ContainsKey(State))
                    {
                        // The animation does not exist. Default animation to idle
                        // --------------------------

                        temp_state = Shared.EntityState.IDLE;
                        if (!Animation_state.ContainsKey(temp_state))
                        {
                            // Idle isn't set? This entity is bugged. Avoid it.
                            // --------------------------
                            return;
                        }
                    }
                    int max_index;
                    // --------------------------
                    // Animation Frame Calculation
                    max_index = Animation_state[temp_state].Frames.Count();
                    anim_delay++;
                    if (anim_delay > Shared.FPS)
                    {
                        anim_delay = 0;
                        anim_index++;
                        if (anim_index >= max_index)
                        {
                            final_frame = true;
                            if (Animation_state[temp_state].Loop)
                                anim_index = 0;
                            else
                                anim_index = Animation_state[temp_state].Frames.Count() - 1;
                        }
                        else
                            final_frame = false;
                    }

                    // --------------------------
                    // Complete Attacks
                    if (Active_Attack != "none" && final_frame)
                        Try_Attack_Clear();
                }
            }
        }
        /// <summary>
        /// Base entity draw method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                List<Rectangle> animation;
                Rectangle source;
                Vector2 origin;
                Texture2D image;
                Vector2 draw_position = Position;
                SpriteEffects flipDirection;

                if (animated)
                {
                    Shared.EntityState temp_state = State;

                    // Animation doesn't exist checks
                    // --------------------
                    if (!Animation_state.ContainsKey(State))
                    {
                        // Set to IDLE by default
                        // --------------------
                        temp_state = Shared.EntityState.IDLE;

                        if (State == Shared.EntityState.FALLING)
                        {
                            if (Animation_state.ContainsKey(Shared.EntityState.JUMPING))
                            {
                                // If this entity doesn't have a falling animation, resort to Jumping
                                // --------------------
                                temp_state = Shared.EntityState.JUMPING; ;
                            }
                        }
                        else if (State == Shared.EntityState.RUNNING)
                        {
                            if (Animation_state.ContainsKey(Shared.EntityState.WALKING))
                            {
                                // If this entity doesn't have a falling animation, resort to Jumping
                                // --------------------
                                temp_state = Shared.EntityState.WALKING; ;
                            }
                        }
                        else if (!Animation_state.ContainsKey(Shared.EntityState.IDLE))
                        {
                            // Idle isn't set? This entity is bugged. Avoid it.
                            // --------------------------

                            return;
                        }
                    }
                    // --------------------
                    animation = Animation_state[temp_state].Frames;
                    // --------------------

                    // --------------------
                    // Safety Dance
                    if (anim_index >= animation.Count())
                    {
                        anim_index = 0;
                    }
                    // --------------------
                    // Full Play
                    if (State_Cache != State)
                    {
                        anim_index = 0;
                    }
                    // --------------------
                    source = animation[anim_index];
                    image = SpriteSheet;
                    origin = new Vector2(FrameSize.X / 2, FrameSize.Y / 2);
                }
                else
                {
                    source = new Rectangle(0, 0, Still_image.Width, Still_image.Height);
                    image = Still_image;
                    origin = new Vector2(Still_image.Width / 2, Still_image.Height / 2);
                }
                // --------------------
                // Direction
                // --------------------
                if (Active_Attack == "none")
                {
                    if (direction == Shared.Direction.LEFT)
                        flipDirection = SpriteEffects.FlipHorizontally;
                    else
                        flipDirection = SpriteEffects.None;
                }
                else
                {
                    if (ATK_Direction == Shared.Direction.LEFT)
                        flipDirection = SpriteEffects.FlipHorizontally;
                    else if (ATK_Direction == Shared.Direction.RIGHT)
                        flipDirection = SpriteEffects.None;
                    else
                    {
                        if (direction == Shared.Direction.LEFT)
                            flipDirection = SpriteEffects.FlipHorizontally;
                        else
                            flipDirection = SpriteEffects.None;
                    }
                }

                if (Invulnerable)
                    Overlay_Color = new Color(255,150,150);

                // --------------------
                // Draw
                // --------------------
                spriteBatch.Begin
                    (
                    SpriteSortMode.FrontToBack,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone,
                    null,
                    Shared.Screen.Get_Camera()
                    );
                spriteBatch.Draw
                    (
                    image,
                    Position,
                    source,
                    Overlay_Color,
                    rotation,
                    origin,
                    Shared.Pixel_Scale,
                    flipDirection,
                    0
                    );
                spriteBatch.End();
                // ----------------------------------
                // Refresh Basics
                State_Cache = State;
                Overlay_Color = Color.White;
                // ----------------------------------
                // Debug Bound Box Borders
                if (this is Character && Shared.Debug)
                    Draw_Box(Get_Bounds(), 1, Color.Red);
                // ----------------------------------
            }
        }
        // ----------------------------------
        /// <summary>
        /// Get's the rectangle bounds of an entity
        /// </summary>
        /// <returns>The bounds of our entity</returns>
        public override Rectangle Get_Bounds()
        {
            // Bound Box Adjustments
            // -------------------------------
            if (State_Boundary_Offset.ContainsKey(State))
            {
                Boundary_Offset = State_Boundary_Offset[State];
            }
            else if (State_Boundary_Offset.ContainsKey(Shared.EntityState.IDLE))
            {
                Boundary_Offset = State_Boundary_Offset[Shared.EntityState.IDLE];
            }
            else
            {
                Boundary_Offset = Rectangle.Empty;
            }
            // -------------------------------

            // ----------------------------------
            Vector2 origin_coord;
            Vector2 bound_size;
            // ----------------------------------

            // Animated vs Static
            // ----------------------------------
            if (animated)
            {
                bound_size = new Vector2
                (
                    (int)(FrameSize.X - Boundary_Offset.Width),
                    (int)(FrameSize.Y - Boundary_Offset.Height)
                );
            }
            else
            {
                bound_size = new Vector2
                (
                    (int)(Still_image.Width - Boundary_Offset.Width),
                    (int)(Still_image.Height - Boundary_Offset.Height)
                );
            }
            // ----------------------------------

            // Direction check ensures the boundary box maintains relation to the sprite
            // ----------------------------------
            origin_coord = new Vector2
                (
                    (int)Position.X + Boundary_Offset.X,
                    (int)Position.Y + Boundary_Offset.Y + Boundary_Offset.Height
                );

            return new Rectangle
                (
                (int)(origin_coord.X - (bound_size.X * Shared.Pixel_Scale) / 2),
                (int)(origin_coord.Y - (bound_size.Y * Shared.Pixel_Scale) / 2),
                (int)(bound_size.X * Shared.Pixel_Scale),
                (int)(bound_size.Y * Shared.Pixel_Scale)
                );
        }
        // ----------------------------------
        #endregion
        // ----------------------------------
    }
}
