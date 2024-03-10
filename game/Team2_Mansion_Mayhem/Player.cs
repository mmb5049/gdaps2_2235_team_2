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
        private KeyboardState kbState;
        private KeyboardState preKbState;


        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame

        private int WalkFrameCount = 8;
        private int offSetY = 714;
        private const int recWidth = 64;
        private const int recHeight = 53;

        // Constructor
        public Player(Texture2D spriteSheet, Vector2 location, playerState state, KeyboardState kbState)
        {
            this.spriteSheet = spriteSheet;
            this.location = location;
            this.state = state;
            this.kbState = kbState;
            fps = 10.0;
            timePerFrame = 1.0 / fps;
        }
        // Properties
        public playerState State
        {
            get { return state; }
            set { state = value; }
        }
        public float X
        {
            get { return location.X; }
            set { location.X = value; }
        }
        public float Y
        {
            get { return location.Y; }
            set { location.Y = value; }
        }

        // method
        public void Update(GameTime gameTime)
        {
            kbState = Keyboard.GetState();
            switch (state)
            {
                case playerState.FaceLeft:
                    ProcessFaceLeft(kbState);
                    break;

                case playerState.FaceRight:
                    ProcessFaceRight(kbState);
                    break;

                case playerState.WalkLeft: 
                    ProcessWalkLeft(kbState);
                    location.X -= 2;
                    break;

                case playerState.WalkRight:
                    ProcessWalkRight(kbState);
                    location.X += 2;
                    break;
            }
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

                if (frame > WalkFrameCount)     // Check the bounds 
                    frame = 1;                  

                timeCounter -= timePerFrame;    // Remove the time we "used" 
            }
        }
        public void Draw(SpriteBatch sb) // draw appropriate sprite based on state
        {
            switch(state)
            {
                case playerState.FaceRight:
                    DrawStanding(sb, SpriteEffects.None); 
                    break;

                case playerState.FaceLeft:
                    DrawStanding(sb, SpriteEffects.FlipHorizontally);
                    break;

                case playerState.WalkRight:
                    DrawWalking(sb, SpriteEffects.None);
                    break;
                case playerState.WalkLeft:
                    DrawWalking(sb, SpriteEffects.FlipHorizontally);
                    break;
            }
        }
        private void DrawWalking(SpriteBatch sb, SpriteEffects flipSprite)
        {
            // draw walking animation
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
        private void DrawStanding(SpriteBatch sb, SpriteEffects flipSprite)
        {
            // draw standing sprite
            sb.Draw(
                spriteSheet,
                location,
                new Rectangle(
                    0,
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

        private void ProcessFaceLeft(KeyboardState keyState) // when player face left
        {
            if (keyState.IsKeyDown(Keys.D)) // face right
            {
                state = playerState.FaceRight;
            }

            if (keyState.IsKeyDown(Keys.A)) // walk left
            {
                state = playerState.WalkLeft;
            }
        }

        private void ProcessFaceRight(KeyboardState keyState) // when player face right
        {
            if (keyState.IsKeyDown(Keys.A)) // face right
            {
                state = playerState.FaceLeft;
            }

            if (keyState.IsKeyDown(Keys.D)) // walk left
            {
                state = playerState.WalkRight;
            }
        }

        private void ProcessWalkRight(KeyboardState keyState) // when player walk right
        {
            if (keyState.IsKeyUp(Keys.D)) // stop when D stop being pressed
            {
                state = playerState.FaceRight;
            }
        }

        private void ProcessWalkLeft(KeyboardState keyState) // when mario walk right
        {
            if (keyState.IsKeyUp(Keys.A)) // stop when A stop being pressed
            {
                state = playerState.FaceLeft;
            }
        }
    }
}
