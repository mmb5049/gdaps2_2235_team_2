using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team2_Mansion_Mayhem
{
    /// <summary>
    /// A subclass of Enemy. Monsters will chase the player and can enrage under a certain 
    /// health threshold, increasing attack power and speed.
    /// </summary>
    internal class Monster : Enemy
    {
        private bool enraged;
        private double rageThreshold;
        private int ragePower;

        public Monster(Vector2 position, int health, int damage, int speed) :base(position,health,damage,speed)
        {
            this.position = position;
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.speed = speed;
            this.enraged = false;

            //same for all Monsters.. change me if needed!
            this.rageThreshold = 0.5;
            this.ragePower = 2;
        }

        public override void Update()
        {
            //enrage logic.. if at rage threshold and not enraged yet..
            if (((health/maxHealth) <= rageThreshold) && !enraged)
            {
                damage *= ragePower;
                speed *= ragePower;
                enraged = true;
            }
        }

        public override int Attack()
        {
            //update me with like.. actual code later!
            return 0;
        }
    }//end of class 
}
