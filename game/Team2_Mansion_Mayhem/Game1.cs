using System;
using System.Collections.Generic;
using System.IO;
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
        private SpriteFont headerFont;
        private SpriteFont normalFont;
        //textures

        //misc
        private int windowHeight;
        private int windowWidth;


        // player
        private Player player;
        private Rectangle playerLoc;
        private Texture2D playerSprite;
        private int playerHealth;
        private int playerDamage;
        private int playerSpeed;
        private int playerDefense;
        private string[] playerData;

        // projectile
        private Texture2D projectileSprite;
        private Vector2 projectileLoc;
        private Projectile projectile;

        // monster
        private Monster monster;
        private Rectangle monsterLoc;
        private Texture2D monsterSprite;
        private int monsterHealth;
        private int monsterDamage;
        private int monsterSpeed;
        private int monsterDefense;
        private string[] monsterData;

        // monster
        private Ghost ghost;
        private Rectangle ghostLoc;
        private Texture2D ghostSprite;
        private int ghostHealth;
        private int ghostDamage;
        private int ghostSpeed;
        private int ghostDefense;
        private string[] ghostData;
        private List<Monster> monsters;

        // map
        private Map map;
        private Texture2D mapSprite;

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
            
            // read and intialize data
            monsterData = new string[5];
            monsterData = LoadStats("Monster");

            ghostData = new string[5];
            ghostData = LoadStats("Ghost");

            playerData = new string[5];
            playerData = LoadStats("Player");
            for (int i = 1;  i < 5; i++) 
            {
                if (i == 1)
                {
                    int.TryParse(playerData[i], out playerHealth);
                    int.TryParse(monsterData[i], out monsterHealth);
                    int.TryParse(ghostData[i], out ghostHealth);
                }
                if (i == 2)
                {
                    int.TryParse(playerData[i], out playerDefense);
                    int.TryParse(monsterData[i], out monsterDefense);
                    int.TryParse(ghostData[i], out ghostDefense);
                }
                if (i == 3)
                {
                    int.TryParse(playerData[i], out playerDamage);
                    int.TryParse(monsterData[i], out monsterDamage);
                    int.TryParse(ghostData[i], out ghostDamage);
                }
                if (i == 4)
                {
                    int.TryParse(playerData[i], out playerSpeed);
                    int.TryParse(monsterData[i], out monsterSpeed);
                    int.TryParse(ghostData[i], out ghostSpeed);
                }
            }
            monsters = new List<Monster>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            debugFont = Content.Load<SpriteFont>("Fonts/Debugfont");
            headerFont = Content.Load<SpriteFont>("Fonts/Header");
            normalFont = Content.Load<SpriteFont>("Fonts/normalFont");
            // load sprite
            playerLoc = new Rectangle(50, 50, 22, 49);
            playerSprite = Content.Load<Texture2D>("Sprites/newPlayerSpriteSheet");

            projectileSprite = Content.Load<Texture2D>("Sprites/projectileSpriteSheet");

            monsterLoc = new Rectangle (200, 200, 64, 53);
            monsterSprite = Content.Load<Texture2D>("Sprites/monsterSpriteSheet");

            mapSprite = Content.Load<Texture2D>("Sprites/mapSpriteSheet");

            map = new Map(mapSprite, windowWidth, windowHeight);
            monster = new Monster(monsterSprite,monsterLoc, monsterHealth, monsterDefense, monsterDamage, monsterSpeed, monsterState.WalkRight);
            player = new Player(playerSprite, playerLoc, playerHealth, playerDefense, playerDamage, playerSpeed,
                playerState.FaceRight, kbState, projectileSprite, windowWidth, windowHeight);
            monsters.Add(monster);
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

                    monster.Update(gameTime);
                    monster.Chase(player.Location);
                    monster.ChangeState(gameTime,player.Location);

                    foreach (Projectile projectile in player.Projectiles)
                    {
                        foreach (Monster monster in monsters)
                        {
                            projectile.CheckCollision(monster, player.Damage);
                        }
                    }
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
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            // TODO: Add your drawing code here

            if (debugEnabled) 
            {
                _spriteBatch.DrawString(debugFont, $"GameState: {currentState}", new Vector2(5, windowHeight - 18), Color.Black);
            }

            switch (currentState)
            {
                case GameState.MainMenu:
                    _spriteBatch.DrawString(headerFont, "Mansion Mayhem", new Vector2(275, 175), Color.Red);
                    _spriteBatch.DrawString(normalFont, 
                        "Instruction: \nMove: WASD \nShoot : J \nStartGame: Space",
                        new Vector2(175, 250), Color.White);
                    break;

                case GameState.Game:
                    map.Draw(_spriteBatch);
                    player.Draw(_spriteBatch);
                    monster.Draw(_spriteBatch);
                    _spriteBatch.DrawString(debugFont, string.Format("playerState: {0}", player.State),
                        new Vector2(10, 10), Color.White);
                    _spriteBatch.DrawString
                        (debugFont, string.Format("Timer: {0} \nShoot timer:{1}" +
                        "\nProjectiles count: {2}\nMonster position: {3},{4}" +
                        "\nMonster Data: {5}, {6}, {7}, {8}, {9}", 
                        player.Timer, player.ShootTimer, player.Count, monster.X, monster.Y, 
                        monster.Health, monster.Defense, monster.Damage, monster.Speed, monster.Alive),
                        new Vector2(10, 30), Color.White);

                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(headerFont, "GAME OVER", new Vector2(275, 175), Color.Red);
                    break;
            }

            base.Draw(gameTime);

            _spriteBatch.End();
        }

        public string[] LoadStats(string type)
        {
            string[] data = new string[5];
            string line = null;
            StreamReader output = null;
            string path = "..//..//..//stats.txt";
            try
            {
                output = new StreamReader(path);
                while ((line = output.ReadLine()) != null)
                {
                    if (line.Contains(","))
                    {
                        string[] splitData = line.Split(',');
                        if (splitData[0].ToLower() == type.ToLower())
                        {
                            data = splitData;
                        }
                    }
                }
                return data;
            }
            catch (Exception e) 
            {
                
            }
            finally
            {
                // close the file
                if (output != null)
                {
                    output.Close();
                }
            }
            return data;
        }
    }
}
