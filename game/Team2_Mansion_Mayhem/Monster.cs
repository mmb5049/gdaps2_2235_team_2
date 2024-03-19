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
        private bool enraged;
        private double rageThreshold;
        private int ragePower;
        private monsterState state;

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
            this.state = state;
            //same for all Monsters.. change me if needed!
            this.rageThreshold = 0.5;
            this.ragePower = 2;

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
        public override void Update(GameTime gameTime)
        {
            UpdateAnimation(gameTime);
            attackTimer = 0.7;
            //enrage logic.. if at rage threshold and not enraged yet..
            if (((health/maxHealth) <= rageThreshold) && !enraged)
            {
                damage *= ragePower;
                speed *= ragePower;
                enraged = true;
            }
            attackRange.X = position.X - 10;
            attackRange.Y = position.Y - 10;
            attackRange.Width = position.Width - 5 ;
            attackRange.Height = position.Height - 5;

            switch (state)
            {
                case monsterState.AttackLeft:
                    UpdateAttackAnimation(gameTime);
                    ProcessAttackLeft(gameTime, attackTimer);
                    break;

                case monsterState.AttackRight:
                    UpdateAttackAnimation(gameTime);
                    ProcessAttackRight(gameTime, attackTimer); 
                    break;
            }

            Dead();
        }
        public override void Draw(SpriteBatch sb)
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
            }
        }

        public override void Chase(Rectangle playerPosition, int windowWidth, int windowHeight)
        {
            if (state != monsterState.AttackLeft && state != monsterState.AttackRight) 
            {
                // Calculate direction towards the player
                float deltaX = playerPosition.X - Position.X;
                float deltaY = playerPosition.Y - Position.Y;
                float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                float directionX = deltaX / distance;
                float directionY = deltaY / distance;

                // Update enemy position if not colliding with player
                /*Rectangle newPosition = new Rectangle(
                    Position.X + (int)(directionX * speed),
                    Position.Y + (int)(directionY * speed),
                    Position.Width,
                    Position.Height);

                if (!newPosition.Intersects(playerPosition))
                {
                    position = newPosition;
                }*/

                // Calculate the new position
                int newX = (int)(Position.X + directionX * Speed);
                int newY = (int)(Position.Y + directionY * Speed);

                // Check if the new position is within bounds
                if (newX >= 0 && newX + Position.Width <= windowWidth &&
                    newY >= 0 && newY + Position.Height <= windowHeight)
                {
                    // Update enemy position
                    position = new Rectangle(newX, newY, Position.Width, Position.Height);
                }
            }
        }
        public override int Attack()
        {
            //update me with like.. actual code later!
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
                Color.White,
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
                Color.White,
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
                Color.White,
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

        public void ProcessAttackRight(GameTime gameTime, double attackTimer)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= attackTimer)
            {
                state = monsterState.WalkRight;
                timer = 0;
                attackFrame = 0;
            }
        }

        public void ProcessAttackLeft(GameTime gameTime, double attackTimer)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= attackTimer)
            {
                state = monsterState.WalkLeft;
                timer = 0;
                attackFrame = 0;
            }
        }

    }//end of class 
}
