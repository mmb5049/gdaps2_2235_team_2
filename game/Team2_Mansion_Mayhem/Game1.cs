using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Team2_Mansion_Mayhem.Content.Sprites;

namespace Team2_Mansion_Mayhem
{
    enum GameState
    {
        MainMenu,
        Game,
        GameOver
    }

    public class Game1 : Game
    {
        //game controllers
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private bool debugEnabled;
        
        //control states
        private GameState currentState;
        private KeyboardState kbState;
        private KeyboardState preKbState;

        //fonts
        private SpriteFont debugFont;

        //textures

        //misc
        private int windowHeight;
        private int windowWidth;


        // player
        private Player player;
        private Rectangle playerLoc;
        private Texture2D playerSprite;

        // projectile
        private Texture2D projectileSprite;
        private Vector2 projectileLoc;
        private Projectile projectile;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            currentState = GameState.MainMenu;
            kbState = new KeyboardState();
            debugEnabled = false;

            windowHeight = _graphics.PreferredBackBufferHeight;
            windowWidth = _graphics.PreferredBackBufferWidth;


        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            debugFont = Content.Load<SpriteFont>("Fonts/Debugfont");

            // create player
            playerLoc = new Rectangle(50, 50, 64, 53);
            playerSprite = Content.Load<Texture2D>("Sprites/playerSpriteSheet");

            

            projectileLoc = new Vector2(200f, 200f);
            projectileSprite = Content.Load<Texture2D>("Sprites/projectileSpriteSheet");
            player = new Player(playerSprite, playerLoc, playerState.FaceRight, kbState, projectileSprite, windowWidth, windowHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            kbState = Keyboard.GetState();

            //toggle debug if the user presses Tilde (~)
            if (kbState.IsKeyDown(Keys.OemTilde) && preKbState.IsKeyUp(Keys.OemTilde)) 
            { 
                debugEnabled = !debugEnabled;
            }
            switch (currentState)
            {
                case GameState.MainMenu:
                    // starts game by pressing space
                    if(preKbState.IsKeyUp(Keys.Space) && kbState.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.Game;
                    }
                    break;
                case GameState.Game:
                    // in game update logic to be added
                    player.Update(gameTime);

                    /* temporary way to move from Game to GameOver 
                     * if we need to check that state for anything
                    */
                    if (preKbState.IsKeyUp(Keys.Space) && kbState.IsKeyDown(Keys.Space))
                    {
                        
                        currentState = GameState.GameOver;
                    }
                    break;
                case GameState.GameOver:
                    // sends player back to main menu when they press Space on the GameOver screen
                    if (preKbState.IsKeyUp(Keys.Space) && kbState.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.MainMenu;
                    }
                    break;
                default:
                    break;
            }
            preKbState = kbState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // TODO: Add your drawing code here

            if (debugEnabled) 
            {
                _spriteBatch.DrawString(debugFont, $"GameState: {currentState}", new Vector2(5, windowHeight - 18), Color.Black);
            }

            switch (currentState)
            {
                case GameState.MainMenu:
                    break;

                case GameState.Game:
                    player.Draw(_spriteBatch);

                    _spriteBatch.DrawString(debugFont, string.Format("playerState: {0}", player.State),
                        new Vector2(10, 10), Color.White);
                    _spriteBatch.DrawString
                        (debugFont, string.Format("Timer: {0} \nShoot timer:{1}" +
                        "\nProjectiles count: {2}", player.Timer, player.ShootTimer, player.Count),
                        new Vector2(10, 30), Color.White);

                    break;

                case GameState.GameOver:
                    break;
            }

            base.Draw(gameTime);

            _spriteBatch.End();
        }
    }
}
