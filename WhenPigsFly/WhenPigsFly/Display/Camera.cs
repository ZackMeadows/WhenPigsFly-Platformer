// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Game Camera Class
// Created 12/01/2015
//
// Heavily influenced by David Amador
// http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
//
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
    /// The Camera Class, used for controlling the game camera
    /// </summary>
    public class Camera
    {
        // --------------------------
        // Essentials
        // --------------------------
        private Matrix Screen;
        public Vector2 Position;

        private float Zoom_Level;
        private float Rotation;
        private bool Map_Lock = true;
        private bool Critical_Map_Lock = true;
        // --------------------------

        /// <summary>
        /// Default Camera Constructor
        /// </summary>
        public Camera() 
        {
            Zoom_Level = 3f;
            Rotation = 0f;
            Position = Vector2.Zero;
        }

        /// <summary>
        /// Shifts the focus of a camera to center upon a target vector location
        /// </summary>
        /// <param name="target">The target vector</param>
        public void Set_Focus(Vector2 target)
        {
            Position = new Vector2
            (
                target.X - (Shared.RESOLUTION.X / 2 / Zoom_Level),
                target.Y - (Shared.RESOLUTION.Y / 2 / Zoom_Level)
            );

            if (Map_Lock)
            {
                // --------------------------
                Rectangle Screen_View = Get_WorldSpace();
                // --------------------------
                // Keeps the camera within the bounds of the map,
                // But doesn't restrict it's maximum if it's smaller than the actual resolution
                //
                // We Keep it within +1 block as to provide easy peasy map boundaries
                if (Screen_View.X < Shared.Block_Dimension * Shared.Pixel_Scale)
                {
                    Position.X = Shared.Block_Dimension * Shared.Pixel_Scale;
                }

                // --------------------------
                // With Critical Lock active, the camera will enforce a change to the minimum zoom
                // Ensuring that the screen can fit to the map size.
                if (Critical_Map_Lock)
                {
                    if (Shared.Active_World.Size.X <= Shared.RESOLUTION.X / Zoom_Level 
                        || Shared.Active_World.Size.Y <= Shared.RESOLUTION.Y / Zoom_Level)
                    {
                        Shared.TEMP_MIN_ZOOM = Math.Max
                            (
                                (float)Math.Floor(Shared.RESOLUTION.X / Shared.Active_World.Size.X),
                                (float)Math.Floor(Shared.RESOLUTION.Y / Shared.Active_World.Size.Y)
                            );
                        Shared.ZOOM_LEVEL = Shared.TEMP_MIN_ZOOM;
                        Set_Zoom(Shared.ZOOM_LEVEL);
                    }
                }
                // --------------------------

                // --------------------------
                // These checks only run if the map size is bigger than the view size, in order
                // to prevent conflicts
                if (Shared.Active_World.Size.X >= Screen_View.Width)
                {
                    if (Screen_View.X + Screen_View.Width > Shared.Active_World.Size.X - Shared.Block_Dimension)
                    {
                        Position.X = Shared.Active_World.Size.X - Screen_View.Width - Shared.Block_Dimension;
                    }
                }
                if (Shared.Active_World.Size.Y >= Screen_View.Height)
                {
                    if (Screen_View.Y + Screen_View.Height > Shared.Active_World.Size.Y - Shared.Block_Dimension)
                    {
                        Position.Y = Shared.Active_World.Size.Y - Screen_View.Height - Shared.Block_Dimension;
                    }
                }
                // --------------------------
            }

        }
        // --------------------------

        /// <summary>
        /// Sets the zoom level of the camera
        /// </summary>
        /// <param name="input">The new zoom value</param>
        public void Set_Zoom(float input)
        {
            Zoom_Level = input;
        }
        // --------------------------

        /// <summary>
        /// Sets the zoom level of the camera
        /// </summary>
        /// <param name="input">The new zoom value</param>
        public void Set_Rotation(float input)
        {
            Rotation = input;
        }
        // --------------------------

        /// <summary>
        /// Gets the cameras view matrix
        /// </summary>
        /// <returns></returns>
        public Matrix Get_Camera()
        {
            GraphicsDevice graphics = Shared.GfxDevice;
            Screen =
                    Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(Zoom_Level);

            return Screen;
        }
        // --------------------------

        /// <summary>
        /// Gets the world space thats within the camera
        /// </summary>
        /// <returns>Returns a rectangle equal to viewed world space</returns>
        public Rectangle Get_WorldSpace()
        {
            Rectangle World_Space = new Rectangle
                (
                (int)Position.X,
                (int)Position.Y,
                (int)(Shared.RESOLUTION.X / Zoom_Level),
                (int)(Shared.RESOLUTION.Y / Zoom_Level)
                );

            return World_Space;
        }
        // --------------------------

        /// <summary>
        /// Gets the world space position of the mouse
        /// </summary>
        /// <returns></returns>
        public Vector2 Get_Mousespace(Vector2 mouse)
        {
            Matrix Inverse = Matrix.Invert(Screen);
            Vector2 Mouse_Pos = Vector2.Transform(mouse, Inverse);

            return Mouse_Pos;
        }
        // --------------------------
    }
}
