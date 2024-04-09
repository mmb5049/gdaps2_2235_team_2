using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Team2_Mansion_Mayhem.Content.Sprites;

namespace Team2_Mansion_Mayhem
{
    // obstacles that the player and monsters can't walk through
    internal class Obstacle
    {
        // fields
        protected Rectangle position;
        protected Texture2D spriteSheet;
        // could be used to determine where certain obstacles go
        protected int windowWidth;
        protected int windowHeight;

        public Obstacle(Texture2D spriteSheet, int windowWidth, int windowHeight)
        {
            this.spriteSheet = spriteSheet;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        // properties
        public int X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }
        public int Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }

        public Rectangle Position
        {
            get { return position; }
            set {  position = value; }
        }
        public void DrawTable(SpriteBatch sb, Vector2 position)
        {
            // variables for the table source rectangles
            int leftTableOffsetX = 13 * 16;
            int midTableOffsetX = 14 * 16;
            int rightTableOffsetX = 15 * 16;
            int tableOffsetY = 3 * 16;
            int tableLegOffsetY = 4 * 16;

            // draw the left part of the table
            sb.Draw(
                spriteSheet,
                position,
                new Rectangle(leftTableOffsetX, tableOffsetY, 16, 16),
                Color.White);
            // draw the middle part of the table
            sb.Draw(
                spriteSheet,
                new Vector2(position.X + 16, position.Y),
                new Rectangle(midTableOffsetX, tableOffsetY, 16, 16),
                Color.White);
            // draw the right part of the table
            sb.Draw(
                spriteSheet,
                new Vector2(position.X + 32, position.Y),
                new Rectangle(rightTableOffsetX, tableOffsetY, 16, 16),
                Color.White);
            // draw the table legs
            sb.Draw(
                spriteSheet,
                new Vector2(position.X, position.Y + 16),
                new Rectangle(leftTableOffsetX, tableLegOffsetY, 16, 16),
                Color.White);
            sb.Draw(
                spriteSheet,
                new Vector2(position.X + 32, position.Y + 16),
                new Rectangle(leftTableOffsetX, tableLegOffsetY, 16, 16),
                Color.White);
        }

        public void DrawPaintings(SpriteBatch sb)
        {
            int paintingYOffset = 16 * 3;
            int tileRows = windowWidth / 16;
            for (int i = 0; i < tileRows; i += 2)
            {
                sb.Draw(
                    spriteSheet,
                    new Vector2(i * 16, 0),
                    new Rectangle(16 * 11, paintingYOffset, 16, 16),
                    Color.White);
            }
        }

    }
}
