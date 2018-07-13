// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Attack Class
// Created 12/12/2015
// ------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhenPigsFly
{
    /// <summary>
    /// Attack class is used to hold varying information for character attacks
    /// </summary>
    public class Attack
    {
        public List<Rectangle> Frames = new List<Rectangle>(); // The attack animation frames
        public SoundEffect SFX = null;
        public Character Source = null;
        public Vector2 Source_Offset = Vector2.Zero; // The offset in which this 'projectile' spawns from its source origin
        public Vector2 Velocity = Vector2.Zero;
        public float Directional_Velocity = 0f; // If this attack can be fired in multiple directions, do this.

        public bool Follow_Source = false; // Whether or not this 'projectile' should follows its source movement
        public bool Inherit_Velocity = false; // Whether or not this attack should inherit its parents velocity
        public bool Loop = false;

        public int Hit_Frame = 0; // The animation frame that should generate the projectile / damage box
        public float Lifespan = 0f; // The duration this projectile should exist for

        public string Effect_Name = "none"; // The projectile effect to spawn
        public float Cooldown = 0f; // The cooldown for this particular attack
        public float Cooldown_timer = 0f; // The actual value of the cooldown timer

        public int Damage = 0; // The damage to deal
        public int Focus_Cost = 0; // The focus cost of this attack

        public float Knockback = 0f; // Knockback value 
        public Shared.Damage_Team Damage_Team = Shared.Damage_Team.NEUTRAL; // The team this projectile will inflict damage upon
        public Shared.Type Damage_Type = Shared.Type.MUNDANE; // The type of damage to inflict
    }
}
