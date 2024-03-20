using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography.X509Certificates;
using Team2_Mansion_Mayhem.Content.Sprites;
using System.Runtime.InteropServices;

namespace Team2_Mansion_Mayhem
{
    /// <summary>
    /// A subclass of Enemy. Monsters will chase the player and can enrage under a certain 
    /// health threshold, increasing attack power and speed.
    /// </summary>
    enum monsterState
    {
        FaceLeft,
        WalkLeft,
        FaceRight,
        WalkRight,
        AttackLeft, 
        AttackRight,
    }
    internal class Monster : Enemy
    {
        //rage stats
        private bool enraged;
        private double rageThreshold;
        private int ragePower;
        private Color rageColor;
        
        //states
        private monsterState state;
        private bool canDamage;
        private Rectangle attackRange = new Rectangle();

        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double attackTimeCounter;
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame
        private int attackFrame;
        private int frameCount;
        private int attackFrameCount;
        private int offSetY;
        private int recWidth = 64;
        private int recHeight = 53;
        private int xShift;
        private double timer;
        private double attackTimer;

        public Monster(Texture2D texture, Rectangle position, int health, int defense,int damage, int speed, monsterState state) 
            :base(texture, position,health, defense, damage, speed)
        {
            this.texture = texture;
            this.position = position;
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.speed = speed;
            this.enraged = false;
            this.spriteColor = Color.White;

            this.state = state;
            this.alive = true;

            //same for all Monsters.. change me if needed!
            this.rageThreshold = 0.5;
            this.ragePower = 2;
            this.rageColor = Color.PaleVioletRed;

            fps = 10.0;
            timePerFrame = 1.0 / fps;
        }

        // properties
        public int attackRangeX
        {
            get { return attackRange.X; }
        }
        
        public int attackRangeY
        {
            get { return attackRange.Y; }
        }

        //method
        public override void Update(GameTime gameTime, Player player)
        {
            if (alive == true)
            {
                UpdateAnimation(gameTime);
                attackTimer = 0.7;
                //enrage logic.. if at rage threshold and not enraged yet..

                if (((double)health / maxHealth <= rageThreshold) && !enraged)
                {
                    damage *= ragePower;
                    speed *= ragePower;
                    enraged = true;
                    spriteColor = rageColor;
                }

                attackRange.X = position.X - 10;
                attackRange.Y = position.Y - 10;
                attackRange.Width = position.Width - 5;
                attackRange.Height = position.Height - 5;

                switch (state)
                {
                    case monsterState.AttackLeft:
                        UpdateAttackAnimation(gameTime);
                        ProcessAttackLeft(gameTime, attackTimer, player);
                        break;

                    case monsterState.AttackRight:
                        UpdateAttackAnimation(gameTime);
                        ProcessAttackRight(gameTime, attackTimer, player);
                        break;
                }

                if (!alive)
                {
                    position.X = 0;
                    position.Y = 0;
                }
                Dead();
            }
           
        }
        public override void Draw(SpriteBatch sb, bool debugEnabled, SpriteFont debugFont)
        {
            if (alive == true)
            {
                switch(state) 
                {
                    case monsterState.WalkRight:
                        DrawWalking(sb, SpriteEffects.None); 
                        break;

                    case monsterState.WalkLeft:
                        DrawWalking(sb, SpriteEffects.FlipHorizontally);
                        break;

                    case monsterState.AttackLeft:
                        xShift = 50;
                        DrawAttackingLeft(sb, SpriteEffects.None);
                        break;

                    case monsterState.AttackRight:
                        xShift = 64;
                        DrawAttackingRight(sb, SpriteEffects.None);
                        break;
                }

                //draw stats under position in the event that debug is enabled
                if (debugEnabled)
                {
                    sb.DrawString(debugFont, DebugStats,
                    new Vector2(X, Y + position.Height), Color.Black);
                }
            }
        }

