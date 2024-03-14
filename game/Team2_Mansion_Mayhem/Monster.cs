using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
    }
    internal class Monster : Enemy
    {
        private bool enraged;
        private double rageThreshold;
        private int ragePower;
        private monsterState state;
        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double shootTimeCounter;
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame
        private int shootFrame;
        private int frameCount;
        private int offSetY;
        private const int recWidth = 64;
        private const int recHeight = 53;

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

        public override void Update(GameTime gameTime)
        {
            UpdateAnimation(gameTime);
            //enrage logic.. if at rage threshold and not enraged yet..
            if (((health/maxHealth) <= rageThreshold) && !enraged)
            {
                damage *= ragePower;
                speed *= ragePower;
                enraged = true;
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            if (alive)
            {
                switch(state) 
                {
                    case monsterState.WalkRight:
                        DrawWalking(sb, SpriteEffects.None); break; 
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

        private void DrawWalking(SpriteBatch sb, SpriteEffects flipSprite)
        {
            // draw walking animation
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
    }//end of class 
}
