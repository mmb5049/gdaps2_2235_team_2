using System.Collections.Generic;
using System.Net;
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
        private Vector2 playerLoc;
        private Texture2D playerSprite;
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
            playerLoc = new Vector2(50f, 50f);
            playerSprite = Content.Load<Texture2D>("playerSpriteSheet");

            player = new Player(playerSprite, playerLoc, playerState.WalkRight);
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
                    break;

                case GameState.GameOver:
                    break;
            }

            base.Draw(gameTime);

            _spriteBatch.End();
        }
    }
}
