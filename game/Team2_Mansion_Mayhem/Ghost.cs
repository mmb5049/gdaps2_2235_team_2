using System;
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

        public Ghost(Texture2D texture, Rectangle position, int health, int defense, int damage, int speed) 
            :base(texture,position, health, defense, damage, speed)
        {
            this.texture = texture;
            this.position = position;
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.speed = speed;
        }

        public override void Update(GameTime gameTime)
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
