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
    // collision not implemented yet
    internal class Wall: Obstacle
    {
        // source for the background walls
        private int endWallOffsetY = 96;
        private int leftEndWallOffsetX = 0;
        private int rightEndWallOffsetX = 16;
        private int topWallOffsetX = 0;
        private int topWallOffsetY = 112;
        private int topWallCornerOffsetX = 128;
        private int topWallCornerOffsetY = 32;
        // source for the side walls
        private int sideWallOffsetX = 16;
        private int sideWallOffsetY = 128;


        public Wall(Texture2D spriteSheet, int windowWidth, int windowHeight):base(spriteSheet, windowWidth, windowHeight)
        {
            
        }

        // draws the top wall sprites and adds each tile to a list to detect collision
        public void DrawTopWall(SpriteBatch sb)
        {
            int tilesRow = windowWidth / 16;
            
            for (int i = 0; i  < tilesRow; i++)
            {
                if(i == 0)
                {
                    sb.Draw(
                        spriteSheet,
                        new Vector2((float)(i * 16), (float)(0)),
                        new Rectangle(leftEndWallOffsetX, endWallOffsetY, 16, 16),
                        Color.White);
            // adds the wall to the list to detect collisions
                }
                else if(i == tilesRow - 1)
                {
                    sb.Draw(
                        spriteSheet,
                        new Vector2((float)(i * 16), (float)(0)),
                        new Rectangle(rightEndWallOffsetX, endWallOffsetY, 16, 16),
                        Color.White);
                }
                else
                {
                    sb.Draw(
                        spriteSheet,
                        new Vector2((float)(i * 16), (float)(0)),
                        new Rectangle(topWallOffsetX, topWallOffsetY, 16, 16),
                        Color.White);
                }
            }
        }

        public void DrawSideWalls(SpriteBatch sb)
        {
            int wallTiles = windowHeight / 16;
            // draws the wall on the right side of the screen
            for(int i = 0; i < wallTiles; i++)
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2((float)(windowWidth - 16), (float)(i * 16)),
                    new Rectangle(sideWallOffsetX, sideWallOffsetY, 16, 16),
                    Color.White);
            }
            // draws the wall on the left side of the screen
            for (int i = 0; i < wallTiles; i++)
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2((float)(0), (float)(i * 16)),
                    new Rectangle(sideWallOffsetX, sideWallOffsetY, 16, 16),
                    Color.White,
                    0,
                    Vector2.Zero,
                    1,
                    SpriteEffects.FlipHorizontally,
                    0);
            }
        }

    }
}
