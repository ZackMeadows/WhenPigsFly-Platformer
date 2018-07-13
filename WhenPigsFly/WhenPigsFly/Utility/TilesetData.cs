// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Tileset Class
// Created 12/06/2015
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
    /// Tileset class, to further enhance block data & 
    /// improve the world generator
    /// </summary>
    public class Tileset
    {
        public Texture2D Tile;
        public string Name;
        public bool Dynamics;

        /// <summary>
        /// Basic tile constructor
        /// </summary>
        /// <param name="tile">The tile image</param>
        /// <param name="name">The name of the tile</param>
        public Tileset(Texture2D tile, string name, bool dynamic)
        {
            this.Tile = tile;
            this.Name = name;
            this.Dynamics = dynamic;
        }
    }
}
