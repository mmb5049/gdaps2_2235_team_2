﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Team2_Mansion_Mayhem
{
    /* a subclass of Enemy, a low level enemy 
   * whose main feature is the ability 
   * to pass through obstacles
   */
    internal class Ghost : Enemy
    {

        public Ghost(Rectangle position, int health, int damage, int speed) : base(position, health, damage, speed)
        {
            this.position = position;
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.speed = speed;
        }

        public override void Update()
        {
            // should move through walls without any updates
        }

        public override int Attack()
        {
            // will update when we have the player class
            return 0;
        }
    }
}