        public override void Chase(Rectangle playerPosition, int windowWidth, int windowHeight)
        {
            if (state != monsterState.AttackLeft && state != monsterState.AttackRight && alive == true) 
            {
                // Calculate direction towards the player by normalizing a vector
                float deltaX = playerPosition.X - position.X;
                float deltaY = playerPosition.Y - position.Y;
                float distance = (float)Math.Sqrt(Math.Pow(deltaX,2) + Math.Pow(deltaY,2));
                float directionX = deltaX / distance;
                float directionY = deltaY / distance;

                // Calculate the new position
                int newX = (int)(position.X + directionX * speed);
                int newY = (int)(position.Y + directionY * speed);

                // Check if the new position is within bounds
                if (newX >= 0 && newX + position.Width <= windowWidth &&
                    newY >= 0 && newY + position.Height <= windowHeight)
                {
                    // Update enemy position
                    position = new Rectangle(newX, newY, position.Width, position.Height);
                }
            }
        }
        public override int Attack()
        {
            //update me with like.. actual code later!
            // no need to use this yet
            return 0;
        }

        public void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing


            // How much time has passed 
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame > frameCount)     // Check the bounds 
                    frame = 1;

                timeCounter -= timePerFrame;    // Remove the time we "used" 
            }
        }

        public void UpdateAttackAnimation(GameTime gameTime) // different method to handle shoot animation
        {
            // Handle animation timing


            // How much time has passed 
            attackTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (attackTimeCounter >= timePerFrame)
            {
                attackFrame += 1;                     // Adjust the frame to the next image

                if (attackFrame > attackFrameCount)     // Check the bounds 
                    attackFrame = 1;

                attackTimeCounter -= timePerFrame;    // Remove the time we "used" 
            }
        }
        private void DrawWalking(SpriteBatch sb, SpriteEffects flipSprite)
        {
            // draw walking animation
            recWidth = 64;
            recHeight = 53;
            offSetY = 714;
            frameCount = 8;
            sb.Draw(
                texture,
                new Vector2((float)position.X, (float)position.Y),
                new Rectangle(
                    (frame * recWidth),
                    offSetY,
                    recWidth,
                    recHeight),
                spriteColor,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

        private void DrawAttackingRight(SpriteBatch sb, SpriteEffects flipSprite)
        {
            recWidth = 192;
            recHeight = 53;
            offSetY = 1994;

            attackFrameCount = 5;
            sb.Draw(
                texture,
                new Vector2((float)position.X, (float)position.Y),
                new Rectangle(
                    (attackFrame * recWidth) + xShift,
                    offSetY,
                    recWidth,
                    recHeight),
                spriteColor,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

        private void DrawAttackingLeft(SpriteBatch sb, SpriteEffects flipSprite)
        {
            recWidth = 192;
            recHeight = 53;
            offSetY = 1609;

            attackFrameCount = 5;
            sb.Draw(
                texture,
                new Vector2((float)position.X, (float)position.Y),
                new Rectangle(
                    (attackFrame * recWidth) + xShift,
                    offSetY,
                    recWidth,
                    recHeight),
                spriteColor,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

        public void ChangeState(GameTime gameTime, Rectangle playerLoc)
        {
            if (playerLoc.X > position.X)
            {
                state = monsterState.WalkRight;
            }
            else if (playerLoc.X < position.X)
            {
                state = monsterState.WalkLeft;
            }
        }

        public void StartAttack(Rectangle playerPosition)
        {
            if (attackRange.Intersects(playerPosition))
            {
                if (state == monsterState.WalkRight)
                {
                    state = monsterState.AttackRight;
                }

                if (state == monsterState.WalkLeft)
                {
                    state = monsterState.AttackLeft;
                }
            }
        }

        public void ProcessAttackRight(GameTime gameTime, double attackTimer , Player player)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= attackTimer)
            {
                state = monsterState.WalkRight;
                timer = 0;
                attackFrame = 0;
                canDamage = true;
            }
            
            if ((timer - 0.3) > 0.3 && canDamage)
            {
                if (attackRange.Intersects(player.Location))
                {
                    canDamage = false;
                    player.DamageTaken(damage);
                    
                    player.IsHurt = true;
                    
                }
            }
        }

        public void ProcessAttackLeft(GameTime gameTime, double attackTimer, Player player)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            
            if (timer >= attackTimer)
            {
                state = monsterState.WalkLeft;
                timer = 0;
                attackFrame = 0;
                canDamage = true;
            }
            
            if ((timer - 0.3) > 0.3 && canDamage)
            {
                if (attackRange.Intersects(player.Location))
                {
                    canDamage = false;
                    player.DamageTaken(damage);
                    
                    player.IsHurt = true;
                }
            }
            
        }

    }//end of class 
}
