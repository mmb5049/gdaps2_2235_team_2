using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Team2_Mansion_Mayhem
{
    enum projectileState 
    {
        FaceLeft, 
        FaceRight,
    }
    
    internal class Projectile : IDebug
    {
        // field
        private Texture2D projectileSheet;
        private Rectangle location;
        private int speed = 7;
        private double rotation;
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
        public Projectile(Texture2D projectileSheet, Rectangle location, double rotation, projectileState state, int windowWidth, int windowHeight)
        {
            this.projectileSheet = projectileSheet;
            this.location = location;
            this.rotation = rotation;
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

        public Rectangle Location
        {
            get { return location; }
        }
        public virtual string DebugStats
        {
            //return a list of stats to be printed 
            get
            {
                return
                 $"Speed: {speed}\n" +
                 $"Rotation: {rotation}";
            }
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

            //switch (state)
            {
                //case projectileState.FaceRight:
                    //location.X += speed;
                    //break;

                //case projectileState.FaceLeft:
                    //location.X -= speed;
                    //break;
            }

            location.X += (int)(speed * Math.Cos((double)rotation));
            location.Y += (int)(speed * Math.Sin((double)rotation));

            // deactivate when move out of window
            if (location.X >= windowWidth || location.X <= 0 || location.Y >= windowHeight || location.Y <= 0)
            {
                isActive = false;
            }
        }

        public void Draw(SpriteBatch sb, bool debugEnabled, SpriteFont debugFont)
        {
            if (isActive)
            {
               DrawFlying(sb, SpriteEffects.None); 

                //draw stats under position in the event that debug is enabled
                if (debugEnabled)
                {
                    sb.DrawString(debugFont, DebugStats,
                    new Vector2(location.X, location.Y + location.Height), Color.Black);
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
                (float)rotation,
                new Vector2(location.Width/2, location.Height/2),
                1.0f,
                flipSprite,
                0);
        }

        public void CheckCollision(Enemy enemy, int damage) // check if projectile hit enemy
        {
            if (location.Intersects(enemy.Position)) 
            {
                isActive = false;
                enemy.DamageTaken(damage);
            }
        }

    }
}
