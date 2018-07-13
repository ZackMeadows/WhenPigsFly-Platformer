// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Collision Management Class
// Created 11/28/2015
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
    /// The Collision Manager controls all entity collision
    /// </summary>
    public static class CollisionManager
    {
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Generic Character Entity vs Terrain Entity Collision
        /// <summary>
        /// Ground Collision Checker
        /// </summary>
        public static bool Ground_Collision(Character character_entity)
        {
            //-------------------------------------
            // Check block below for floor collision
            //-------------------------------------

            //-------------------------------------
            // Block Below Adjacent
            Terrain block = Shared.Active_World.Get_Terrain_At(
                        (int)character_entity.Grid_Position.X,
                        (int)character_entity.Grid_Position.Y + 1);
            if (block == null)
                return false;
            //-------------------------------------

            // Retrieve the character entities floor collision bound box
            //-------------------------------------
            Rectangle bound_box = new Rectangle
            (
            character_entity.Get_Bounds().X,
            character_entity.Get_Bounds().Y,
            character_entity.Get_Bounds().Width,
            character_entity.Get_Bounds().Height
            );
            //-------------------------------------

            if (bound_box.Intersects(block.Get_Bounds()))
            {
                // We've found a collision. Let's check if it's a ground collision!
                //-------------------------------------
                // Bound Boxes
                //--------------------
                Rectangle c_bound = bound_box;
                Rectangle t_bound = block.Get_Bounds();
                //--------------------

                // Make sure this block is actually below us

                //--------------------
                if (c_bound.Bottom > t_bound.Top)
                {
                    if (c_bound.Top < t_bound.Top)
                    {
                        // Ensure the player is only shifted to the ground if absolutely necessary
                        //--------------------
                        if (c_bound.Top < t_bound.Top && c_bound.Bottom > t_bound.Top && c_bound.Bottom < t_bound.Bottom)
                        {
                            character_entity.Shift(new Vector2(0, (c_bound.Bottom - t_bound.Top) - 3));
                        }
                        return true;
                    }
                }
            }
            return false;
            //-------------------------------------
        }
        /// <summary>
        /// Ceiling Collision Checker
        /// </summary>
        public static bool Ceiling_Collision(Character character_entity)
        {
            //-------------------------------------
            // Check block above character for collision
            //-------------------------------------
            List<Terrain> quick_terrain_list = new List<Terrain>();

            //-------------------------------------
            // Block Atop
            Terrain block1 = Shared.Active_World.Get_Terrain_At(
                        (int)character_entity.Grid_Position.X,
                        (int)character_entity.Grid_Position.Y);
            if (block1 != null)
                quick_terrain_list.Add(block1);
            //-------------------------------------
            // Block Above
            Terrain block2 = Shared.Active_World.Get_Terrain_At(
                                    (int)character_entity.Grid_Position.X,
                                    (int)character_entity.Grid_Position.Y - 1);
            if (block2 != null)
                quick_terrain_list.Add(block2);
            //-------------------------------------

            if (quick_terrain_list.Count() == 0)
                return false; // There are no blocks worth checking on this side, leave collision check

            // Retrieve the character entities wall collision bound box
            //-------------------------------------
            Rectangle bound_box = new Rectangle
            (
            character_entity.Get_Bounds().X + 5,
            character_entity.Get_Bounds().Y,
            character_entity.Get_Bounds().Width - 10,
            character_entity.Get_Bounds().Height
            );
            //-------------------------------------

            foreach (Terrain block in quick_terrain_list)
            {
                if (bound_box.Intersects(block.Get_Bounds()))
                {
                    // We've found a collision. Let's check if it's a ground collision!
                    //-------------------------------------
                    // Bound Boxes
                    //--------------------
                    Rectangle c_bound = bound_box;
                    Rectangle t_bound = block.Get_Bounds();
                    //--------------------
                    if (c_bound.Top < t_bound.Bottom)
                    {
                        if (c_bound.Bottom > t_bound.Bottom)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
            //-------------------------------------        
        }
        /// <summary>
        /// Wall Collision Checker
        /// </summary>
        public static bool Wall_Collision(Character character_entity, Shared.Direction direction)
        {

            //-------------------------------------
            // Produce a quick list of terrain entities within a direct radius of the character
            //-------------------------------------
            List<Terrain> quick_terrain_list = new List<Terrain>();

            // Get the side we're checking
            int x_side = 1; // 1 = Right side, -1 = Left side
            if (direction == Shared.Direction.LEFT)
                x_side = -1;

            //-------------------------------------
            // Block Adjacent
            Terrain block1 = Shared.Active_World.Get_Terrain_At(
                        (int)character_entity.Grid_Position.X + x_side,
                        (int)character_entity.Grid_Position.Y);
            if (block1 != null)
                quick_terrain_list.Add(block1);
            //-------------------------------------
            // Block Below Adjacent
            Terrain block2 = Shared.Active_World.Get_Terrain_At(
                        (int)character_entity.Grid_Position.X + x_side,
                        (int)character_entity.Grid_Position.Y + 1);
            if (block2 != null)
                quick_terrain_list.Add(block2);
            //-------------------------------------
            // Block Above Adjacent
            Terrain block3 = Shared.Active_World.Get_Terrain_At(
                        (int)character_entity.Grid_Position.X + x_side,
                        (int)character_entity.Grid_Position.Y - 1);
            if (block3 != null)
                quick_terrain_list.Add(block3);
            //-------------------------------------

            if (quick_terrain_list.Count() == 0)
                return false; // There are no blocks worth checking on this side, leave collision check
            //-------------------------------------

            // Retrieve the character entities wall collision bound box
            //-------------------------------------
            Rectangle bound_box = new Rectangle
                    (
                    character_entity.Get_Bounds().X - 3,
                    character_entity.Get_Bounds().Y,
                    character_entity.Get_Bounds().Width + 6,
                    character_entity.Get_Bounds().Height
                    );
            //-------------------------------------

            foreach (Terrain terrain_entity in quick_terrain_list)
            {
                  if (bound_box.Intersects(terrain_entity.Get_Bounds()))
                    {
                        Rectangle c_bound = bound_box;
                        Rectangle t_bound = terrain_entity.Get_Bounds();
                        //--------------------
                        // We should first make sure this intersected peice of terrain isn't cleanly above or below
                        // the character entity
                        //--------------------
                        if (c_bound.Top > t_bound.Bottom || c_bound.Bottom < t_bound.Top)
                        {
                            return false;
                        }

                        //  Safety check informs us if this is a floor rather than a wall.
                        // -------------------
                        if ((c_bound.Bottom - t_bound.Top) < 5)
                        {
                            return false;
                        }
                        //--------------------
                        if (direction == Shared.Direction.LEFT)
                        {
                            // Left Wall Check
                            // -------------------
                            if (c_bound.Left < t_bound.Right && c_bound.Right > t_bound.Right)
                            {
                                // This bit helps ensure players don't exploit spam 
                                // jumping against hole walls to breach the wall.
                                if (character_entity.Get_Bounds().Left < t_bound.Right)
                                {
                                    character_entity.Shift(new Vector2(-1, 0));
                                }
                                // -------------------
                                return true;
                            }
                            // -------------------
                        }
                        if (direction == Shared.Direction.RIGHT)
                        {
                            // Right Wall Check
                            // -------------------
                            if (c_bound.Right > t_bound.Left && c_bound.Left < t_bound.Left)
                            {
                                // This bit helps ensure players don't exploit spam 
                                // jumping against hole walls to breach the wall.
                                if (character_entity.Get_Bounds().Right > t_bound.Left)
                                {
                                    character_entity.Shift(new Vector2(1, 0));
                                }
                                // -------------------
                                return true;
                            }
                            // -------------------
                        }
                    }
                }  
            //-------------------------------------
            return false;
        } 
        #endregion
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Combat Collision Management

        // ------------------------------------
        /// <summary>
        /// Checks all world characters for any combat collisions
        /// </summary>
        public static void Character_Collision()
        {
            if (Shared.Active_World.WorldCharacters.Count() != 0)
            {
                foreach (Character attacker in Shared.Active_World.WorldCharacters)
                {
                    if (!attacker.Enabled || attacker.Is_dead())
                        continue;

                    if (attacker.Team == Shared.Damage_Team.HOSTILE)
                    {
                        foreach (Character target in Shared.Active_World.WorldCharacters)
                        {
                            if (!target.Enabled || target.Is_dead() || target.Invulnerable)
                                continue;

                            if (target.Team == Shared.Damage_Team.FRIENDLY)
                            {
                                // ------------------------------------
                                if (attacker.Get_Bounds().Intersects(target.Get_Bounds()))
                                {
                                    if (attacker is NPC)
                                    {
                                        NPC npc = (NPC)attacker;
                                        target.Damage
                                            (
                                            npc,
                                            npc.True_Damage,
                                            npc.Velocity,
                                            npc.Damage_type
                                            );
                                        continue;
                                    }
                                }
                                // ------------------------------------
                            }
                        }
                    }
                }
            }
        }
        // ------------------------------------


        /// <summary>
        /// Sifts through all active projectiles for any collisions
        /// </summary>
        public static void Projectile_Collisions()
        {
            if (Particles.Active_Projectiles.Count() != 0)
            {
                // ------------------------------------
                // Establish a list of dead projectiles to be removed
                List<Projectile> dead_projectiles = new List<Projectile>();
                // ------------------------------------

                foreach (Projectile projectile in Particles.Active_Projectiles)
                {
                    foreach (Character character in Shared.Active_World.WorldCharacters)
                    {
                        // If this character is currently set to invulnerable,
                        // We shouldn't waste time checking.
                        if (character.Invulnerable)
                            break;

                        // If the damage team is neutral, it can hurt anyone. So check everyone.
                        // ------------------------------------
                        if (projectile.Damage_team == Shared.Damage_Team.NEUTRAL)
                        {
                            // Check for hit
                            // ------------------------------------
                            if (projectile.Get_Bounds().Intersects(character.Get_Bounds()))
                            {
                                character.Damage
                                    (
                                        projectile.Source,
                                        projectile.Damage,
                                        projectile.Velocity,
                                        projectile.Damage_type,
                                        projectile
                                    );

                                dead_projectiles.Add(projectile);
                                break;
                            }
                        }
                        // Hostile damage team can only hurt friendly characters
                        // ------------------------------------
                        if (projectile.Damage_team == Shared.Damage_Team.HOSTILE)
                        {
                            if (character.Team == Shared.Damage_Team.FRIENDLY)
                            {
                                // Check for hit
                                // ------------------------------------
                                if (projectile.Get_Bounds().Intersects(character.Get_Bounds()))
                                {
                                    character.Damage
                                        (
                                        projectile.Source,
                                        projectile.Damage,
                                        projectile.Velocity,
                                        projectile.Damage_type,
                                        projectile
                                        );

                                    dead_projectiles.Add(projectile);
                                    break;
                                }
                            }
                        }

                        // Friendly damage team can only hurt hostile characters
                        // ------------------------------------
                        if (projectile.Damage_team == Shared.Damage_Team.FRIENDLY)
                        {
                            if (character.Team == Shared.Damage_Team.HOSTILE)
                            {
                                // Check for hit
                                // ------------------------------------
                                if (projectile.Get_Bounds().Intersects(character.Get_Bounds()))
                                {
                                    character.Damage
                                        (
                                        projectile.Source,
                                        projectile.Damage,
                                        projectile.Velocity,
                                        projectile.Damage_type,
                                        projectile
                                        );

                                    dead_projectiles.Add(projectile);
                                    break;
                                }
                            }
                        }
                    }
                }
                // ------------------------------------
                // Remove any dead projectiles
                if (dead_projectiles.Count() != 0)
                    foreach (Projectile dead_projectile in dead_projectiles)
                        Particles.Active_Projectiles.Remove(dead_projectile);
                // ------------------------------------
            }
        }

        #endregion
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Range & LoS checks

        /// <summary>
        /// Intersection method that tells a character entity if a target is within a range.
        /// </summary>
        /// <param name="source">The origin character of our search</param>
        /// <param name="target">The target they're looking for</param>
        /// <param name="range">The range in which to search</param>
        /// <param name="in_range">A boolean out telling us whether or not the target was in range</param>
        /// <param name="direction">The direction in which the target was found, if any</param>
        public static void In_Range(Character source, Entity target, int range, out bool in_range, out Shared.Direction direction)
        {
            // Produce the range rectangle
            //-------------------------------------
            Rectangle range_box = new Rectangle
            (
            source.Get_Bounds().X - (int)(((range * Shared.Block_Dimension * Shared.Pixel_Scale) * 2) / 2),
            source.Get_Bounds().Y - (int)(((range * Shared.Block_Dimension * Shared.Pixel_Scale) * 2) / 2),
            source.Get_Bounds().Width + (int)(((range * Shared.Block_Dimension * Shared.Pixel_Scale) * 2)),
            source.Get_Bounds().Height + (int)(((range * Shared.Block_Dimension * Shared.Pixel_Scale) * 2))
            );

            //-------------------------------------
            // Debugger draws range boxes
            if(Shared.Debug)
                source.Draw_Box(range_box, 1, Color.Green);
            //-------------------------------------

            // Set default outs
            //-------------------------------------
            in_range = false;
            direction = Shared.Direction.NONE;

            if (range_box.Intersects(target.Get_Bounds()))
            {
                // Target is within range.
                //-------------------------------------
                in_range = true;
                int center_of = range_box.Left + (range_box.Width / 2);
                if (target.Get_Bounds().Right < center_of)
                {
                    // Then the target is on the left side
                    //-------------------------------------
                    direction = Shared.Direction.LEFT;
                }
                else
                {
                    // Otherwise the target is on the right.
                    //-------------------------------------
                    direction = Shared.Direction.RIGHT;
                }
            }
        }

        /// <summary>
        /// Determines whether or not an NPC has Line of sight of an input target
        /// </summary>
        /// <param name="source">The source NPC</param>
        /// <param name="target">The target to look for</param>
        /// <param name="range">The LoS range in blocks</param>
        /// <returns>Returns true if the target character is within LoS</returns>
        public static bool Line_Of_Sight(NPC source, Entity target, int range)
        {
            // Establish essential calculation data
            //-------------------------------------

            //-------------------------------------
            // The NPC's source of sight. Grid position, plus the eye offset tells us where there 'eyes' are

            Vector2 NPC_sight_source = new Vector2
                (
                source.Grid_Position.X + source.Eye_Offset.X,
                source.Grid_Position.Y + source.Eye_Offset.Y
                );
            //-------------------------------------
            // The line slope from our target position and the NPC's sight source

            Vector2 sight_slope = new Vector2
                (
                target.Grid_Position.X - NPC_sight_source.X,
                (target.Grid_Position.Y - NPC_sight_source.Y - 
                (int)Math.Floor((target.Get_Bounds().Height / Shared.Block_Dimension / Shared.Pixel_Scale)))
                );
            //-------------------------------------
            // The absolute of the slope. We need to quantify values later.

            Vector2 absolute_slope = new Vector2
                (
                Math.Abs(sight_slope.X),
                Math.Abs(sight_slope.Y)
                );
            //-------------------------------------
            // Calculates the amount of blocks we'll need to check based on the
            // highest vertical or horizontal distance

            int block_count = (int)Math.Max
                (
                absolute_slope.X,
                absolute_slope.Y
                );
            //-------------------------------------
            // In order to prevent falling a block short through odd division, 
            // add an extra block when using odd numbers

            if (block_count % 2 != 0)
            {
                block_count++;
            }
            //-------------------------------------

            //-------------------------------------
            // Set up some crucial variables

            int block_X = 0; // The current X position we're checking
            int block_Y = 0; // The current Y position we're checking
            bool x_split = false; // Whether we're splitting the horizontal distance
            bool y_split = false; // or the vertical distance. Distance splitting means we'll
                                  // get more natural lines, instead of L shapes

            int SPLIT_COUNT = 0; // Initialize
            int split_counter = 0; // Initialize
            //-------------------------------------


            //-------------------------------------
            // Prepare split counting if necessary

            if (absolute_slope.X > absolute_slope.Y)
            {
                // Since the horizontal distance is greater, we'll dispere the vertical increment
                // evenly throughout the horizontal line to assure we don't get any L shaped LoS lines
                y_split = true;
                SPLIT_COUNT = (int)Math.Ceiling(absolute_slope.X / absolute_slope.Y);
                split_counter = SPLIT_COUNT;
            }
            else if (absolute_slope.Y > absolute_slope.X)
            {
                // Since the vertical distance is greater, we'll dispere the horizontal increment
                // evenly throughout the horizontal line to assure we don't get any L shaped LoS lines
                x_split = true;
                SPLIT_COUNT = (int)Math.Ceiling(absolute_slope.Y / absolute_slope.X);
                split_counter = SPLIT_COUNT;
            }
            //-------------------------------------

            //-------------------------------------
            // Now let's run through all the blocks we need to check!
            for (int block = 0; block < block_count + 1; block++)
            {

                //-------------------------------------
                // Increment or decrement depending on split & slope
                split_counter++;
                if (block_X > sight_slope.X && (!x_split || split_counter == SPLIT_COUNT))
                    block_X--;
                else if (block_X < sight_slope.X && (!x_split || split_counter == SPLIT_COUNT))
                    block_X++;

                if (block_Y > sight_slope.Y && (!y_split || split_counter == SPLIT_COUNT))
                    block_Y--;
                else if (block_Y < sight_slope.Y && (!y_split || split_counter == SPLIT_COUNT))
                    block_Y++;
                //-------------------------------------


                //-------------------------------------
                // Reset split counter
                if (split_counter > SPLIT_COUNT)
                    split_counter = 1;
                //-------------------------------------

                //-------------------------------------
                // Check current block location for solid blocks

                int check_X = (int)(NPC_sight_source.X + block_X);
                int check_Y = (int)(NPC_sight_source.Y + block_Y);
                Terrain terrain = Shared.Active_World.Get_Terrain_At(check_X, check_Y);
                //-------------------------------------
                if (terrain is Terrain)
                {
                    return false; // A solid block lays in the way of our LoS
                }
                //-------------------------------------
            }

            return true; // With all blocks between us and our target clear, we have LoS
            //-------------------------------------
        }

        #endregion
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region NPC terrain navigation checks

        /// <summary>
        /// A simple terrain check that lets an NPC know if it's about to walk off an edge
        /// </summary>
        /// <param name="source">The NPC</param>
        /// <returns>Returns true if it's safe to walk</returns>
        public static bool Safe_Step(NPC source)
        {
            //-------------------------------------
            // Establish data
            Terrain floor_ahead;
            Vector2 source_pos = source.Grid_Position;
            //-------------------------------------

            //-------------------------------------
            // Check floor to the left
            if (source.Direction == Shared.Direction.LEFT)
            {
                floor_ahead = Shared.Active_World.Get_Terrain_At((int)source_pos.X - 1, (int)source_pos.Y + 1);
                if (floor_ahead is Terrain)
                    return true;
            }
            //-------------------------------------
            // Check floor to the right
            if (source.Direction == Shared.Direction.RIGHT)
            {
                floor_ahead = Shared.Active_World.Get_Terrain_At((int)source_pos.X + 1, (int)source_pos.Y + 1);
                if (floor_ahead is Terrain)
                    return true;
            }
            //-------------------------------------
            return false;
            //-------------------------------------
        }


        /// <summary>
        /// Safe drop is similar to safe step, but instead checks to make sure an NPC
        /// will be able to jump back up if they jump down.
        /// 
        /// Only use safe drop after having checked safe step
        /// </summary>
        /// <param name="source">The NPC</param>
        /// <returns>Returns true if it's safe to walk</returns>
        public static bool Safe_Drop(NPC source)
        {
            //-------------------------------------
            // Establish data
            Vector2 source_pos = source.Grid_Position;
            int direction_offset = 0;
            //-------------------------------------

            //-------------------------------------
            // Check floor to the left
            if (source.Direction == Shared.Direction.LEFT)
                direction_offset = -1;
            else
                direction_offset = 1;

            // Iterate for the amount of blocks this NPC can roughly jump high
            for (int check = 0; check < source.Jump_Height + 1; check++)
            {
                //-------------------------------------
                // Add Block if not null
                Terrain block = Shared.Active_World.Get_Terrain_At(
                            (int)source_pos.X + direction_offset,
                            (int)source_pos.Y + 1 + check);
                if (block != null)
                    return true;
                //-------------------------------------
            }
            //-------------------------------------

            //-------------------------------------
            return false;
            //-------------------------------------
        }

        /// <summary>
        /// Jumpable terrain lets NPC's know that they should even bother trying to jump. 
        /// No sense jumping against a wall you can't get up constantly!
        /// 
        /// </summary>
        /// <param name="source">The NPC</param>
        /// <returns>Returns true if jumping is s-m-a-r-t</returns>
        public static bool Jumpable_Terrain(NPC source)
        {
            //-------------------------------------
            // Establish data
            List<Terrain> quick_list = new List<Terrain>();
            Vector2 source_pos = source.Grid_Position;
            int direction_offset = 0;
            //-------------------------------------

            //-------------------------------------
            // Check floor to the left
            if (source.Direction == Shared.Direction.LEFT)
                direction_offset = -1;
            else
                direction_offset = 1;

            // Iterate for the amount of blocks this NPC can roughly jump high

            for (int check = 1; check <= source.Jump_Height; check++)
            {
                //-------------------------------------
                // Add Block if not null
                Terrain block = Shared.Active_World.Get_Terrain_At(
                            (int)source_pos.X + direction_offset,
                            (int)source_pos.Y - check);
                if (block == null)
                    return true;
                //-------------------------------------
            }
            //-------------------------------------
            return false;
            //-------------------------------------
        }
        #endregion
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }
}
