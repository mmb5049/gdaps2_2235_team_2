using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        Color.White,
                        0,
                        Vector2.Zero,
                        1.0f,
                        0,
                        0);
                        
                }
            }
            walls.DrawTopWall(sb);
        }
    }
}
