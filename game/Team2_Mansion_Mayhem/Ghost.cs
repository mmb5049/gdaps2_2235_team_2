using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Team2_Mansion_Mayhem.Content.Sprites;


namespace Team2_Mansion_Mayhem
{
    /*  a subclass of Enemy, a low level enemy 
        whose main feature is the ability 
        to pass through obstacles               */

    enum GhostState
    {
        Normal,
        Hurt,
        Dying
    }

    internal class Ghost : Enemy, IDebug
    {
        private GhostState currentState;
        private bool damageTaken;
        private bool invulnerable;
        private bool hurtAnimationCompleted;
        private bool dyingAnimationCompleted;

        //how long to wait until the enemy can attack again
        private double damageCooldown = 0.25;
        private double timeUntilAttack;
        //how long to wait after being attacked until it can be attacked again
        private double invulnerableTime = 3;
        private double timeSinceDamaged;
        //prevent loss of base speed by using division by ints
        private int baseSpeed;

        private int currentFrame;
        private float animationTimer;
        private const int numberOfWalkingFrames = 4;
        private const int numberOfHurtFrames = 4;
        private const int numberOfDeathFrames = 8;
        private const float animationSpeed = 0.2f;

        public override string DebugStats
        {
            //return a list of stats to be printed 
            get
            {
                return
                 $"State: {currentState}\n" +
                 $"Health: {health}/{maxHealth} \n " +
                 $"Defense: {defense} \n " +
                 $"Damage: {damage}\n " +
                 $"Position: ({X}, {Y})\n" +
                 $"Invulnerability: {invulnerable}";
            }
        }

        public Ghost(Texture2D texture, Rectangle position, int health, int defense, int damage, int speed) 
            : base(texture, position, health, defense, damage, speed)
        {
            this.texture = texture;
            this.position = position;
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.baseSpeed = speed;
            this.speed = speed;
            this.spriteColor = Color.White;

            currentState = GhostState.Normal;
            currentFrame = 0;
            animationTimer = 0f;
            damageTaken = false;
            invulnerable = false;
            hurtAnimationCompleted = false;
            dyingAnimationCompleted = false;
        }

