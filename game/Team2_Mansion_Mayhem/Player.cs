using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
        Dashing
    }

    internal class Player : IDebug
    {
        // fields
        private Texture2D spriteSheet;
        private Texture2D abilities;
        private Rectangle location;
        private Rectangle obstacleBounds;
        private Vector2 resetPoint;
        private playerState state;
        private bool isHurt;
        private bool isShooting = false;
        private KeyboardState kbState;
        private KeyboardState preKbState;
        private MouseState mouseState;
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
        private bool invincible = false;
        private int damageIntake = 0;
        private int maxSpeed;

        // abilities
        private double dashAbilityProgress = 0;
        private double dashAbilityCooldown = 1;
        private playerState dashState;
        private int dashX;
        private int dashY;

        private double dashProgress = 0;
        private double dashTimer = 0.2;


        private double screamAbilityProgress = 0;
        private double screamAbilityCooldown = 10;

        // projectile
        private double lookAngle;
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
        private int shootEventFrame = 4;

        private Random rng = new Random();

        // Constructor
        public Player(Texture2D spriteSheet, Texture2D abilitySheet, Rectangle location, int health, int defense, int damage, int speed 
            ,playerState state, KeyboardState kbState, Texture2D projectileSheet, int windowWidth, int windowHeight
            )
        {
            this.health = health;
            this.maxHealth = health;
            this.defense = defense;
            this.damage = damage;   
            this.speed = speed;
            this.maxSpeed = speed;
            this.spriteSheet = spriteSheet;
            this.location = location;
            this.obstacleBounds = new Rectangle(location.X, location.Y + location.Height/2, location.Width, location.Height/2 - 4);
            this.abilities = abilitySheet;

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

        public int Speed
        {
            get { return speed;}
            set { speed = value; }  
        }
        public virtual string DebugStats
        {
            //return a list of stats to be printed 
            get
            {
                return
                 $"State: {state}\n" +
                 $"Health: {health}/{maxHealth} \n " +
                 $"Defense: {defense} \n " +
                 $"Damage: {damage}\n " +
                 $"Position: ({X}, {Y})\n" +
                 $"Mouse Position: ({mouseState.Position})\n" +
                 $"(Shoot angle of {lookAngle * 180/Math.PI})\n" +
                 $"Shooting? {isShooting}";
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
            speed = maxSpeed;
        }

        /// <summary>
        /// Returns the angle between the center of the character and the mouse's position in degrees. 
        /// </summary>
        public double UpdateAngle()
        {
            double distanceX = mouseState.Position.X - location.Center.X;
            double distanceY = mouseState.Position.Y - location.Center.Y;
            return Math.Atan2(distanceY, distanceX);
        }

        public void DamageTaken(int damage)
        {
            if (!invincible)
            {
                damageIntake = damage - defense;

                if (damageIntake < 0) // avoid taking negative damage
                {
                    damageIntake = 0;
                }

                health -= damageIntake;
            }
        }

        public void Update(GameTime gameTime, List<Obstacle>obtacles)
        {
            kbState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            lookAngle = UpdateAngle();
            shootTimer = .7;
            UpdateAnimation(gameTime);
            UpdateAbilites(gameTime);

            if ((mouseState.RightButton == ButtonState.Pressed) && dashAbilityProgress == 1)
            {
                // Determine movement direction based on keyboard input
                dashX = (Convert.ToInt32(kbState.IsKeyDown(Keys.D)) - Convert.ToInt32(kbState.IsKeyDown(Keys.A)));
                dashY = (Convert.ToInt32(kbState.IsKeyDown(Keys.S)) - Convert.ToInt32(kbState.IsKeyDown(Keys.W)));

                //failsafe to ensure the plauer dashes in some direction
                if (dashX == 0)
                {
                    if (state == playerState.FaceLeft)
                    {
                        dashX = -1;
                    }
                    else
                    {
                        dashX = 1;
                    }
                }

                dashProgress = dashTimer;
                dashState = state;
                state = playerState.Dashing;
                invincible = true;
            }

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


            //dash if dashing, else move normally
            if (dashProgress > 0)
            {
                ProcessDash(gameTime, dashX, dashY, obtacles);
            }
            else
            {
                ProcessMovement(gameTime, kbState, obtacles);
            }
            

            if (kbState.IsKeyDown(Keys.E) && screamAbilityProgress == 1)
            {
                ProcessScream();
            }
        }

        public void ProcessMovement(GameTime gameTime, KeyboardState kbState, List<Obstacle> obstacles)
        {
            int movementX = 0;
            int movementY = 0;

            if (isShooting == false)
            {
                {
                    // Store the current location before any movement for bounds checking
                    int oldX = location.X;
                    int oldY = location.Y;

                    // Determine movement direction based on keyboard input
                    movementX = (Convert.ToInt32(kbState.IsKeyDown(Keys.D)) - Convert.ToInt32(kbState.IsKeyDown(Keys.A)));
                    movementY = (Convert.ToInt32(kbState.IsKeyDown(Keys.S)) - Convert.ToInt32(kbState.IsKeyDown(Keys.W)));

                    // Calculate the new position
                    int newX = location.X + speed * movementX;
                    int newY = location.Y + speed * movementY;

                    Rectangle newPlayerBounds = new Rectangle(newX, newY + obstacleBounds.Height, obstacleBounds.Width, obstacleBounds.Height);

                    foreach (Obstacle obstacle in obstacles)
                    {
                        if (newPlayerBounds.Intersects(obstacle.Position))
                        {
                            // If there's a collision, don't update the player's position
                            newX = oldX;
                            newY = oldY;
                            break; // No need to check further obstacles
                        }
                    }

                    // Check if the new position is within the window bounds
                    if (newX >= -10 && newX + location.Width <= windowWidth - 15)
                    {
                        // Check for collision with each obstacle
                        
                        location.X = newX; // Update X position if within bounds
                        obstacleBounds.X = newX;
                    }

                    if (newY >= -10 && newY + location.Height <= windowHeight)
                    {
                        // Check for collision with each obstacle
                        
                        location.Y = newY; // Update Y position if within bounds
                        obstacleBounds.Y = newY + obstacleBounds.Height;
                    }

                }

                //the first two evaluate if the player is moving
                if (movementX == 1 && !isShooting)
                {
                    state = playerState.WalkRight;
                }
                else if (movementX == -1 && !isShooting)
                {
                    state = playerState.WalkLeft;
                }
                //this evaluates if the player is moving BUT only in the y direction
                else if (movementY != 0 && !isShooting)
                {
                    //in this case, the walk direction is based on the player's direction while still
                    if (state == playerState.FaceLeft)
                    {
                        state = playerState.WalkLeft;
                    }
                    if (state == playerState.FaceRight)
                    {
                        state = playerState.WalkRight;
                    }
                }
                //if player is standing still..
                else
                {
                    //if player's mouse is located to the right..
                    if (mouseState.X >= location.Center.X)
                    {
                        state = playerState.FaceRight;
                    }
                    //if player's mouse is located to the left..
                    else
                    {
                        state = playerState.FaceLeft;
                    }
                }

                //if attempting to shoot
                if (mouseState.LeftButton == ButtonState.Pressed || isShooting)
                {
                    //if player's mouse is located to the right..
                    if (mouseState.X >= location.Center.X)
                    {
                        state = playerState.ShootingRight;

                    }
                    //if player's mouse is located to the left..
                    else
                    {
                        state = playerState.ShootingLeft;
                    }

                    isShooting = true;
                }
            }
            else
            {
                if (mouseState.X >= location.Center.X)
                {
                    state = playerState.ShootingRight;
                }
                //if player's mouse is located to the left..
                else
                {
                    state = playerState.ShootingLeft;
                    
                }
                UpdateShootAnimation(gameTime);
            }
        }

        private void ProcessDash(GameTime gameTime,int movementX, int movementY, List<Obstacle> obstacles)
        {
            dashAbilityProgress = 0;
            // Store the current location before any movement for bounds checking
            int oldX = location.X;
            int oldY = location.Y;

            // Calculate the new position
            int newX = location.X + (speed * 3) * movementX;
            int newY = location.Y + (speed * 3) * movementY;

            Rectangle newPlayerBounds = new Rectangle(newX, newY + obstacleBounds.Height, obstacleBounds.Width, obstacleBounds.Height);

            foreach (Obstacle obstacle in obstacles)
            {
                if (newPlayerBounds.Intersects(obstacle.Position))
                {
                    // If there's a collision, don't update the player's position
                    newX = oldX;
                    newY = oldY;
                    break; // No need to check further obstacles
                }
            }

            // Check if the new position is within the window bounds
            if (newX >= -10 && newX + location.Width <= windowWidth - 15)
            {
                // Check for collision with each obstacle

                location.X = newX; // Update X position if within bounds
                obstacleBounds.X = newX;
            }

            if (newY >= -10 && newY + location.Height <= windowHeight)
            {
                // Check for collision with each obstacle

                location.Y = newY; // Update Y position if within bounds
                obstacleBounds.Y = newY + obstacleBounds.Height;
            }
            dashProgress = Math.Max(0, dashProgress - gameTime.ElapsedGameTime.TotalSeconds);
            
            if (dashProgress == 0)
                invincible = false;
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
        public void UpdateAbilites(GameTime gameTime) 
        {
            screamAbilityProgress = Math.Min(screamAbilityProgress + gameTime.ElapsedGameTime.TotalSeconds/ screamAbilityCooldown, 1);
            dashAbilityProgress = Math.Min((dashAbilityProgress + gameTime.ElapsedGameTime.TotalSeconds) / dashAbilityCooldown, 1);
            

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
                
                if (shootFrame == shootEventFrame) //if the animation is at the frame where the bullet is supposed to appear aka the apex of the thrust
                {
                    ProcessShoot();
                }
                if (shootFrame > frameCount)
                {
                    shootFrame = 1;
                    isShooting = false;
                }

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
                case playerState.Dashing:
                    if (dashState == playerState.WalkLeft || dashState == playerState.FaceLeft)
                    {
                        DrawWalking(sb, SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        DrawWalking(sb, SpriteEffects.None);
                    }
                    break;
            }

            DrawAbilities(sb);

            if (projectiles != null)
            {
                foreach (Projectile projectile in projectiles)
                {
                    projectile.Draw(sb, debugEnabled, debugFont);
                }
            }

           

            //draw stats under position in the event that debug is enabled
            if (debugEnabled)
            {
                sb.DrawString(debugFont, DebugStats,
                new Vector2(X, Y + location.Height), Color.Black);
            }
        }

        public void DrawAbilities(SpriteBatch sb)
        {
            //draw the dash UI background
            sb.Draw(abilities, new Rectangle(60, windowHeight - 50, 48, 48), new Rectangle(0, 0, 16, 16), Color.White);

            
            //draw the dash UI foreground
            sb.Draw(abilities, new Rectangle(60, (windowHeight - 50) + (48 - (int)Math.Round(48 * dashAbilityProgress)), 48, 48 - (48 - (int)Math.Round(48 * dashAbilityProgress))), new Rectangle(0, 16 + (16 - (int)Math.Round(16 * dashAbilityProgress)), 16, (int)Math.Round(16 * dashAbilityProgress)), Color.Cyan);

            //draw the dash UI background
            sb.Draw(abilities, new Rectangle(110, windowHeight - 50, 48, 48), new Rectangle(16, 0, 16, 16), Color.White);
            sb.Draw(abilities, new Rectangle(110, (windowHeight - 50) + (48 - (int)Math.Round(48 * screamAbilityProgress)), 48, 48 - (48 - (int)Math.Round(48 * screamAbilityProgress))), new Rectangle(16, 16 + (16 - (int)Math.Round(16 * screamAbilityProgress)), 16, (int)Math.Round(16 * screamAbilityProgress)), Color.Green);
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




        private void ProcessShoot()
        {
                if (state == playerState.ShootingLeft)
                {
                    // spawn a projectile
                    projectileLoc = new Rectangle((int)(location.Center.X - 3), (int)(location.Center.Y + 6), 20, 18);
                }
                else
                {
                    // spawn a projectile
                    projectileLoc = new Rectangle((int)(location.Center.X + 16), (int)(location.Center.Y + 6), 20, 18);
                }
                

                Projectile projectile = new Projectile
                    (projectileSheet, projectileLoc,lookAngle, projectileState.FaceRight, windowWidth, windowHeight);
                projectiles.Add(projectile);
        }

        private void ProcessScream()
        {
            projectileLoc = new Rectangle((int)(location.Center.X), (int)(location.Center.Y), 20, 16);

            for (int i = 0; i < rng.Next(30, 50); i++)
            {
                Projectile projectile = new Projectile
                    (projectileSheet, projectileLoc, lookAngle + rng.Next(-2, 5) + rng.NextDouble(), projectileState.FaceRight, windowWidth, windowHeight);
                projectiles.Add(projectile);
            }

            screamAbilityProgress = 0;
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
