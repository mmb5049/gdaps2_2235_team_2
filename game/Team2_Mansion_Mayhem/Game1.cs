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
        private double localdiffculty = 1;
        //misc
        private int windowHeight;
        private int windowWidth;
        private Texture2D mainMenuBackground;
        private Texture2D title;
        private Texture2D gameOver;
        private Texture2D enemyHP;
        private Texture2D abilities;

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

        // ghost
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
        private Obstacle obstacle;
        private List<Obstacle> obstacles;

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
            obstacles = new List<Obstacle>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            debugFont = Content.Load<SpriteFont>("Fonts/Debugfont");
            headerFont = Content.Load<SpriteFont>("Fonts/Header");
            normalFont = Content.Load<SpriteFont>("Fonts/normalFont");

            mainMenuBackground = Content.Load<Texture2D>("Screen/mainMenuBackground");
            title = Content.Load<Texture2D>("Screen/title");
            gameOver = Content.Load<Texture2D>("Screen/gameOver");

            // Load sprites
            playerLoc = new Rectangle(50, 50, 22, 49);
            playerSprite = Content.Load<Texture2D>("Sprites/newPlayerSpriteSheet");

            projectileSprite = Content.Load<Texture2D>("Sprites/projectileSpriteSheet");

            monsterLoc = new Rectangle (200, 200, 32, 53);
            monsterSprite = Content.Load<Texture2D>("Sprites/monsterSpriteSheet");

            ghostLoc = new Rectangle(200, 200, 32, 32);
            ghostSprite = Content.Load<Texture2D>("Sprites/ghostSpriteSheet");

            mapSprite = Content.Load<Texture2D>("Sprites/mapSpriteSheet");

            enemyHP = Content.Load<Texture2D>("Sprites/enemyhealthbars");

            abilities = Content.Load<Texture2D>("Sprites/abilitiesUI");

            obstacle = new Obstacle(mapSprite, windowWidth, windowHeight);

            map = new Map(mapSprite, windowWidth, windowHeight);
            monster = new Monster(monsterSprite, enemyHP, monsterLoc, monsterHealth, monsterDefense, monsterDamage, monsterSpeed, monsterState.WalkRight);
            ghost = new Ghost(ghostSprite, enemyHP, ghostLoc, ghostHealth, ghostDefense, ghostDamage, ghostSpeed);
            player = new Player(playerSprite, abilities, playerLoc, playerHealth, playerDefense, playerDamage, playerSpeed,
                playerState.FaceRight, kbState, projectileSprite, windowWidth, windowHeight);

            enemies.Add(monster);
            enemies.Add(ghost);
            obstacles = map.Obstacles;
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            kbState = Keyboard.GetState();

            // Toggle debug if the user presses Tilde (~)
            if (kbState.IsKeyDown(Keys.OemTilde) && preKbState.IsKeyUp(Keys.OemTilde)) 
            { 
                debugEnabled = !debugEnabled;
            }

            switch (currentState)
            {
                case GameState.MainMenu:
                    // Starts game by pressing space
                    if(preKbState.IsKeyUp(Keys.Space) && kbState.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.Game;
                        NextLevel();
                    }
                    break;

                case GameState.Game:
                    // In game update logic to be added
                    player.Update(gameTime, obstacles);
                    
                    if (enemies != null)
                    {
                        foreach (Enemy enemy in enemies)
                        {
                            enemy.Update(gameTime, player, obstacles);
                            enemy.Chase(player.Location, windowWidth, windowHeight, obstacles);

                            if (enemy is Monster monsterEnemy)
                            {
                                monsterEnemy.ChangeState(gameTime, player.Location);
                                monsterEnemy.StartAttack(player.Location);
                            }
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

            switch (currentState)
            {
                case GameState.MainMenu:
                    _spriteBatch.Draw(mainMenuBackground, Vector2.Zero, Color.White);
                    _spriteBatch.Draw(title, new Vector2(100, 100), Color.White);
                    _spriteBatch.DrawString(normalFont, 
                        "Kill supernatural creatures to get to the farthest wave!" +
                        "\nMove: WASD \nShoot: Left Mouse \nDash: Right Mouse \nAbility: E \nStart Game: Space",
                        new Vector2(110, 255), Color.White);
                    break;

                case GameState.Game:
                    map.Draw(_spriteBatch);
                    player.Draw(_spriteBatch, debugEnabled, debugFont);

                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(_spriteBatch, debugEnabled, debugFont);
                    }

                    /*_spriteBatch.DrawString(debugFont, string.Format("playerState: {0}", player.State),
                        new Vector2(10, 10), Color.White);*/
                    _spriteBatch.DrawString
                        (debugFont, string.Format("{0}, \n {1}, {2}", monster.State, monster.attackRangeWidth, monster.attackRangeHeight) 
                        ,
                        new Vector2(300, 10), Color.White);

                    _spriteBatch.DrawString(normalFont, 
                        string.Format("Health: {0}" +
                        "\nLevel: {1}" +
                        "\nEnemy Count: {2}"
                        , player.Health,currentLevel, enemies.Count), 
                        new Vector2(10,10), Color.White);
                    map.DisplayHealth(player, _spriteBatch);
                    break;

                case GameState.GameOver:
                    _spriteBatch.Draw(mainMenuBackground, Vector2.Zero, Color.White);
                    _spriteBatch.Draw(gameOver, new Vector2(220, 110), Color.White);
                    _spriteBatch.DrawString(headerFont, "Level Reached: " + currentLevel, new Vector2(275, 270), Color.Red);
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
            if (currentLevel >= 7) 
            {
                localdiffculty += 0.25;
            }
            
            enemies.Clear(); // Clear the list

            // decide how many enemies in a level
            int baseMonster = 1;
            int extraMonster = 1;
            int totalMonster = baseMonster + (extraMonster * (currentLevel) / 2); // increase enemy per 2 levels
            
            for (int i = 0; i < totalMonster; i++)
            {
                monsterLoc = new Rectangle(rng.Next(10, windowWidth - 53), rng.Next(10, windowHeight - 64), 32, 53);
                monster = new Monster(monsterSprite, enemyHP, monsterLoc, (int)(monsterHealth * localdiffculty) + rng.Next(-5, 6), monsterDefense, (int)(monsterDamage), monsterSpeed, monsterState.WalkRight);
                
                // regenerate the monster position so it won't spawn inside an obstacle and got stuck
                foreach(Obstacle obstacle in obstacles)
                {
                    while (monster.Position.Intersects(obstacle.Position))
                    {
                        monsterLoc = new Rectangle(rng.Next(10, windowWidth - 53), rng.Next(10, windowHeight - 64), 32, 53);
                        monster = new Monster(monsterSprite, enemyHP, monsterLoc, (int)(monsterHealth * localdiffculty) + rng.Next(-5, 6), monsterDefense, (int)(monsterDamage * localdiffculty), monsterSpeed, monsterState.WalkRight);
                    }
                }

                enemies.Add(monster);

                ghostLoc = new Rectangle(rng.Next(10, windowWidth - 64), rng.Next(10, windowHeight - 64), 32, 30);
                ghost = new Ghost(ghostSprite, enemyHP, ghostLoc, (int)(ghostHealth * localdiffculty) + rng.Next(-5, 6), ghostDefense, (int)(ghostDamage * localdiffculty), ghostSpeed);
                enemies.Add(ghost);
            }
        }

        public void ResetGame()
        {
            currentLevel = 0;
        }
    }
}
