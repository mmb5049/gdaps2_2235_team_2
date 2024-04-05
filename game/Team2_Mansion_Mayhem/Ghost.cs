﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
        private int currentFrame;
        private float animationTimer;
        private GhostState currentState;

        private const int numberOfWalkingFrames = 4;
        private const int numberOfHurtFrames = 4;
        private const int numberOfDeathFrames = 8;
        private const float animationSpeed = 0.2f;

        public Ghost(Texture2D texture, Rectangle position, int health, int defense, int damage, int speed) 
            : base(texture, position, health, defense, damage, speed)
        {
            this.texture = texture;
            this.position = position;
            this.maxHealth = health;
            this.health = health;
            this.damage = damage;
            this.speed = speed;

            currentFrame = 0;
            animationTimer = 0f;
            currentState = GhostState.Normal;
        }

        public override void Update(GameTime gameTime, Player player)
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
        }

        public override void Draw(SpriteBatch spriteBatch, bool debugEnabled, SpriteFont debugFont)
        {
            // Draw the Ghost animation
            DrawGhostAnimation(spriteBatch, SpriteEffects.None);

            // Draw stats under position in the event that debug is enabled
            if (debugEnabled)
            {
                spriteBatch.DrawString(debugFont, DebugStats,
                new Vector2(X, Y + position.Height), Color.Black);
            }
        }

        private void UpdateNormalState(GameTime gameTime)
        {
            // Update walking animation
            UpdateAnimation(gameTime, numberOfWalkingFrames);

            // Chase the player disregarding collision detections of obstacles

            // When damage is taken, go to hurt state

            // When health is 0 or less, transition to dying state
            if (health <= 0)
            {
                currentState = GhostState.Dying;
            }
        }

        private void UpdateHurtState(GameTime gameTime)
        {
            // Update hurt animation
            UpdateAnimation(gameTime, numberOfHurtFrames);

            // Ghost becomes invulnerable and speed decreases during hurt animation

            // Transition back to normal state after hurt animation is done
            if (currentFrame == numberOfHurtFrames - 1)
            {
                currentState = GhostState.Normal;
            }
        }

        private void UpdateDyingState(GameTime gameTime)
        {
            // Update death animation
            UpdateAnimation(gameTime, numberOfDeathFrames);

            // Stop moving and attacking in dying state
            Dead();
            speed = 0;
            damage = 0;

            // When the death animation is finished, remove the ghost
            if (currentFrame == numberOfDeathFrames - 1)
            {
                // Logic to remove from the game
            }
        }

        private void DrawGhostAnimation(SpriteBatch spriteBatch, SpriteEffects flipSprite)
        {
            int recWidth = 32;
            int recHeight = 32;
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
                currentFrame * recWidth,
                offsetY,
                recWidth,
                recHeight);

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
                    currentFrame = 0; // Reset frame if it exceeds total frames (loop animation)
                }
                animationTimer -= animationSpeed;
            }
        }

        public override void DamageTaken(int damage)
        {
            base.DamageTaken(damage);

            // Transition to Hurt state
            currentState = GhostState.Hurt;
            currentFrame = 0; // Reset animation frame
        }

        public override int Attack()
        {
            // Ghost can't attack when in hurt or dying state
            if (currentState == GhostState.Normal)
            {
                // Implement attack logic here
                return damage;
            }

            // Update when we have the player class
            return 0;
        }
    }
}
