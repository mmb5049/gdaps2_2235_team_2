using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team2_Mansion_Mayhem.Content.Sprites;

namespace Team2_Mansion_Mayhem
{
    // the floor of the map
    internal class Map
    {
        // fields
        private Texture2D spriteSheet;
        // for the # of tiles needed on the floor
        private int windowWidth;
        private int windowHeight;

        private int rectOffsetY = 80;
        private int tileFrameX = 16;
        private int tileFrameY = 16;

        List<Obstacle> obstacles = new List<Obstacle>();

        // properties
        public List<Obstacle> Obstacles
        {
            get { return obstacles; }
        }

        // constructor
        public Map(Texture2D spriteSheet, int windowWidth, int windowHeight)
        {
            this.spriteSheet = spriteSheet;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        // methods

        // draws tiles across the screen for the floor
        public void Draw(SpriteBatch sb)
        {
            int tilesRow = windowWidth / tileFrameX;
            int tilesCol = windowHeight / tileFrameY;
            Wall walls = new Wall(spriteSheet, windowWidth, windowHeight);
            
            for(int i = 0; i < tilesRow; i++)
            {
                for(int j = 0; j < tilesCol; j++)
                {
                    sb.Draw(
                        spriteSheet,
                        new Vector2((float)(i * tileFrameX), (float)(j * tileFrameY)),
                        new Rectangle(0, rectOffsetY, tileFrameX, tileFrameY),
                        Color.White);
                        
                }
            }
            walls.DrawTopWall(sb);
            walls.DrawSideWalls(sb);
            obstacles.Add(walls);

            for(int i = 1; i < 4; i++)
            {
                Obstacle obs = new Obstacle(spriteSheet, windowWidth, windowHeight);
                obs.Position = new Rectangle(i * 100 - 10, i * 100 - 10, 26, 16);
                obs.DrawTable(sb, new Vector2(i * 100, i * 100));
                obs.DrawPaintings(sb);

                obstacles.Add(obs);
            }
        }

        public void DisplayHealth(Player p, SpriteBatch sb)
        {
            if(p.Health <= 100 && p.Health >= 80)
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2((float)10, (float)windowHeight - 50), 
                    new Rectangle(0, 0, 16, 16),
                    Color.Red,
                    0,
                    default,
                    (float)3, // triple the size of the heart sprite
                    default,
                    0);
            }
            else if(p.Health < 80 && p.Health >= 60)
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2((float)10, (float)windowHeight - 50),
                    new Rectangle(16, 0, 16, 16),
                    Color.Red,
                    0,
                    default,
                    (float)3, // triple the size of the heart sprite
                    default,
                    0);
            }
            else if(p.Health < 60 && p.Health >= 40)
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2((float)10, (float)windowHeight - 50),
                    new Rectangle(32, 0, 16, 16),
                    Color.Red,
                    0,
                    default,
                    (float)3, // triple the size of the heart sprite
                    default,
                    0);
            }
            else if(p.Health < 40 && p.Health >= 20)
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2((float)10, (float)windowHeight - 50),
                    new Rectangle(48, 0, 16, 16),
                    Color.Red,
                    0,
                    default,
                    (float)3, // triple the size of the heart sprite
                    default,
                    0);
            }
            else // health less than 20
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2((float)10, (float)windowHeight - 50),
                    new Rectangle(64, 0, 16, 16),
                    Color.Red,
                    0,
                    default,
                    (float)3, // triple the size of the heart sprite
                    default,
                    0);
            }
        }
    }
}
