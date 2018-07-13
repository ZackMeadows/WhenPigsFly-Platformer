// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Shared Configuration
// Created 11/28/2015
// ------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhenPigsFly
{
    // ------------------------------
    /// <summary>
    /// Clock allows the simple organization of timers, delays, and decision making
    /// </summary>
    public class Clock
    {
        public float Time = 0f; // The current float value held within the timer
        public int Primary_Decision = 0; // Primary Decision Making
        public int Secondary_Decision = 0; // Secondary Decision Making
    }
    // ------------------------------

    public class Shared
    {
        // -------------------------------
        // Public Globals
        // --------------------------------
        public static int Count = 0;
        public static bool Debug = false;
        public static Random Randomizer = new Random();
        public static BasicText DebugWindow;
        public static Texture2D Pixel;
        public static SpriteBatch Batch;
        public static Game Main;
        public static MainMenu MainMenu;
        public static DeadScene DeadScene;

        public static int Block_Dimension = 16;
        public static Vector2 Gravity = new Vector2(0, -14f);
        public static Player Player;
        public static bool Player_HeldSpace;
        public static World Active_World;

        public static List<GUI_Element> GUI = new List<GUI_Element>();
        public static Rectangle Scene;
        public static Camera Parallax_Cam = new Camera();
        public static Camera Screen = new Camera();
        public static GraphicsDevice GfxDevice;
        public static Vector2 CameraFocus = Vector2.Zero;
        public static float Pixel_Scale = 3;

        public static float ZOOM_LEVEL = 1;
        public static bool ZOOM_LOCK = false;

        public static float MAX_ZOOM = 5;
        public static float TEMP_MIN_ZOOM = -1;
        public static float MIN_ZOOM = 1;

        public static int FPS = 3;
        public static Vector2 RESOLUTION = new Vector2(1024, 768);
        public static bool Fullscreen = false;
        // --------------------------------

        // Public Asset Libraries
        // --------------------------------
        public static Dictionary<string, Texture2D> IMG_Interface;
        public static Dictionary<string, Texture2D> IMG_Items;
        public static Dictionary<string, Texture2D> IMG_Sprites;
        public static Dictionary<string, Texture2D> IMG_Worlds;

            // Effects
            // -------
            public static Dictionary<string, Texture2D> IMG_Animations;
            public static Dictionary<string, Texture2D> IMG_Emotes;
            public static Dictionary<string, Texture2D> IMG_Projectiles;

        public static Dictionary<string, Texture2D> IMG_Tiles;
        public static Dictionary<string, Tileset> Tileset;

        public static Dictionary<string, Texture2D> IMG_Parallax;
        public static Dictionary<string, WorldParallax> World_Parallax;

        public static Dictionary<string, SoundEffect> SFX_Sounds;
        public static Dictionary<string, Song> AUDIO_Music;
        // --------------------------------

        // Controls
        // --------------------------------
        public static Keys KEY_Debug = Keys.OemTilde;
        public static Keys KEY_FULLSCREEN = Keys.F1;

        public static Keys MOVE_Left = Keys.A;
        public static Keys MOVE_Right = Keys.D;
        public static Keys MOVE_Crouch = Keys.S;
        public static Keys MOVE_Jump = Keys.Space;
        public static Keys MOVE_Run = Keys.LeftShift;

        public static Keys ATK_Up = Keys.Up;
        public static Keys ATK_Right = Keys.Right;
        public static Keys ATK_Left = Keys.Left;
        public static Keys ATK_Down = Keys.Down;
        // --------------------------------

        // Enumerations
        // --------------------------------

        /// <summary>
        /// States available to entities relevant to animations, actions, & physics
        /// </summary>
        public enum EntityState
        {
            IDLE,
            CROUCHING,
            WALKING,
            RUNNING,
            JUMP_WIND,
            JUMPING,
            FALLING,
            DYING,
            DEAD,
            ATTACKING,
            SIDE_ATTACK,
            UP_ATTACK,
            DOWN_ATTACK
        };

        /// <summary>
        /// Basic Cardinal Directions
        /// </summary>
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            NONE
        }

        /// <summary>
        /// Damage Teams -- To determine who an attack should effect.
        /// </summary>
        public enum Damage_Team
        {
            NEUTRAL,
            FRIENDLY,
            HOSTILE
        }

        /// <summary>
        /// Types -- You know, for rock paper scissors esque tactics!!!
        /// </summary>
        public enum Type
        {
            MUNDANE,
            PIERCING,
            ARCANE,
            FIRE,
            WATER,
            ICE,
            ROCK, 
            LIGHTNING,
            POISON,
            TOXIC,
            DEMONIC,
            HOLY
        }
        /// <summary>
        /// Converts a Texture2D into a list of hexcodes based on
        /// each pixel.
        /// </summary>
        /// <param name="image">The Texture2D to be converted into hexadecimal color codes</param>
        /// <returns>Returns a list of strings (hexadecimals)</returns>
        public static List<string> HexBuilder(Texture2D image)
        {
            // -------------------------------
            // Convert world pixel map to RGBA colors
            // -------------------------------
            Color[] pixel_map = new Color
                [
                image.Width *
                image.Height
                ];
            image.GetData(pixel_map);
            // -------------------------------
            // Convert RGBA colors to a functional hexcode
            // -------------------------------
            List<string> image_codes = new List<string>();
            foreach (Color item in pixel_map)
            {
                string slot = "";
                slot += item.R.ToString("X2");
                slot += item.G.ToString("X2");
                slot += item.B.ToString("X2");
                image_codes.Add(slot);
            }
            return image_codes;
            // -------------------------------
        }
    }
}
