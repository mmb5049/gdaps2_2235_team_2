using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Team2_Mansion_Mayhem.Content.Sprites;

namespace Team2_Mansion_Mayhem
{
    /// <summary>
    /// Enemies serve as the main set of antagonists and are encountered the most commonly.
    /// </summary>
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
        protected bool alive;
        protected Color spriteColor;

        // constructor
        public Enemy(Texture2D texture, Rectangle position, int health, int defense, int damage, int speed)
        {
            this.texture = texture;
            this.position = position;
            this.defense = defense;
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.speed = speed;
            alive = true;
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
        public int Speed
        {
            get { return speed; }
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
                 $"Speed: {speed}\n" +
                 $"Position: ({X}, {Y})";
            }
        }
        //method
        public abstract void Update(GameTime gameTime, Player player, List<Obstacle> obstacles);

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

        public virtual void Chase(Rectangle playerPosition, int windowWidth, int windowHeight, List<Obstacle> obstacles)
        {
            // Calculate direction towards the player
            float deltaX = playerPosition.X - Position.X;
            float deltaY = playerPosition.Y - Position.Y;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            float directionX = deltaX / distance;
            float directionY = deltaY / distance;

            // Update enemy position if not colliding with player
            Rectangle newPosition = new Rectangle(
                Position.X + (int)(directionX * speed),
                Position.Y + (int)(directionY * speed),
                Position.Width,
                Position.Height);

            if (!newPosition.Intersects(playerPosition))
            {
                position = newPosition;
            }
        }

        
        public virtual void Draw(SpriteBatch sb, bool debugEnabled, SpriteFont debugFont)
        {
            if (alive)
            {
                sb.Draw(texture, position, Color.White);
            }
            
        }

        public virtual void Dead()
        {
            if(health < 1)
            {
                alive = false;
            }
        }

        
    }//end of class
}
