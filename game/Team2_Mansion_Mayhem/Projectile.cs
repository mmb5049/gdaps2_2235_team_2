using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team2_Mansion_Mayhem
{
    enum projectileState
    {
        FaceLeft, 
        FaceRight,
    }

    internal class Projectile
    {
        // field
        private Texture2D projectileSheet;
        private Rectangle location;
        private int speed = 3;
        private bool isActive;
        private projectileState state;
        private int windowWidth;
        private int windowHeight;

        // animation field
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame
        private int frameCount;
        private int offSetY = 1;
        private const int recWidth = 20;
        private const int recHeight = 18;


        // constructor
        public Projectile(Texture2D projectileSheet, Rectangle location, projectileState state, int windowWidth, int windowHeight)
        {
            this.projectileSheet = projectileSheet;
            this.location = location;
            this.state = state;

            isActive = true;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            fps = 10.0;
            timePerFrame = 1.0 / fps;
        }

        // properties
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        // method
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

        public void Update(GameTime gameTime)
        {
            UpdateAnimation(gameTime);

            switch (state)
            {
                case projectileState.FaceRight:
                    location.X += speed;
                    break;

                case projectileState.FaceLeft:
                    location.X -= speed;
                    break;
            }

            // deactivate when move out of window
            if (location.X >= windowWidth || location.X <= 0 || location.Y >= windowHeight || location.Y <= 0)
            {
                isActive = false;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (isActive)
            {
                switch (state)
                {
                    case projectileState.FaceRight:
                        DrawFlying(sb, SpriteEffects.None); 
                        break;

                    case projectileState.FaceLeft:
                        DrawFlying(sb, SpriteEffects.FlipHorizontally);
                        break;
                }
            }
        }
        public void DrawFlying(SpriteBatch sb, SpriteEffects flipSprite)
        {
            frameCount = 3;
            sb.Draw(
                projectileSheet,
                new Vector2((float)location.X,(float)location.Y),
                new Rectangle(
                    5 + (frame * recWidth),
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

        public bool CheckCollision(Enemy enemy) // check if projectile hit enemy
        {
            if (location.Intersects(enemy.Position)) 
            {
                isActive = false;
                return true;
            }
            return false;
        }

    }
}
