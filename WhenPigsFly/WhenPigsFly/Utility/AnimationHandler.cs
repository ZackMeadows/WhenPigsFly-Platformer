// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Animation Handler
// Created 11/27/2015
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
    /// Animation Class, Allows additional data to be added for custom animation
    /// </summary>
    public class Animation
    {
        public List<Rectangle> Frames;
        public bool Loop;

        public Animation()
        { Frames = new List<Rectangle>(); }
    }

    /// <summary>
    /// Contains useful methods for controlling and generating animations
    /// </summary>
    public static class AnimationHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture">The spritesheet to be used</param>
        /// <param name="frameSize">The size of the frame. Spritesheet must be uniform, or else this will not work.</param>
        /// <param name="length">The amount of frames in this animation</param>
        /// <param name="start">The vertical starting position on the spritesheet</param>
        /// <param name="uniqueDelay">An optional array input that should align properly with the frame count.
        /// <param name="x_start">The horizontal starting position on the spritesheet</param>
        /// uniqueDelay will allow an animation to specify a certain frame to play longer by repeating its addition to the animation list.</param>
        /// <returns></returns>
        public static Animation Generate(
            Texture2D texture, 
            Vector2 frameSize, 
            int start = 1, 
            int length = -1, 
            int[] uniqueDelay = null, 
            bool loop = true,
            int x_start = 1)
        {
            Animation animation = new Animation();
            animation.Loop = loop;
            Vector2 initialCorner = new Vector2
                (
                // Ease of use fix, subtracting one from start
                // Simplifies the programmers thought process.
                // For example, if you had input "1" originally, it would
                // Consider the origin for sheet using frame size 64x64 as (64,64),
                // Instead of the intended (0,0)
                frameSize.X * x_start,
                (start - 1) * frameSize.Y
                );
            // --------------------
            // Check for unset length. If unset, just run for the length of the file.
            if (length == -1)
                length = (int)(Math.Round(texture.Width / frameSize.X) - 1);

            for (int frame = 1; frame <= length; frame++)
            {
                Rectangle newFrame = new Rectangle
                    (
                    (int)initialCorner.X * frame, 
                    (int)initialCorner.Y, 
                    (int)frameSize.X, 
                    (int)frameSize.Y
                    );
                animation.Frames.Add(newFrame);
                if (uniqueDelay != null)
                {
                    for (int repeat = 0; repeat < uniqueDelay[frame-1]; repeat++)
                    {
                        animation.Frames.Add(newFrame);
                    }
                }
            }
            return animation;
        }
    }
}
