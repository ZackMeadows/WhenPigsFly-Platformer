// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Terrain Abstract Class
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
    /// The terrain class extends Entity, and should be used for all terrain in which
    /// Characters & Other collide with
    /// </summary>
    public class Terrain : Entity
    {
        // ---------------------
        // Flags
        // ---------------------
        private bool dynamic_render = true;
        public bool background = false;
        // ---------------------
        // Data
        // ---------------------
        private string terrain_name = "Unnamed";

        private Rectangle tile_source = Rectangle.Empty;
        internal string tile_type = "Regular";
        private Vector2 dimensions = new Vector2(Shared.Block_Dimension, Shared.Block_Dimension);

        // ---------------------
        #region Getters & Setters

        public string Name
        { get { return terrain_name; } set { terrain_name = value; } }
        public Rectangle Tile_Source
        {get { return tile_source; }set { tile_source = value; }}
        public Vector2 Dimensions
        { get { return dimensions; }set { dimensions = value; }}
        #endregion
        // ---------------------

        /// <summary>
        /// Basic Terrain constructor
        /// </summary>
        public Terrain(Texture2D image, Vector2 input_position, bool dynamic_render, bool background = false): base()
        {
            this.Still_image = image;
            this.Position = input_position;
            this.dynamic_render = dynamic_render;
            this.background = background;
        }
        // ---------------------

        /// <summary>
        /// Tile render runs upon the completion of world generation- and should be
        /// ran again if the world terrain is changed. Tile render changes the tiles frame according to
        /// adjacent tiles
        /// </summary>
        public void Terrain_Render()
        {
            // ---------------------
            // If this tile is not flagged as dynamically rendered, just use it's base frame.
            tile_source = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            if (!dynamic_render)
                return;
            // ---------------------

            // Determine adjacent block types, and if they're the same as this
            // ---------------------

            int tile_x = 0;
            int tile_y = 0;

            // Flags
            // ---------------------
            bool above = false;
            bool below = false;
            bool right = false;
            bool left = false;

            bool above_right = false;
            bool above_left = false;
            bool below_right = false;
            bool below_left = false;
            // ---------------------
            #region Adjacent Checks
            //-------------------------------------
            // Above check
            Terrain blockabove = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X,
                        (int)Grid_Position.Y - 1,
                        background
                        );
            if (blockabove != null)
            {
                if(blockabove.terrain_name == terrain_name)
                    above = true;
            }
            //-------------------------------------
            // Below check
            Terrain blockbelow = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X,
                        (int)Grid_Position.Y + 1,
                        background
                        );
            if (blockbelow != null)
                below = true;
            //-------------------------------------
            // Right check
            Terrain blockright = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X + 1,
                        (int)Grid_Position.Y,
                        background
                        );
            if (blockright != null)
            {
                if (blockright.terrain_name == terrain_name)
                    right = true;
            }
            //-------------------------------------
            // Left check
            Terrain blockleft = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X - 1,
                        (int)Grid_Position.Y,
                        background
                        );
            if (blockleft != null)
            {
                if (blockleft.terrain_name == terrain_name)
                    left = true;
            }
            //-------------------------------------
            // Above Right check
            Terrain blockaboveright = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X + 1,
                        (int)Grid_Position.Y - 1,
                        background
                        );
            if (blockaboveright != null)
                above_right = true;
            //-------------------------------------
            // Above Left check
            Terrain blockaboveleft = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X - 1,
                        (int)Grid_Position.Y - 1,
                        background
                        );
            if (blockaboveleft != null)
                above_left = true;
            //-------------------------------------
            // Below Right check
            Terrain blockbelowright = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X + 1,
                        (int)Grid_Position.Y + 1,
                        background
                        );
            if (blockbelowright != null)
                below_right = true;
            //-------------------------------------
            // Below Left check
            Terrain blockbelowleft = Shared.Active_World.Get_Terrain_At
                        (
                        (int)Grid_Position.X - 1,
                        (int)Grid_Position.Y + 1,
                        background
                        );
            if (blockbelowleft != null)
                below_left = true;
            //-------------------------------------
            #endregion
            // ---------------------

            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // This is a pretty long and ugly method for automatically rendering
            // Tiles from the tile sheet. Instead of commenting everything, for now just understand
            // That this clump runs through tiles and compares things such as the tiles around,
            // and sometimes the tile type it was previously set to, in order to chose on the proper
            // tile type for its position.

            // For example, if a block discovers that there are tiles all around it, except for directly above,
            // It will know to choose a top edge block. Yes. Amazing!

            // ---------------------
            #region Tiles with air Right
            if (!right)
            {
                tile_x = 2;
                tile_y = 1;
            }
            #endregion
            // ---------------------

            // ---------------------
            #region Tiles with air Left
            if (!left)
            {
                if (right)
                {
                    tile_x = 0;
                    tile_y = 1;
                }
                if (!right)
                {
                    tile_x = 1;
                    tile_y = 5;
                }
            }
            #endregion
            // ---------------------

            // ---------------------
            #region Tiles with air Below
            if (!below)
            {
                tile_x = 1;
                tile_y = 2;
                if (!left)
                {
                    tile_x = 0;
                    tile_y = 2;
                }
                if (!right)
                {
                    tile_x = 2;
                    tile_y = 2;
                }
                if (!right && !left && above)
                {
                    tile_x = 1;
                    tile_y = 6;
                }
            }
            #endregion
            // ---------------------

            // ---------------------
            #region Tiles with air Above
            if (!above)
            {
                tile_x = 1;
                tile_y = 0;
                if (!left)
                {
                    tile_x = 0;
                    tile_y = 0;
                }
                if (!right)
                {
                    tile_x = 2;
                    tile_y = 0;
                }
                if (!below)
                {
                    tile_x = 1;
                    tile_y = 3;
                }
                if (!right && !left && below)
                {
                    tile_x = 1;
                    tile_y = 4;
                }
                if (!right && !below && left)
                {
                    tile_x = 2;
                    tile_y = 3;
                }
                if (!left && !below && right)
                {
                    tile_x = 0;
                    tile_y = 3;
                }
                if (!right && !left && !below)
                {
                    tile_x = 1;
                    tile_y = 7;
                }
            }
            #endregion
            // ---------------------

            // ---------------------
            #region Surrounded Tile & Surrounded Corner Tile
            if (above && below && left && right)
            {
                    tile_x = 1;
                    tile_y = 1;
                    if (!above_left && !above_right)
                    {
                        tile_x = 1;
                        tile_y = 0;
                    }
                    if (!above_left && !above_right && !below_left && !below_right)
                    {
                        tile_x = 1;
                        tile_y = 5;
                    }
            }

            if (blockabove != null)
            {
                if (blockabove.terrain_name != terrain_name)
                {
                    if (tile_y == 1)
                    {
                        tile_y = 0;
                    }
                    if (tile_y == 2)
                    {
                        tile_y = 3;
                    }
                    if (tile_y == 5)
                    {
                        tile_y = 0;  
                    }
                }
            }
            if (above && below && right && left)
            {
                if (blockaboveleft == null)
                {
                    tile_x = 2;
                    tile_y = 4;
                }
                else if (blockaboveleft.terrain_name != terrain_name)
                {
                    tile_x = 2;
                    tile_y = 4;
                }
                if (blockaboveright == null)
                {
                    tile_x = 0;
                    tile_y = 4;
                }
                else if (blockaboveright.terrain_name != terrain_name)
                {
                    tile_x = 0;
                    tile_y = 4;
                }

                if (blockaboveright == null && blockaboveleft == null)
                {
                       tile_x = 0;
                        tile_y = 5;
                }

                if (blockaboveright != null & blockaboveleft != null)
                {
                    if (blockaboveright.terrain_name != terrain_name 
                        && blockaboveleft.terrain_name != terrain_name)
                    {
                        tile_x = 0;
                        tile_y = 5;
                    }
                }
            }
            #endregion
            // ---------------------

            // ---------------------
            #region Special Tile Instances

                #region Castle Block
                if(terrain_name == "castle_block")
                {
                    if (tile_y == 0 && tile_x == 1)
                    {
                        if (!below_left && !below_right)
                        {
                            tile_x = 1;
                            tile_y = 4;
                        }

                        if (below && left && !below_left)
                        {
                            tile_x = 0;
                            tile_y = 0;
                        }
                    }
                    if (tile_y == 0 && tile_x == 0 && !below_right)
                    {
                        tile_x = 1;
                        tile_y = 4;
                    }
                    if (tile_y == 0 && tile_x == 2 && !below_left)
                    {
                        tile_x = 1;
                        tile_y = 4;
                    }

                    if (above && tile_y == 1 && tile_x == 0 && !above_right)
                    {
                        tile_x = 0;
                        tile_y = 0;
                    }
                    if (above && tile_y == 1 && tile_x == 2 && !above_left)
                    {
                        tile_x = 2;
                        tile_y = 0;
                    }
                }
                #endregion

            #endregion
            // ---------------------

            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            // ---------------------
            // We know what tile from the sheet we need, so grab it.
            tile_source = new Rectangle
                (
                (int)(dimensions.X * tile_x),
                (int)(dimensions.X * tile_y), 
                (int)dimensions.X, 
                (int)dimensions.Y
                );
            // ---------------------
        }
        // ---------------------

        /// <summary>
        /// Generates all the loaded tiles into dictionary with hexcodes recognizeable to
        /// the world generation system
        /// </summary>
        /// <param name="tile_images">A Dictionary of Texture2D images representing tiles</param>
        public static void Generate_Tiles(Dictionary<string, Texture2D> tile_images)
        {
            // ---------------------
            // Iterate through each loaded tileset image
            foreach (KeyValuePair<string, Texture2D> tileset in tile_images)
            {
                // ---------------------
                // Convert the image into hexadecimal color codes

                List<string> tileset_hexadecimals = Shared.HexBuilder(tileset.Value);

                // ---------------------
                // This little fella grabs us the pixel color from the bottom left corner. Amazing!
                // We'll use that as the hex identifier key for our map generater

                string pixel_identifer = tileset_hexadecimals
                    [
                    ((tileset.Value.Width * tileset.Value.Height) - 1) - 
                    (tileset.Value.Width - 1)
                    ];
                // ---------------------
                // Determine if this is a dynamic tile
                bool dynamic = false;
                if (tileset.Value.Width > Shared.Block_Dimension)
                    dynamic = true;

                // ---------------------
                // Now, let's add this tile to our Blockset
                Tileset tile = new Tileset(tileset.Value, tileset.Key, dynamic);
                Shared.Tileset.Add(pixel_identifer, tile);
                // ---------------------
            }
        }
        // ---------------------

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Draw & Update & Bounds
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // ----------------------------------
        #region Draw & Update & Bounds

        /// <summary>
        /// Updates the terrain entity
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

            float leniance_range = 10 * Shared.Block_Dimension;

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
        }

        /// <summary>
        /// Base entity draw method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                Rectangle source = Tile_Source;
                Vector2 origin = new Vector2(Dimensions.X / 2, Dimensions.Y / 2);
                Texture2D image = Still_image;
                Vector2 draw_position = Position;
                SpriteEffects flipDirection;
                
                // --------------------
                // Direction
                // --------------------
                if (direction == Shared.Direction.LEFT)
                    flipDirection = SpriteEffects.FlipHorizontally;
                else
                    flipDirection = SpriteEffects.None;

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
                if (background)
                    Overlay_Color = Color.Gray;
                else
                    Overlay_Color = Color.White;
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
            return new Rectangle
                (
                (int)Position.X - (int)((Dimensions.X / 2) * Shared.Pixel_Scale),
                (int)Position.Y - (int)((Dimensions.Y / 2) * Shared.Pixel_Scale),
                (int)(Dimensions.X * Shared.Pixel_Scale),
                (int)(Dimensions.Y * Shared.Pixel_Scale)
                );
        }
        // ----------------------------------
        #endregion
        // ----------------------------------
    }
}
