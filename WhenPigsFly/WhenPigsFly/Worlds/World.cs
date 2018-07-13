// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/2/2015
//
// ------------------------------
// World Functions
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
    /// World Class - For all your world godding needs
    /// </summary>
    public class World : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // -----------------------------
        // World Instance Variables
        // -----------------------------
        private Vector2 size;
        public bool terrain_changed = false;
        public bool loaded = false;
        public bool has_background = false;

        private WorldParallax worldParallax;

        private Dictionary<string, Terrain> worldTerrain;
        private Dictionary<string, Terrain> worldBKGTerrain;

        private List<Character> worldCharacters;

        #region Getters & Setters
        public Vector2 Size
        { get { return size; } set { size = value; } }
        public Dictionary<string, Terrain> WorldTerrain
        { get { return worldTerrain; } set { worldTerrain = value; } }
        public Dictionary<string, Terrain> WorldBKGTerrain
        { get { return worldBKGTerrain; } set { worldBKGTerrain = value; } }

        public List<Character> WorldCharacters
        { get { return worldCharacters; } set { worldCharacters = value; } }
        #endregion

        // -----------------------------

        /// <summary>
        /// Basic World Constructor.
        /// World is used to contain all non-player entities.
        /// </summary>
        public World(Vector2 size) : base(Shared.Main)
        {
            this.size = size;

            worldTerrain = new Dictionary<string, Terrain>();
            worldBKGTerrain = new Dictionary<string, Terrain>();
            worldCharacters = new List<Character>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // World Render Order
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                // Prepare dead character list
                List<Character> Dead_Characters = new List<Character>();

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // Update 
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                
                // World Parallax
                // --------------------------------
                worldParallax.Update(gameTime);

                // Perform Combat Collision Checks
                // --------------------------------
                CollisionManager.Projectile_Collisions();
                CollisionManager.Character_Collision();

                // Terrain
                // --------------------------------
                foreach (KeyValuePair<string,Terrain> terrain in worldTerrain)
                {
                    terrain.Value.Update(gameTime);

                    if (terrain_changed)
                        terrain.Value.Terrain_Render();
                }
                if (has_background && worldBKGTerrain.Count() != 0)
                {
                    foreach (KeyValuePair<string, Terrain> terrain in worldBKGTerrain)
                    {
                        terrain.Value.Update(gameTime);

                        if (terrain_changed)
                            terrain.Value.Terrain_Render();
                    }
                }
                // Reset terrain changed flag
                terrain_changed = false;
                // ----------------------

                // Characters
                // --------------------------------
                if (Shared.Active_World.loaded)
                {
                    foreach (Character character in worldCharacters)
                    {
                        if (character.Is_dead() && !(character is Player))
                        {
                            Dead_Characters.Add(character);
                            character.Enabled = false;
                            break;
                        }
                        character.Update(gameTime);
                    }

                    if (Dead_Characters.Count() != 0)
                      foreach (Character dead_character in Dead_Characters)
                        worldCharacters.Remove(dead_character);
                }
                // --------------------------------

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // Draw 
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                // BKG Terrain
                // --------------------------------
                if (has_background && worldBKGTerrain.Count() != 0)
                {
                    foreach (KeyValuePair<string, Terrain> terrain in worldBKGTerrain)
                    {
                        terrain.Value.Draw(gameTime);
                    }
                }

                // Characters
                // --------------------------------
                foreach (Character character in worldCharacters)
                {
                        character.Draw(gameTime);
                }

                // Terrain
                // --------------------------------
                foreach (KeyValuePair<string, Terrain> terrain in worldTerrain)
                {
                    terrain.Value.Draw(gameTime);
                } 

                if (Shared.Debug)
                {
                    string key = Get_Grid_Key(Shared.Player.Grid_Position.X, Shared.Player.Grid_Position.Y + 1);
                    if (WorldTerrain.ContainsKey(key))
                        Shared.DebugWindow.Message += WorldTerrain[key].Name + "\n";
                    else
                        Shared.DebugWindow.Message += "Air \n";

                    Shared.DebugWindow.Draw(gameTime);
                    Shared.DebugWindow.Message = "";
                }
                // -------------------------------- 

                // Effects
                // --------------------------------
                Particles.Draw_Effects(gameTime);
                // -------------------------------- 

                // Projectiles
                // --------------------------------
                Particles.Draw_Projectiles(gameTime);
                // -------------------------------- 

                // GUI
                // --------------------------------
                foreach (GUI_Element element in Shared.GUI)
                    element.Draw(gameTime);
                // --------------------------------
            }
        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Static Features Begins
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// The base world generator
        /// </summary>
        /// <param name="world">The world pixel map to referrence</param>
        /// <param name="destination">The gamescene which to generate the world in.</param>
        /// <returns></returns>
        public static string Generate(string world, World destination)
        {
            // -------------------------------
            // Ensure we've been passed a useable world map
            // -------------------------------
            if (!Shared.IMG_Worlds.ContainsKey(world))
            {
                return "Failed";
            }

            Player player_ENTITY = new Player();
            Shared.Player = player_ENTITY;

            // -------------------------------
            // We're safe. Let's continue constructing our world file.
            // -------------------------------

            // -------------------------------
            // Let's get some parallax in here!!
            // -------------------------------

            destination.worldParallax = Shared.World_Parallax["Overworld"];

            // -------------------------------
            List<string> world_codes = Shared.HexBuilder(Shared.IMG_Worlds[world]);
            int map_width = Shared.IMG_Worlds[world].Width;
            // -------------------------------

            // With our list, we can now begin placing objects following the
            // Pixel map guide.
            // -------------------------------
            int breakCounter = 0; // Keeps track of when we should be shifting to the next layer
            int x_place = 0;
            int y_place = 0;

            string key = "";
            bool player_placed = false;
            // -------------------------------
            #region For Foreground
            for (int code = 0; code < world_codes.Count(); code++)
            {
                // -------------------------------
                // Initiate this iteration's block key
                key = "X" + x_place + "Y" + y_place;
                string hexCode = world_codes[code];
                // -------------------------------

                // -------------------------------
                // Check Tileset for tile with matching hex key
                if (Shared.Tileset.ContainsKey(hexCode))
                {
                    Terrain block = new Terrain(
                        Shared.Tileset[hexCode].Tile,
                        new Vector2
                            (
                            x_place * Shared.Block_Dimension * Shared.Pixel_Scale,
                            y_place * Shared.Block_Dimension * Shared.Pixel_Scale
                            ),
                            Shared.Tileset[hexCode].Dynamics
                        );
                    block.Grid_Position = new Vector2(x_place, y_place);
                    block.Name = Shared.Tileset[hexCode].Name;

                    destination.WorldTerrain.Add(key, block);
                }

                if (world_codes[code] == "FF0000")
                {
                    Vector2 spawner = new Vector2
                            (
                            x_place * Shared.Block_Dimension * Shared.Pixel_Scale,
                            y_place * Shared.Block_Dimension * Shared.Pixel_Scale
                            );
                    Goblin monster = new Goblin(spawner);

                    monster.Post_Position = new Vector2(x_place, y_place);
                    destination.WorldCharacters.Add(monster);
                }
                // -------------------------
                // Player Spawn Point
                if (world_codes[code] == "00FF00" && !player_placed)
                {
                    Shared.Player.Position = 
                        new Vector2
                            (
                            x_place * Shared.Block_Dimension * Shared.Pixel_Scale,
                            y_place * Shared.Block_Dimension * Shared.Pixel_Scale
                            );

                    // Set default respawn point;
                    Shared.Player.Respawn_Point = Shared.Player.Position;

                    player_placed = true; // Ensures only one player is placed
                    destination.WorldCharacters.Add(player_ENTITY);
                }
                // -------------------------


                // -------------------------
                x_place++;
                breakCounter++;

                if (breakCounter == map_width)
                {
                    breakCounter = 0;
                    x_place = 0;
                    y_place++;
                }
                // -------------------------
            }
            #endregion
            // -------------------------------
            destination.Size = new Vector2(
                (map_width - 1) * Shared.Block_Dimension * Shared.Pixel_Scale, 
                (y_place - 1) * Shared.Block_Dimension * Shared.Pixel_Scale);
            destination.terrain_changed = true;

            // -------------------------------
            // Prepare for paired background file

            if (!Shared.IMG_Worlds.ContainsKey(world + "_bkg"))
            {
               // Doesn't exist, skip.
                return "No BKG";
            }
            world_codes = Shared.HexBuilder(Shared.IMG_Worlds[world + "_bkg"]);
            int bkg_map_width = Shared.IMG_Worlds[world + "_bkg"].Width;

            if (bkg_map_width > map_width)
            {
                // This image is faulty. Skip.
                return "BKG too big.";
            }
            breakCounter = 0; // Keeps track of when we should be shifting to the next layer
            x_place = 0;
            y_place = 0;

            key = "";
            // -------------------------------
            #region For Background
            for (int code = 0; code < world_codes.Count(); code++)
            {
                // -------------------------------
                // Initiate this iteration's block key
                key = "X" + x_place + "Y" + y_place;
                string hexCode = world_codes[code];
                // -------------------------------

                // -------------------------------
                // Check Tileset for tile with matching hex key
                if (Shared.Tileset.ContainsKey(hexCode))
                {
                    Terrain block = new Terrain(
                        Shared.Tileset[hexCode].Tile,
                        new Vector2
                            (
                            x_place * Shared.Block_Dimension * Shared.Pixel_Scale,
                            y_place * Shared.Block_Dimension * Shared.Pixel_Scale
                            ),
                            Shared.Tileset[hexCode].Dynamics,
                            true
                        );
                    block.Grid_Position = new Vector2(x_place, y_place);
                    block.Name = Shared.Tileset[hexCode].Name;

                    destination.WorldBKGTerrain.Add(key, block);
                }
                // -------------------------


                // -------------------------
                x_place++;
                breakCounter++;

                if (breakCounter == map_width)
                {
                    breakCounter = 0;
                    x_place = 0;
                    y_place++;
                }
                // -------------------------
            }
            #endregion
            // -------------------------------
            if (destination.WorldBKGTerrain.Count() != 0)
                destination.has_background = true;

            destination.loaded = true;
            return "Complete";
        }

        /// <summary>
        /// A simple method that returns a vector grid position as a recognizeable world Terrain dictionary key
        /// </summary>
        /// <param name="Grid_Position">The grid position to find a key for</param>
        /// <returns>A recognizeable world terrain dictionary key string</returns>
        public string Get_Grid_Key(Vector2 Grid_Position)
        {
            string key = "X" + Grid_Position.X + "Y" + Grid_Position.Y;
            return key;
        }

        /// <summary>
        /// A simple method that returns a vector grid position as a recognizeable world Terrain dictionary key
        /// </summary>
        /// <param name="X">The X grid location</param>
        /// <param name="Y">The Y grid location</param>
        /// <returns>A recognizeable world terrain dictionary key string</returns>
        public string Get_Grid_Key(float X, float Y)
        {
            string key = "X" + X + "Y" + Y;
            return key;
        }

        /// <summary>
        /// A simple method that returns a vector grid position as a recognizeable world Terrain dictionary key
        /// </summary>
        /// <param name="X">The X grid location</param>
        /// <param name="Y">The Y grid location</param>
        /// <returns>A recognizeable world terrain dictionary key string</returns>
        public string Get_Grid_Key(int X, int Y)
        {
            string key = "X" + X + "Y" + Y;
            return key;
        }
        // -------------------------------

        /// <summary>
        /// A simple method that gets the terrain entity at a grid location.
        /// </summary>
        /// <param name="X">The X to check</param>
        /// <param name="Y">The Y to check</param>
        /// <returns>Returns a terrain entity, null if there is none.</returns>
        public Terrain Get_Terrain_At(int X, int Y, bool background = false)
        {
            Terrain terrain_entity = null;

            string key = "X" + X + "Y" + Y;

            if (!background)
            {
                if(WorldTerrain.ContainsKey(key))
                {
                    terrain_entity = WorldTerrain[key];
                }
            }
            else
            {
                if (WorldBKGTerrain.ContainsKey(key))
                {
                    terrain_entity = WorldBKGTerrain[key];
                }
            }
            return terrain_entity;
        }
    }
}
