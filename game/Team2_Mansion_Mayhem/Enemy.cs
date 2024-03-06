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
    internal abstract class Enemy
    {
        // fields
        private Texture2D texture;
        private Vector2 position;
        private int health;
        private int defense;
        private int damage;
        private int speed;
        private bool alive = true;

        // constructor
        public Enemy(Vector2 position, int health, int damage, int speed)
        {
            this.position = position;
            this.health = health;
            this.damage = damage;
            this.speed = speed;
        }

        // properties
        public int Health
        {
            get { return health; }
        }
        public int Defense
        {
            get { return defense; }
        }
        public int Damage
        {
            get { return damage; }
        }
        public bool Alive
        {
            get { return alive; }
        }
        public float X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }
        public float Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }

        //method
        public abstract void Update();

        public abstract int Attack();
        public virtual void DamageTaken(int damage)
        {
            int damageTaken = damage - defense;
            if (damageTaken < 0) // avoid taking negative damage
            {
                damageTaken = 0;
            }
            health -= damageTaken;
        }

        public virtual void Chase(Vector2 playerPosition)
        {
            // Calculate direction towards the player
            Vector2 direction = Vector2.Normalize(playerPosition - position);

            // Update enemy position
            position += direction * speed;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (alive)
            {
                sb.Draw(texture, position, Color.White);
            }
            
        }

        public virtual void Dead()
        {
            if(health <= 0)
            {
                alive = false;
            }
        }

        
    }
}
