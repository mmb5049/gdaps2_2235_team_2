using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team2_Mansion_Mayhem.Content.Sprites
{
    enum playerState
    {
        FaceLeft,
        WalkLeft,
        FaceRight,
        WalkRight,
    }
    internal class Player
    {
        private Texture2D spriteSheet;
        private Vector2 location;
        private playerState state;

        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame

        private int WalkFrameCount = 8;
        private int offSetY;
        private const int recWidth = 64;
        private const int recHeight = 53;

        // Constructor
        public Player(Texture2D spriteSheet, Vector2 location, playerState state)
        {
            this.spriteSheet = spriteSheet;
            this.location = location;
            this.state = state;

            fps = 10.0;
            timePerFrame = 1.0 / fps;
        }
        // Properties
        public playerState State
        {
            get { return state; }
            set { state = value; }
        }
        public void Draw(SpriteBatch sb)
        {
            DrawWalking(sb, SpriteEffects.None);
        }
        public void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

            
            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                    frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                // This keeps the time passed 
            }
        }
        private void DrawWalking(SpriteBatch sb, SpriteEffects flipSprite)
        {
            offSetY = 714;
            sb.Draw(
                spriteSheet,
                location,
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
    }
}
