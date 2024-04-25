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
using System.IO;

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
        Dying
    }
    internal class Monster : Enemy
    {
        // Rage stats
        private bool enraged;
        private double rageThreshold;
        private int ragePower;
        private Color rageColor;
        
        private monsterState state;
        private bool canDamage;
        private Rectangle attackRange = new Rectangle();

        // Animation
        private int frame;              // Current animation frame
        private double timeCounter;     // Amount of time that has passed
        private double attackTimeCounter;
        private double fps;             // Speed of the animation
        private double timePerFrame;    // Amount of time (in fractional seconds) per frame
        private int attackFrame;
        private int dyingFrame = 0;
        private int frameCount;
        private int attackFrameCount;
        private int dyingFrameCount = 6;
        private bool dyingAnimationCompleted = false;
        private int offSetY;
        private int recWidth = 64;
        private int recHeight = 53;
        private int xShift;
        private double timer;
        private double attackTimer;
        private double attackEventFrame = 4;

        public Monster(Texture2D texture, Rectangle position, int health, int defense,int damage, int speed, monsterState state) 
            :base(texture, position,health, defense, damage, speed)
        {
            this.texture = texture;
            this.position = position;
            this.obstacleBounds = new Rectangle(position.X, position.Y + position.Height / 2, position.Width, position.Height / 2 - 4);
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.speed = speed;
            this.enraged = false;
            this.spriteColor = Color.White;

            this.state = state;
            this.alive = true;

            // Same for all Monsters, change if needed
            this.rageThreshold = 0.5;
            this.ragePower = 2;
            this.rageColor = Color.PaleVioletRed;

            fps = 10.0;
            timePerFrame = 1.0 / fps;
        }

        public int attackRangeX
        {
            get { return attackRange.X; }
        }
        
        public int attackRangeY
        {
            get { return attackRange.Y; }
        }

        public override void Update(GameTime gameTime, Player player, List<Obstacle> obstacles)
        {
            if (health > 0)
            {
                UpdateAnimation(gameTime);
                attackTimer = 0.7;
                
                // Enrage logic, if at rage threshold and not enraged yet
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
            }
            else
            {
                state = monsterState.Dying;
                UpdateDyingState(gameTime);
            }
        }

        public override void Draw(SpriteBatch sb, bool debugEnabled, SpriteFont debugFont)
        {
            switch (state)
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

                case monsterState.Dying:
                    DrawDying(sb, SpriteEffects.None);
                    break;
            }

            // Draw stats under position in the event that debug is enabled
            if (debugEnabled)
            {
                sb.DrawString(debugFont, DebugStats,
                new Vector2(X, Y + position.Height), Color.Black);
            }
        }

        public override void Chase(Rectangle playerPosition, int windowWidth, int windowHeight, List<Obstacle> obstacles)
        {
            if (state != monsterState.AttackLeft || state != monsterState.AttackRight || state != monsterState.Dying && alive == true) 
            {
                float deltaX = playerPosition.X - position.X;
                float deltaY = playerPosition.Center.Y - position.Center.Y;

                float distance = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                float directionX = (float)deltaX / distance;
                float directionY = (float)deltaY / distance;

                // Calculate the new position
                int newX = (int)(position.X + directionX * speed);
                int newY = (int)(position.Y + directionY * speed);

                Rectangle newMonsterBoundsX = new Rectangle(newX, position.Y + obstacleBounds.Height, obstacleBounds.Width, obstacleBounds.Height);
                Rectangle newMonsterBoundsY = new Rectangle(position.X, newY + obstacleBounds.Height, obstacleBounds.Width, obstacleBounds.Height);
                
                // Check for collision with each obstacle
                foreach (Obstacle obstacle in obstacles)
                {
                    if (newMonsterBoundsX.Intersects(obstacle.Position))
                    {
                        newX = position.X;
                    }
                    if (newMonsterBoundsY.Intersects(obstacle.Position))
                    {
                        newY = position.Y;
                    }
                }

                // Check if the new position is within bounds
                if (newX >= 0 && newX + position.Width <= windowWidth && newY >= 0 && newY + position.Height <= windowHeight)
                {
                    // Update enemy position
                    position = new Rectangle(newX, newY, position.Width, position.Height);
                }
            }
        }
        public override int Attack()
        {
            // No need to use this yet
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
                frame += 1; // Adjust the frame to the next image

                if (frame > frameCount) // Check the bounds 
                    frame = 1;

                timeCounter -= timePerFrame; // Remove the time we "used" 
            }
        }

        public void UpdateAttackAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // How much time has passed 
            attackTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (attackTimeCounter >= timePerFrame)
            {
                attackFrame += 1; // Adjust the frame to the next image

                if (attackFrame > attackFrameCount) // Check the bounds 
                    attackFrame = 1;

                attackTimeCounter -= timePerFrame; // Remove the time we "used" 
            }
        }

        private void UpdateDyingAnimation(GameTime gameTime, int totalFrames)
        {
            // Handle animation timing for dying animation
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeCounter > timePerFrame)
            {
                dyingFrame++;

                if (dyingFrame >= totalFrames)
                {
                    dyingFrame = 0;
                }
                timeCounter -= timePerFrame;
            }
        }

        private void DrawWalking(SpriteBatch sb, SpriteEffects flipSprite)
        {
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

        private void DrawDying(SpriteBatch sb, SpriteEffects flipSprite)
        {
            recWidth = 66;
            recHeight = 53;
            offSetY = 1285;

            sb.Draw(
                texture,
                new Vector2((float)position.X, (float)position.Y),
                new Rectangle(
                    (dyingFrame * recWidth),
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
            
            if ((attackFrame == attackEventFrame) && canDamage)
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

        private void UpdateDyingState(GameTime gameTime)
        {
            // Update dying animation frames
            UpdateDyingAnimation(gameTime, dyingFrameCount);

            // Stop movement and attacks during dying animation
            speed = 0;
            damage = 0;

            // Check if dying animation completes
            if (!dyingAnimationCompleted && dyingFrame == dyingFrameCount - 1)
            {
                Dead();
                dyingAnimationCompleted = true;
            }
        }
    }
}
