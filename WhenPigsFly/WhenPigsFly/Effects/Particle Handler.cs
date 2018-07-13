// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Effect & Projectile Class
// Created 12/08/2015
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
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Instance Data & Methods
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// The effect class is used for all sorts of things within the game,
    /// from animations to particle effects to emotes to attacks.
    /// </summary>
    public class Effect
    {
        // ------------------
        // Data
        // ------------------
        protected string name = "Unnamed";

        protected SpriteBatch spriteBatch = Shared.Batch;
        protected bool animated = false;
        protected bool complete = false;
        protected bool follow_source = false;
        public Character Source = null;

        protected Vector2 position = Vector2.Zero;
        protected Vector2 Source_Offset = Vector2.Zero;

        protected float lifespan = 100f;
        protected bool do_lifespan = false;
        protected float rotation = 0f;

        // ------------------
        // Animation
        // ------------------
        protected int anim_index = 0;
        protected int anim_delay = 0;

        protected Texture2D image;
        protected Shared.Direction direction = Shared.Direction.LEFT;
        protected bool flippable = true;
        protected Animation animation;
        protected Vector2 dimensions = Vector2.Zero;
        protected int anim_length = -1;
        protected int spawn_frame = 0;
        protected bool spawned = false;

        // ------------------
        // Special Attributes
        // ------------------
        public bool do_gravity = false;
        public bool do_collision = false;
        public bool do_rotation = false;

        public Vector2 Velocity;
        public float Resistance;
        private float fall_time = 0f;

        public Color overlay_color = Color.White;
        // ------------------

        #region Getters & Setters
        public string Name
        {
            get { return name; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public bool Complete
        {
            get { return complete; }
        }
        public Vector2 Dimensions
        {
            get { return dimensions; }
        }
        public Shared.Direction Direction
        {
            get { return direction; }
        }
        #endregion

        public Effect(Texture2D image, Vector2 dimensions, Animation animation, int anim_length, bool flippable = true, string name = "Unnamed")
        {
            // ------------------
            this.image = image;
            this.flippable = flippable;
            if (animation != null)
            {
                this.animated = true;
                this.animation = animation;
                this.anim_length = anim_length;  
            }
            this.dimensions = dimensions;
            this.name = name;
            // ------------------
        }

        /// <summary>
        /// After a new effect is created based on an available effect mold, it's finally given life
        /// </summary>
        /// <param name="lifespan">The duration this effect should live for</param>
        /// <param name="offset">The offset of this effects spawn location. If it is not given a source, this will offset from 0,0</param>
        /// <param name="follow_source">Whether or not this effect should move with its source emitter</param>
        /// <param name="source">The source emitter of the effect</param>
        public void Create_Effect(
            float lifespan, 
            Vector2 offset, 
            bool follow_source,
            Character source,
            Vector2 velocity)
        {
            this.lifespan = lifespan;
            this.Velocity = velocity;

            if (source == null)
            {
                this.direction = Shared.Direction.RIGHT;
                follow_source = false;

                this.position = offset;
                this.Source_Offset = Vector2.Zero;
            }
            else
            {
                this.follow_source = follow_source;
                this.Source = source;
                this.direction = source.Direction;

                this.position = source.Position;
                this.Source_Offset = offset;
            }

            if (!follow_source)
                position += Source_Offset;
        }
        // ----------------------------------
        /// <summary>
        /// Runs, updates, and draws this effect
        /// </summary>
        /// <param name="gameTime">A snapshot of game time</param>
        public virtual void Play(GameTime gameTime)
        {
            // -------------------------------
            // Lifespan Counter
            // -------------------------------
            if (lifespan > 0)
            {
                float elapsed_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                lifespan -= elapsed_time;
            }
            if (lifespan <= 0)
                complete = true;
            // -------------------------------
            if (animated)
            {
                // -------------------------
                // Play Animation
                anim_delay++;
                if (anim_delay > Shared.FPS)
                {
                    anim_delay = 0;
                    anim_index++;
                    if (anim_index >= anim_length)
                    {
                        if (animation.Loop)
                            anim_index = 0;
                        else
                        {
                            anim_index = anim_length - 1;
                            lifespan = 0f;
                        }
                    }
                }
                // -------------------------
            }
            // -------------------------------

            // -------------------------
            Rectangle frame;
            float scale = Shared.Pixel_Scale;
            Vector2 origin = new Vector2(dimensions.X / 2, dimensions.Y / 2); ;
            // -------------------------

            if (animated)
                frame = animation.Frames[anim_index];
            else
                frame = new Rectangle(0, 0, image.Width, image.Height);

            // --------------------
            // Positioning & Source Following
            // --------------------

            // --------------------
            // Offset Reflection
            Vector2 Offset = Source_Offset;

            if (direction == Shared.Direction.LEFT)
                Offset.X = -Offset.X;

            // --------------------
            // Source Following
            if (follow_source)
                position = Source.Position + Offset;

            // --------------------
            // Flipping
            SpriteEffects Flip = SpriteEffects.None;
            if (flippable)
                if (direction == Shared.Direction.LEFT)
                    Flip = SpriteEffects.FlipHorizontally;


            // --------------------
            // Velocity Rotation
            if (do_rotation)
            {
                float x_slope = (Velocity.X * 2) - Velocity.X;
                float y_slope = (Velocity.Y * 2) - Velocity.Y;
                rotation = (float)Math.Atan(y_slope / x_slope);

                Flip = SpriteEffects.None;
                if (x_slope < 0)
                    Flip = SpriteEffects.FlipHorizontally;
            }
            // --------------------

            // --------------------
            // Special cases
            // --------------------
            if (name == "Blood")
                scale = Shared.Randomizer.Next(2, 5);

                // Health Bars
                // --------------------
                if (name == "HPcontainer")
                {
                    if(Source is NPC)
                    {
                        NPC npc = (NPC)Source;
                        npc.health_container = this;

                        if (npc.Is_dead())
                            lifespan = 0f;
                    }
                }
                if (name == "HP")
                {
                    if (Source is NPC)
                    {
                        NPC npc = (NPC)Source;
                        npc.health_bar = this;

                        // Handle color 
                        // --------------------------
                        float percentage = ((float)npc.Health_Points / npc.MAX_Health_Points);
                        if (percentage > 0.5f)
                            overlay_color = new Color(165,235,126);
                        else if (percentage > 0.25f)
                            overlay_color = new Color(235, 220, 90);
                        else if (percentage <= 0.25f)
                            overlay_color = new Color(210, 64, 49);
                        // --------------------------
                        // Handle size
                        frame.Width = (int)(frame.Width * percentage);

                        if (npc.Is_dead())
                            lifespan = 0f;
                    }
                }
                // --------------------

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
                position,
                frame,
                overlay_color,
                rotation,
                origin,
                scale,
                Flip,
                0
                );
            spriteBatch.End();
            // --------------------

            // Ensure this Effect is spawned at the proper time
            // --------------------
            if (Source != null)
            {
                if (Source.Anim_index == spawn_frame || spawn_frame == 0)
                    spawned = true;
            }
            else
            {
                spawned = true;
            }
            // --------------------

            if (spawned)
            {
                if (do_gravity)
                {
                    // Simple gravity for effects, particles & projectiles
                    // -------------------------------------
                    fall_time += (float)gameTime.ElapsedGameTime.Milliseconds / 2000;
                    Velocity.Y -= (Shared.Gravity.Y * fall_time) * 0.1f;

                    Velocity.X = Velocity.X * 0.98f;

                    if (Velocity.Y > -Shared.Gravity.Y)
                        Velocity.Y = -Shared.Gravity.Y;
                    // -------------------------------------
                }

                if (do_collision)
                {
                    // Simple collision for effects, particles & projectiles
                    // ------------------------------------- 
                    int X = (int)Math.Round((Position.X / Shared.Pixel_Scale) / Shared.Block_Dimension);
                    int Y = (int)Math.Round((Position.Y / Shared.Pixel_Scale) / Shared.Block_Dimension);
                    // ------------------------------------- 
                    // Stop directional velocity
                    Terrain right = Shared.Active_World.Get_Terrain_At(X + 1, Y);
                    Terrain left = Shared.Active_World.Get_Terrain_At(X - 1, Y);
                    Terrain up = Shared.Active_World.Get_Terrain_At(X, Y - 1);
                    Terrain down = Shared.Active_World.Get_Terrain_At(X, Y + 1);

                    // Right Side
                    if (right is Terrain)
                        if (Velocity.X > 0f)
                            if (Get_Bounds().Intersects(right.Get_Bounds()))
                                Velocity.X = 0f;
                    // Left Side
                    if (left is Terrain)
                        if (Velocity.X < 0f)
                            if (Get_Bounds().Intersects(left.Get_Bounds()))
                                Velocity.X = 0f;
                    // Ceiling
                    if (up is Terrain)
                        if (Velocity.Y < 0f)
                            if (Get_Bounds().Intersects(up.Get_Bounds()))
                                Velocity.Y = 0f;
                    // Floor
                    if (down is Terrain)
                        if (Velocity.Y >= 0f)
                            if (Get_Bounds().Intersects(down.Get_Bounds()))
                                Velocity.Y = 0f;
                    // ------------------------------------- 
                }

                // Apply Velocity
                // --------------------
                if (follow_source)
                {
                    // Velocity reflection for flipping left & right
                    if (direction == Shared.Direction.LEFT)
                        Source_Offset.X -= Velocity.X;
                    else
                        Source_Offset.X += Velocity.X;
                    // --------------------

                    Source_Offset.Y += Velocity.Y;
                    // --------------------
                }
                else
                {
                    // --------------------
                    position += Velocity;
                    // --------------------
                }
               
            }
        }
        // ----------------------------------
        /// <summary>
        /// Get's the rectangle bounds of an entity
        /// </summary>
        /// <returns>The bounds of our entity</returns>
        public Rectangle Get_Bounds()
        {
            return new Rectangle
                (
                (int)Position.X - (int)((Dimensions.X / 2) * Shared.Pixel_Scale),
                (int)Position.Y - (int)((Dimensions.Y / 2) * Shared.Pixel_Scale),
                (int)(Dimensions.X * Shared.Pixel_Scale),
                (int)(Dimensions.Y * Shared.Pixel_Scale)
                );
        }
        // ----------------------------------
    }


    /// <summary>
    /// The effect class is used for all sorts of things within the game,
    /// from animations to particle effects to emotes to attacks.
    /// </summary>
    public class Projectile : Effect
    {
        // ------------------
        // Projectile Data
        // ------------------
        public Shared.Damage_Team Damage_team = Shared.Damage_Team.NEUTRAL;
        public Shared.Type Damage_type = Shared.Type.MUNDANE;
        public int Damage = 0;
        public float Knockback = 0f;

        public Projectile(Texture2D image, Vector2 dimensions, Animation animation, int anim_length, bool flippable = true, string name = "Unnamed") 
            : base(image, dimensions, animation, anim_length, flippable, name)
        {
            // ------------------
            this.image = image;
            if (animation != null)
            {
                this.animated = true;
                this.animation = animation;
                this.anim_length = anim_length;
            }
            this.dimensions = dimensions;
            this.name = name;
            // ------------------
        }

        /// <summary>
        /// After a new projectile is created based on an available effect mold, it's finally given life
        /// </summary>
        /// <param name="lifespan">The duration this effect should live for</param>
        /// <param name="location">The raw X & Y coordinates of the effect</param>
        /// <param name="follow_source">Whether or not this effect should move with its source emitter</param>
        /// <param name="source">The source emitter of the effect</param>
        /// <param name="velocity">The velocity of the projectile</param>
        /// <param name="damage">Projectile damage</param>
        /// <param name="knockback">Projectile Knockback</param>
        /// <param name="damage_team">The damage team of the characters this projectile can harm</param>
        /// <param name="damage_type">The type of damage this projectile inflicts</param>
        /// <param name="spawn_frame">The frame in which this projecile spawns in its source's animation</param>
        public void Create_Projectile(
            float lifespan,
            Vector2 offset,
            bool follow_source,
            Character source,
            Vector2 velocity,
            int damage,
            float knockback,
            Shared.Damage_Team damage_team,
            Shared.Type damage_type,
            int spawn_frame)
        {
            this.lifespan = lifespan;
            this.Velocity = velocity;
            this.Damage = damage;
            this.Knockback = knockback;
            this.Damage_team = damage_team;
            this.Damage_type = damage_type;
            this.spawn_frame = spawn_frame;
            this.position = source.Position;
            this.Source_Offset = offset;

            if (source == null)
            {
                this.direction = Shared.Direction.RIGHT;
                follow_source = false;

                this.position = offset;
                this.Source_Offset = Vector2.Zero;
            }
            else
            {
                this.follow_source = follow_source;
                this.Source = source;
                this.direction = source.Direction;

                this.position = source.Position;
                this.Source_Offset = offset;
            }

            if (!follow_source)
                position += Source_Offset;
        }
    }
    // ------------------
    #region Effect_Mold Class
    /// <summary>
    /// The Effect Mold class is used to easily replicate effect presets when performing an effect-
    /// It also prevents having to do calculations for animation & everything also for every effect. Tada.
    /// </summary>
    public class Effect_Mold
    {
        // ------------------
        // Data
        // ------------------
        public string Name = "Unnamed";
        public bool Animated = false;
        public bool Flippable = true;

        // ------------------
        // Animation
        // ------------------
        public Texture2D Image;
        public Animation Animation;
        public Vector2 Dimensions;
        public int Anim_length = -1;

        // ------------------
        // Special Attributes
        // ------------------
        public bool Do_gravity = false;
        public bool Do_collision = false;
        public bool Do_rotation = false;

        public float Resistance = 1f;
        public Color Overlay_color = Color.White;

        public Effect_Mold(
            Texture2D image, 
            Vector2 dimensions, 
            bool animated = false, 
            int[] frame_delay = null,
            bool loop = false,
            bool flippable = true,
            string name = "Unnamed")
        {
            // ------------------
            this.Image = image;
            this.Animated = animated;
            this.Flippable = flippable;
            this.Dimensions = dimensions;
            this.Name = name;

            if (animated)
            {
                Animation = AnimationHandler.Generate
                    (
                    image, 
                    dimensions,
                    1,
                    uniqueDelay: frame_delay, 
                    loop: loop);
                Anim_length = Animation.Frames.Count();
            }
            // ------------------
        }
    }
    #endregion
    // ------------------

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Static Data & Methods
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// The effect handler renders && updates all active effects, 
    /// and keeps the list of effects crisp and optimized
    /// </summary>
    public static class Particles
    {
        // ---------------------
        // Data
        // ---------------------
        public static Dictionary<string, Effect_Mold> Effects = new Dictionary<string, Effect_Mold>();
        public static List<Effect> Active_Effects = new List<Effect>();
        public static List<Projectile> Active_Projectiles = new List<Projectile>();
        // ---------------------

        /// <summary>
        /// Plays all active effects!
        /// </summary>
        /// <param name="gameTime">Snapshot of game time</param>
        public static void Draw_Effects(GameTime gameTime)
        {
            // Since we can't remove within
            List<Effect> Dead_Effects = new List<Effect>();

            if (Active_Effects.Count() != 0)
            {
                foreach (Effect effect in Active_Effects)
                {
                    effect.Play(gameTime);

                    if (effect.Complete)
                    {
                        Dead_Effects.Add(effect);
                        continue;
                    }
                }

                foreach (Effect dead_effect in Dead_Effects)
                    Active_Effects.Remove(dead_effect);

                Dead_Effects.Clear();
            }
        }
        // ---------------------
        /// <summary>
        /// Players all active projectiles!
        /// </summary>
        /// <param name="gameTime">Snapshot of game time</param>
        public static void Draw_Projectiles(GameTime gameTime)
        {
            // Since we can't remove within
            List<Effect> Dead_Projectiles = new List<Effect>();

            if (Active_Projectiles.Count() != 0)
            {
                foreach (Projectile projectile in Active_Projectiles)
                {
                    projectile.Play(gameTime);

                    if (projectile.Complete)
                    {
                        Dead_Projectiles.Add(projectile);
                        continue;
                    }
                }

                foreach (Projectile dead_projectile in Dead_Projectiles)
                    Active_Projectiles.Remove(dead_projectile);

                Dead_Projectiles.Clear();
            }
        }
        // -------------------------------

        // -------------------------------
        #region Effect & Projectile Generation
        /// <summary>
        /// Builds the list of all available effects
        /// </summary>
        public static void Generate_Effects()
        {
            // -----------------
            int[] frame_delay; // Used to apply delays to frames.

            // -----------------
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // Animations
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            // Pixel Particle
            // -----------------
            #region Blood Particles

            Effect_Mold particle = new Effect_Mold
                (
                Shared.IMG_Animations["particle"],
                new Vector2(2, 2),
                name: "Blood"
                );

            particle.Overlay_color = Color.Red;
            particle.Do_gravity = true;
            particle.Do_collision = true;
            particle.Do_rotation = true;

            Particles.Effects.Add("Blood", particle);

            #endregion
            // -----------------

            // Death Animation
            // -----------------
            #region Arcane Overflow

            Effect_Mold death = new Effect_Mold
                (
                Shared.IMG_Animations["death"],
                new Vector2(32, 32),
                true,
                flippable: false,
                name: "Death"
                );

            Particles.Effects.Add("Death", death);

            #endregion
            // -----------------

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // Emotes
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            // Alert Emote
            // -----------------
            #region Alert Emote
            frame_delay = new int[3]
            {
                0,
                0,
                5
            };
            // -----------------
            Effect_Mold Alert = new Effect_Mold
                (
                Shared.IMG_Emotes["Alert"],
                new Vector2(16, 16),
                true,
                frame_delay,
                false,
                false,
                "Alert_Emote"
                );
            Particles.Effects.Add("Alert", Alert);
            #endregion
            // -----------------

            // Confused Emote
            // -----------------
            #region Confused Emote
            frame_delay = new int[5]
            {
                1,
                1,
                3,
                0,
                4
            };
            // -----------------
            Effect_Mold Confused = new Effect_Mold
                (
                Shared.IMG_Emotes["Confused"],
                new Vector2(16, 16),
                true,
                frame_delay,
                false,
                false,
                "Confused_Emote"
                );
            Particles.Effects.Add("Confused", Confused);
            #endregion
            // -----------------

            // Health Bar Container
            // -----------------
            #region Health Bar Container
            // -----------------
            Effect_Mold HPcontainer = new Effect_Mold
                (
                Shared.IMG_Emotes["healthbarcontainer"],
                new Vector2(20, 3),
                false,
                null,
                false,
                false,
                "HPcontainer"
                );
            Particles.Effects.Add("HPcontainer", HPcontainer);
            #endregion
            // -----------------

            // Health Bar
            // -----------------
            #region Health Bar
            // -----------------
            Effect_Mold HP = new Effect_Mold
                (
                Shared.IMG_Emotes["healthbar"],
                new Vector2(20, 3),
                false,
                null,
                false,
                false,
                "HP"
                );
            Particles.Effects.Add("HP", HP);
            #endregion
            // -----------------

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // Attacks & Projectiles
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            // Plain Attack
            // -----------------
            #region Plain Attack
            frame_delay = new int[1];
            // -----------------
            Effect_Mold Plain_Attack = new Effect_Mold
                (
                Shared.IMG_Projectiles["plain_attack"],
                new Vector2(16, 16),
                true,
                frame_delay,
                false,
                true,
                "Plain_Attack"
                );
            Particles.Effects.Add("Plain_Attack", Plain_Attack);
            #endregion
            // -----------------

            // Dagger
            // -----------------
            #region Dagger
            // -----------------
            Effect_Mold Dagger = new Effect_Mold
                (
                Shared.IMG_Projectiles["dagger"],
                new Vector2(8, 8),
                false,
                null,
                false,
                false,
                "Dagger"
                );

            Dagger.Do_collision = true;
            Dagger.Do_gravity = true;
            Dagger.Do_rotation = true;

            Particles.Effects.Add("Dagger", Dagger);
            #endregion
            // -----------------

            // Arcane Bolt
            // -----------------
            #region Arcane_Bolt
            // -----------------
            Effect_Mold Arcane_Bolt = new Effect_Mold
                (
                Shared.IMG_Projectiles["arcane_bolt"],
                new Vector2(12, 7),
                true,
                null,
                true,
                false,
                "Arcane_Bolt"
                );

            Arcane_Bolt.Do_collision = true;
            Arcane_Bolt.Do_rotation = true;

            Particles.Effects.Add("Arcane_Bolt", Arcane_Bolt);
            #endregion
            // -----------------
        }
        #endregion
        // -------------------------------

        // -------------------------------
        #region Effect & Projectile Spawning
        /// <summary>
        /// Emits a new effect from the available effect molds
        /// </summary>
        /// <param name="lifespan">The duration this effect should live for</param>
        /// <param name="location">The raw X & Y coordinates of the effect</param>
        /// <param name="follow_source">Whether or not this effect should move with its source emitter</param>
        /// <param name="source">The source emitter of the effect</param>
        public static void Do_Effect(
            string name,
            float lifespan,
            Vector2 spawn,
            Vector2 velocity,
            bool follow_source = false,
            Character source = null)
        {
            if (Effects.ContainsKey(name))
            {
                Effect effect = new Effect
                    (
                    Effects[name].Image,
                    Effects[name].Dimensions,
                    Effects[name].Animation,
                    Effects[name].Anim_length,
                    Effects[name].Flippable,
                    Effects[name].Name
                    );

                // Special Attributes
                // -------------------------
                effect.overlay_color = Effects[name].Overlay_color;
                effect.do_collision = Effects[name].Do_collision;
                effect.do_gravity = Effects[name].Do_gravity;
                effect.do_rotation = Effects[name].Do_rotation;
                effect.Resistance = Effects[name].Resistance;
                // -------------------------

                effect.Create_Effect(lifespan, spawn, follow_source, source, velocity);
                Active_Effects.Add(effect);
            }
            else
            {
                Console.WriteLine("Effect named '" + name + "' does not exist!");
            }
        }

        // -------------------------------
        /// <summary>
        /// Emites a new projectile from the available effect molds
        /// </summary>
        /// <param name="name">The name of the projectile</param>
        /// <param name="lifespan">The duration this effect should live for</param>
        /// <param name="source_offset">The X & Y offsets this projectile should spawn from</param>
        /// <param name="velocity">Velocity of the projectile</param>
        /// <param name="damage_team">Damage team this projectile should hurt</param>
        /// <param name="damage_type">The type of damage to inflict</param>
        /// <param name="spawn_frame">The frame in which this projectile should be fired in animation. Defaults to first frame.</param>
        /// <param name="follow_source">Whether or not this effect should move with its source emitter Defaults to false.</param>
        /// <param name="source">The source emitter of the effect. Defaults to null.</param>
        public static void Do_Projectile(
            string name, 
            float lifespan,
            Vector2 source_offset,
            Vector2 velocity,
            int damage,
            float knockback,
            Shared.Damage_Team damage_team = Shared.Damage_Team.NEUTRAL,
            Shared.Type damage_type = Shared.Type.MUNDANE,
            int spawn_frame = 0,
            bool follow_source = false,
            Character source = null)
        {
            if (Effects.ContainsKey(name))
            {
                Projectile projectile = new Projectile
                    (
                    Effects[name].Image,
                    Effects[name].Dimensions,
                    Effects[name].Animation,
                    Effects[name].Anim_length,
                    Effects[name].Flippable,
                    Effects[name].Name
                    );

                // Special Attributes
                // -------------------------
                projectile.overlay_color = Effects[name].Overlay_color;
                projectile.do_collision = Effects[name].Do_collision;
                projectile.do_gravity = Effects[name].Do_gravity;
                projectile.do_rotation = Effects[name].Do_rotation;
                projectile.Resistance = Effects[name].Resistance;
                // -------------------------

                projectile.Create_Projectile(lifespan, source_offset, follow_source, source, velocity, damage, knockback, damage_team, damage_type, spawn_frame);
                Active_Projectiles.Add(projectile);
            }
            else
            {
                Console.WriteLine("Projectile named '" + name + "' does not exist!");
            }
        }
        // -------------------------------
        #endregion
        // -------------------------------

        // -------------------------------
        /// <summary>
        /// Creates a cute simple automated blood splurt at the position
        /// </summary>
        /// <param name="position">Position to splurt</param>
        /// <param name="quantity">Quantity of blood particles</param>
        public static void Do_Bloodspurt(Character source, int quantity, int intensity = 5, bool fountain = false)
        {
            for (int count = 0; count < quantity; count++)
            {
                if (fountain)
                {
                    Particles.Do_Effect(
                        "Blood",
                        1f,
                        Vector2.Zero,
                        new Vector2
                            (
                            Shared.Randomizer.Next(-intensity, intensity),
                            Shared.Randomizer.Next(-10, 0)
                            ),
                        source: source
                        );
                }
                else
                {
                    Particles.Do_Effect(
                        "Blood",
                        1f,
                        Vector2.Zero,
                        new Vector2
                            (
                            Shared.Randomizer.Next(-intensity, intensity),
                            Shared.Randomizer.Next(-intensity, intensity)
                            ),
                        source: source
                        );
                }
            }
        }
        // -------------------------------
    }
}
