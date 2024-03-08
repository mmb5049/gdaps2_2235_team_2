using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Team2_Mansion_Mayhem
{
    /* a subclass of Enemy, a low level enemy 
     * whose main feature is the ability 
     * to pass through obstacles
     */
    internal class Ghost: Enemy
    {
        
        public Ghost(Vector2 position, int health, int damage, int speed):base(position, health, damage, speed)
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
