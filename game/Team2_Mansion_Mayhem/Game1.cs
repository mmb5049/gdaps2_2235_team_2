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
        private Random rng = new Random();
        //control states
        private GameState currentState;
        private KeyboardState kbState;
        private KeyboardState preKbState;

        //fonts
        private SpriteFont debugFont;
        private SpriteFont headerFont;
        private SpriteFont normalFont;

        //levels
        private int currentLevel;

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
        private List<Enemy> enemies;

        // map & obstacles
        private Map map;
        private Texture2D mapSprite;
        private Obstacle obstacles;

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
            enemies = new List<Enemy>();
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

            obstacles = new Obstacle(mapSprite, windowWidth, windowHeight);

            map = new Map(mapSprite, windowWidth, windowHeight);
            monster = new Monster(monsterSprite,monsterLoc, monsterHealth, monsterDefense, monsterDamage, monsterSpeed, monsterState.WalkRight);
            player = new Player(playerSprite, playerLoc, playerHealth, playerDefense, playerDamage, playerSpeed,
                playerState.FaceRight, kbState, projectileSprite, windowWidth, windowHeight);

            enemies.Add(monster);
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
                        NextLevel();
                    }
                    break;

                case GameState.Game:
                    // in game update logic to be added
                    player.Update(gameTime);
                    
                    if (enemies != null)
                    {
                        foreach (Monster monster in enemies)
                        {
                            monster.Update(gameTime, player);
                            monster.Chase(player.Location, windowWidth, windowHeight);
                            monster.ChangeState(gameTime, player.Location);
                            monster.StartAttack(player.Location);
                        }

                        foreach (Projectile projectile in player.Projectiles)
                        {
                            foreach (Enemy enemy in enemies)
                            {
                                projectile.CheckCollision(enemy, player.Damage);
                            }
                        }
                    }

                    for (int i = enemies.Count - 1; i >= 0; i--) // iterate through the list backward
                    {
                        if (enemies[i].Alive == false)
                        {
                            enemies.RemoveAt(i);
                        }
                    }

                    if (enemies.Count == 0)
                    {
                        NextLevel();
                    }
                    /* temporary way to move from Game to GameOver 
                     * if we need to check that state for anything
                    */
                    if (player.Alive == false)
                    {
                        currentState = GameState.GameOver;
                    }
                    break;
                case GameState.GameOver:
                    // sends player back to main menu when they press Space on the GameOver screen
                    if (preKbState.IsKeyUp(Keys.Space) && kbState.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.MainMenu;
                        ResetGame();
                        player.Reset();
                    }
                    ResetGame();
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

            

            switch (currentState)
            {
                case GameState.MainMenu:
                    _spriteBatch.DrawString(headerFont, "Mansion Mayhem", new Vector2(275, 175), Color.Red);
                    _spriteBatch.DrawString(normalFont, 
                        "Instruction: \nMove: WASD \nShoot : Mouse \nStartGame: Space",
                        new Vector2(175, 250), Color.White);
                    break;

                case GameState.Game:
                    map.Draw(_spriteBatch);
                    player.Draw(_spriteBatch, debugEnabled, debugFont);
                    foreach(Monster monster in enemies)
                    {
                        monster.Draw(_spriteBatch, debugEnabled, debugFont);
                    }

                    /*_spriteBatch.DrawString(debugFont, string.Format("playerState: {0}", player.State),
                        new Vector2(10, 10), Color.White);
                    _spriteBatch.DrawString
                        (debugFont, string.Format("Timer: {0} \nDamageTaken:{1}" +
                        "\nDefense: {2}\nMonster position: {3},{4}" +
                        "\nMonster Data: {5}, {6}, {7}, {8}, {9}" +
                        "\nAttack Range: {10} {11}", 
                        player.Timer, player.DamageIntake, player.Defense, monster.X, monster.Y, 
                        monster.Health, monster.Defense, monster.Damage, monster.Speed, monster.Alive,
                        monster.attackRangeX, monster.attackRangeY),
                        new Vector2(300, 10), Color.White);*/

                    

                    _spriteBatch.DrawString(normalFont, 
                        string.Format("Health: {0}" +
                        "\nLevel: {1}" +
                        "\nEnemy Count: {2}"
                        , player.Health,currentLevel, enemies.Count), 
                        new Vector2(10,10), Color.White);
                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(headerFont, "GAME OVER", new Vector2(275, 175), Color.Red);
                    break;
            }

            if (debugEnabled)
            {
                _spriteBatch.DrawString(debugFont, $"---DEBUG---\nGameState: {currentState}", new Vector2(5, windowHeight - 36), Color.Black);
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

        public void NextLevel()
        {
            currentLevel += 1;
            enemies.Clear(); // Clear the list

            // decide how many enemies in a level
            int baseMonster = 1;
            int extraMonster = 1;
            int totalMonster = baseMonster + (extraMonster * (currentLevel) / 2); // increase enemy per 2 levels

            for (int i = 0; i < totalMonster; i++)
            {
                monsterLoc = new Rectangle(rng.Next(10, windowWidth - 53), rng.Next(10, windowHeight - 64), 64, 53);
                monster = new Monster(monsterSprite, monsterLoc, 
                    monsterHealth, monsterDefense, monsterDamage, monsterSpeed, monsterState.WalkRight);
                enemies.Add(monster);
            }
        }

        public void ResetGame()
        {
            currentLevel = 0;
        }
    }
}
