using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team2_Mansion_Mayhem
{
    internal abstract class Enemy: IDebug
    {
        // fields
        protected Texture2D texture;
        protected Rectangle position;
        protected int health;
        protected int maxHealth;
        protected int defense;
        protected int damage;
        protected int speed;
        protected bool alive = true;

        // constructor
        public Enemy(Rectangle position, int health, int damage, int speed)
        {
            this.position = position;
            this.maxHealth = health;
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
        public int X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }
        public int Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }

        public Rectangle Position
        {
            get { return position; }
        }
        public virtual string DebugStats
        {
            //return a list of stats to be printed 
            get
            {
                return
                 $"Health: {health}/{maxHealth} \n " +
                 $"Defense: {defense} \n " +
                 $"Damage: {damage}\n " +
                 $"Position: ({X}, {Y})";
            }
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
            // find direction
            Vector2 direction = new Vector2(playerPosition.X - position.X, playerPosition.Y - position.Y);

            // Update enemy position
            position.X += (int)direction.X * speed;
            position.Y += (int)direction.Y * speed;
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

        
    }//end of class
}
