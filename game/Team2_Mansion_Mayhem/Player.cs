using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
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
        ShootingLeft,
        ShootingRight,
    }

    internal class Player : IDebug
    {
        // fields
        private Texture2D spriteSheet;
        private Rectangle location;
        private Vector2 resetPoint;
        private playerState state;
        private bool isHurt;
        private KeyboardState kbState;
        private KeyboardState preKbState;
        private int speed;
        private int windowWidth;
        private int windowHeight;
        private List <Projectile> projectiles = new List<Projectile>();

        // stats
        private int health;
        private int maxHealth;
        private int defense;
        private int damage;
        private bool alive = true;
        private int damageIntake = 0;
        // projectile
        private Texture2D projectileSheet;
        private Rectangle projectileLoc;

        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double shootTimeCounter;
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame
        private int shootFrame;
        private int frameCount;
        private int offSetY;
        private int recWidth = 64;
        private int recHeight = 53;
        private int xShift = 0;
        private Color color;
        // timing
        private double timer = 0;
        private double shootTimer;
        private double hurtTimer = 0;
        // Constructor
        public Player(Texture2D spriteSheet, Rectangle location, int health, int defense, int damage, int speed 
            ,playerState state, KeyboardState kbState, Texture2D projectileSheet, int windowWidth, int windowHeight)
        {
            this.health = health;
            this.maxHealth = health;
            this.defense = defense;
            this.damage = damage;   
            this.speed = speed;
            this.spriteSheet = spriteSheet;
            this.location = location;

            this.resetPoint.X = location.X;
            this.resetPoint.Y = location.Y;

            this.state = state;
            this.kbState = kbState;
            fps = 10.0;
            timePerFrame = 1.0 / fps;
            this.projectileSheet = projectileSheet;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }
        // Properties
        public playerState State
        {
            get { return state; }
            set { state = value; }
        }
        public int X
        {
            get { return location.X; }
            set { location.X = value; }
        }
        public int Y
        {
            get { return location.Y; }
            set { location.Y = value; }
        }

        public double Timer
        {
            get { return timer; }
            set { timer = value; }
        }
        public double ShootTimer
        {
            get { return shootTimer; }
            set { shootTimer = value; } 
        }
        public int Count
        {
            get { return projectiles.Count; }
        }

        public Rectangle Location
        {
            get { return location; }
        }
        
        public List<Projectile> Projectiles
        {
            get { return projectiles; }
        }

        public int Damage
        {
            get { return damage; }
        }

        public bool IsHurt
        {
            get { return isHurt; }
            set { isHurt = value; }
        }
        public int Defense
        {
            get { return defense; }
        }
        public int Health
        {
            get { return health; }
        }
        public int DamageIntake
        {
            get { return damageIntake; }
        }
        public bool Alive
        {
            get { return alive; } 
        }

        public virtual string DebugStats
        {
            //return a list of stats to be printed 
            get
            {
                return
                 $"Health: {health}/{maxHealth} \n " +
                 $"Defense: {defense} \n " +
                 $"Damage: {damage}\n " +
                 $"Position: ({X}, {Y})";
            }
        }

        // method

        public void Reset()
        {
            health = maxHealth;
            location.X = (int)resetPoint.X;
            location.Y = (int)resetPoint.Y;
            isHurt = false;
            alive = true;
        }
        public void Update(GameTime gameTime)
        {
            kbState = Keyboard.GetState();
            shootTimer = .7;
            UpdateAnimation(gameTime);
            if (health < 1)
            {
                alive = false;
            }
            if (projectiles != null) // update each projectile
            {
                foreach (Projectile projectile in projectiles)
                {
                    projectile.Update(gameTime);
                }
            }

            if (projectiles.Count > 0)
            {
                for (int i = projectiles.Count - 1; i >= 0; i--) // remove projectiles when not active
                {
                    if (projectiles[i].IsActive != true)
                    {
                        projectiles.RemoveAt(i);
                    }
                }
            }
            
            if (isHurt)
            {
                ProcessGetHurt(gameTime);
            }

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
                    
                    break;

                case playerState.WalkRight:
                    ProcessWalkRight(kbState);
                    
                    break;

                case playerState.ShootingRight:
                    UpdateShootAnimation(gameTime);
                    ProcessShootRight(kbState, gameTime, shootTimer);
                    break;

                case playerState.ShootingLeft:
                    UpdateShootAnimation(gameTime);
                    ProcessShootLeft(kbState, gameTime, shootTimer);
                    break;
            }
        }

        public void DamageTaken(int damage)
        {
            damageIntake = damage - defense;

            if (damageIntake < 0) // avoid taking negative damage
            {
                damageIntake = 0;
            }

            health -= damageIntake;
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
        public void UpdateShootAnimation(GameTime gameTime) // different method to handle shoot animation
        {
            // Handle animation timing


            // How much time has passed 
            shootTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (shootTimeCounter >= timePerFrame)
            {
                shootFrame += 1;                     // Adjust the frame to the next image

                if (shootFrame > frameCount)     // Check the bounds 
                    shootFrame = 1;

                shootTimeCounter -= timePerFrame;    // Remove the time we "used" 
            }
        }
        public void Draw(SpriteBatch sb, bool debugEnabled, SpriteFont debugFont) // draw appropriate sprite based on state
        {
            switch (state)
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

                case playerState.ShootingRight:
                    xShift = 64;
                    DrawShooting(sb, SpriteEffects.None);
                    break;

                case playerState.ShootingLeft:
                    xShift = -64;
                    DrawShooting(sb, SpriteEffects .FlipHorizontally);
                    break;
            }

            if (projectiles != null)
            {
                foreach (Projectile projectile in projectiles)
                {
                    projectile.Draw(sb);
                }
            }
            //draw stats under position in the event that debug is enabled
            if (debugEnabled)
            {
                sb.DrawString(debugFont, DebugStats,
                new Vector2(X, Y + location.Height), Color.Black);
            }
        }
        private void DrawWalking(SpriteBatch sb, SpriteEffects flipSprite)
        {
            // draw walking animation
            recWidth = 64;
            recHeight = 53;
            offSetY = 714;
            frameCount = 8;
            if (isHurt != true)
            {
                color = Color.White;
            }
            else
            {
                color = Color.Red;
            }
            sb.Draw(
                spriteSheet,
                new Vector2((float)location.X,(float)location.Y),
                new Rectangle(
                    (frame * recWidth),
                    offSetY,
                    recWidth,
                    recHeight),
                color,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }
        private void DrawStanding(SpriteBatch sb, SpriteEffects flipSprite)
        {
            // draw standing sprite
            recWidth = 64;
            recHeight = 53;
            offSetY = 714;
            if (isHurt != true)
            {
                color = Color.White;
            }
            else
            {
                color = Color.Red;
            }
            sb.Draw(
                spriteSheet,
                new Vector2((float)location.X, (float)location.Y),
                new Rectangle(
                    0,
                    offSetY,
                    recWidth,
                    recHeight),
                color,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }
        private void DrawShooting(SpriteBatch sb, SpriteEffects flipSprite)
        {
            recWidth = 192;
            recHeight = 53;
            offSetY = 1994;
            frameCount = 7;

            if (isHurt != true)
            {
                color = Color.White;
            }
            else
            {
                color = Color.Red;
            }

            sb.Draw(
                spriteSheet,
                new Vector2((float)location.X, (float)location.Y),
                new Rectangle(
                    (shootFrame * recWidth) + xShift,
                    offSetY,
                    recWidth,
                    recHeight),
                color,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);


        }

        private void ProcessFaceLeft(KeyboardState keyState) // when player face left
        {
            // walk right
            if (keyState.IsKeyDown(Keys.D))
            {
                state = playerState.FaceRight;
            }

            // walk left
            if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.W)) 
            {
                state = playerState.WalkLeft;
            }

            if (keyState.IsKeyDown(Keys.J))
            {
                state = playerState.ShootingLeft;
            }
        }

        private void ProcessFaceRight(KeyboardState keyState) // when player face right
        {
            // walk left
            if (keyState.IsKeyDown(Keys.A)) 
            {
                state = playerState.FaceLeft;
            }

            // walk right
            if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.W)) 
            {
                state = playerState.WalkRight;
            }

            if (keyState.IsKeyDown(Keys.J))
            {
                state = playerState.ShootingRight;
            }
        }

        private void ProcessWalkRight(KeyboardState keyState) // when player walk right
        {
            

            // stop when D, S, W stop being pressed
            if (keyState.IsKeyUp(Keys.D) && keyState.IsKeyUp(Keys.S) && keyState.IsKeyUp(Keys.W))  
            {
                state = playerState.FaceRight;
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                location.X += speed;
            }

            
            if (keyState.IsKeyDown(Keys.S))
            {
                location.Y += speed;
            }

            if (keyState.IsKeyDown(Keys.W))
            {
                location.Y -= speed;
            }
        }

        private void ProcessWalkLeft(KeyboardState keyState) // when player walk right
        {
            
            // stop when A, S, W stop being pressed
            
            if (keyState.IsKeyUp(Keys.A) && keyState.IsKeyUp(Keys.S) && keyState.IsKeyUp(Keys.W))
            {
                state = playerState.FaceLeft;
            }

            // move around
            if (keyState.IsKeyDown(Keys.S))
            {
                location.Y += speed;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                location.X -= speed;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                location.Y -= speed;
            }
        }

        private void ProcessShootRight(KeyboardState keyState, GameTime gameTime, double shootTimer)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            // Check if timer is within a small range around 0.67
            // Cannot use == because the timing is so fast to register
            if (Math.Abs(timer - 0.67) < 0.01 )
            {
                // spawn a projectile
                projectileLoc = new Rectangle((int)(location.X + 64), (int)(location.Y + (recHeight / 2)), 20,18);

                Projectile projectile = new Projectile
                    (projectileSheet, projectileLoc, projectileState.FaceRight, windowWidth, windowHeight);
                projectiles.Add(projectile);
            }

            if (timer >= shootTimer)
            {
                state = playerState.FaceRight;
                timer = 0;
                shootFrame = 0;
            }
        }

        private void ProcessShootLeft(KeyboardState keyState, GameTime gameTime, double shootTimer)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            // Check if timer is within a small range around 0.67
            // Cannot use == because the timing is so fast to register
            if (Math.Abs(timer - 0.67) < 0.01)
            {
                // spawn a projectile
                projectileLoc = new Rectangle((int)location.X - 10, (int)location.Y + (recHeight / 2), 20, 18);

                Projectile projectile = new Projectile
                    (projectileSheet, projectileLoc, projectileState.FaceLeft, windowWidth, windowHeight);
                projectiles.Add(projectile);
            }

            if (timer >= shootTimer)
            {
                state = playerState.FaceLeft;
                timer = 0;
                shootFrame = 0;
            }
        }

        private void ProcessGetHurt (GameTime gameTime)
        {
            hurtTimer += gameTime.ElapsedGameTime.TotalSeconds;
            
            if (hurtTimer > 0.2)
            {
                isHurt = false;
                hurtTimer = 0;
            }
        }
    }
}