        public override void Update(GameTime gameTime, Player player, List<Obstacle> obstacles)
        {
            // Update based on the current state
            switch (currentState)
            {
                case GhostState.Normal:
                    UpdateNormalState(gameTime);
                    break;

                case GhostState.Hurt:
                    UpdateHurtState(gameTime);
                    break;

                case GhostState.Dying:
                    UpdateDyingState(gameTime);
                    break;
            }

            // Check collision with player
            if (alive && player.Alive && position.Intersects(player.Location) && currentState != GhostState.Dying && timeUntilAttack == 0)
            {
                player.DamageTaken(damage); // Apply damage to player
                player.IsHurt = true;
                timeUntilAttack = damageCooldown;
            }

            timeSinceDamaged = Math.Max(timeSinceDamaged - gameTime.ElapsedGameTime.TotalSeconds, 0);
            timeUntilAttack = Math.Max(timeUntilAttack - gameTime.ElapsedGameTime.TotalSeconds, 0);
            
            if (timeSinceDamaged == 0 && invulnerable == true)
            {
                invulnerable = false;
                spriteColor = Color.White * 1f;
                speed = baseSpeed;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, bool debugEnabled, SpriteFont debugFont)
        {
            // Draw the Ghost animation
            DrawGhostAnimation(spriteBatch, SpriteEffects.None);

            // Draw stats under position in the event that debug is enabled
            if (debugEnabled)
            {
                spriteBatch.DrawString(debugFont, DebugStats, new Vector2(X, Y + position.Height), Color.Black);
            }
        }

        private void UpdateNormalState(GameTime gameTime)
        {
            // Update walking animation
            UpdateAnimation(gameTime, numberOfWalkingFrames);

            // If damage is taken and health > 0, transition to Hurt state
            if (damageTaken)
            {
                if (health > 0)
                {
                    currentState = GhostState.Hurt;
                    currentFrame = 0;
                    hurtAnimationCompleted = false;
                }
                damageTaken = false;
            }

            // When health reaches 0, transition to Dying state
            if (health < 1)
            {
                currentState = GhostState.Dying;
                currentFrame = 0;
                dyingAnimationCompleted = false;
            }
        }

        private void UpdateHurtState(GameTime gameTime)
        {
            // Update hurt animation
            UpdateAnimation(gameTime, numberOfHurtFrames);

            if (!hurtAnimationCompleted && currentFrame == numberOfHurtFrames - 1)
            {
                hurtAnimationCompleted = true;
            }

            // Transition back to normal state after hurt animation is done
            if (hurtAnimationCompleted && currentFrame == 0)
            {
                currentState = GhostState.Normal;
            }
        }

        private void UpdateDyingState(GameTime gameTime)
        {
            // Update death animation
            UpdateAnimation(gameTime, numberOfDeathFrames);

            // Stop moving and attacking in dying state
            if (!dyingAnimationCompleted && currentFrame == numberOfDeathFrames - 1)
            {
                Dead();
                speed = 0;
                damage = 0;
                dyingAnimationCompleted = true;
            }
        }

        private void DrawGhostAnimation(SpriteBatch spriteBatch, SpriteEffects flipSprite)
        {
            int recSize = 32;
            int offsetY = 0;
            int frameCount = 0;

            // Draw Ghost animation based on current state
            switch (currentState)
            {
                case GhostState.Normal:
                    offsetY = 0;
                    frameCount = numberOfWalkingFrames;
                    break;

                case GhostState.Hurt:
                    offsetY = 32;
                    frameCount = numberOfHurtFrames;
                    break;

                case GhostState.Dying:
                    offsetY = 64;
                    frameCount = numberOfDeathFrames;
                    break;
            }

            // Current frame
            Rectangle sourceRect = new Rectangle(
                currentFrame * recSize,
                offsetY,
                recSize,
                recSize);

            // Draw the frame of the animation
            spriteBatch.Draw(
                texture,
                new Vector2((float)position.X, (float)position.Y),
                sourceRect,
                spriteColor,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

        private void UpdateAnimation(GameTime gameTime, int totalFrames)
        {
            // Update animation timer
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed, go to the next frame
            if (animationTimer > animationSpeed)
            {
                currentFrame++;
                if (currentFrame >= totalFrames)
                {
                    currentFrame = 0; // Reset frame if it exceeds total frames
                }
                animationTimer -= animationSpeed;
            }
        }

        public override void DamageTaken(int damage)
        {
            // Only apply damage if the ghost is not invulnerable
            if (!invulnerable && timeSinceDamaged == 0)
            {
                base.DamageTaken(damage);

                invulnerable = true;
                speed /= 2; // Speed decreases in half
                
                spriteColor = Color.White * 0.5f; //ghost becomes translucent
                timeSinceDamaged = invulnerableTime;

                damageTaken = true;
            }
        }

        public override void Chase(Rectangle playerPosition, int windowWidth, int windowHeight, List<Obstacle> obstacles)
        {
            if (currentState == GhostState.Normal || currentState == GhostState.Hurt && alive == true)
            {
                // Calculate direction towards the player
                float deltaX = playerPosition.X - position.X;
                float deltaY = playerPosition.Center.Y - position.Center.Y;
                float distance = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                float directionX = (float)deltaX / distance;
                float directionY = (float)deltaY / distance;

                // Calculate the new position
                int newX = (int)(position.X + directionX * speed);
                int newY = (int)(position.Y + directionY * speed);

                // Check if the new position is within bounds and update position
                if (newX >= 0 && newX + position.Width <= windowWidth &&
                    newY >= 0 && newY + position.Height <= windowHeight)
                {
                    position = new Rectangle(newX, newY, position.Width, position.Height);
                }
            }
        }

        public override int Attack()
        {
            // Update later
            return 0;
        }
    }
}
