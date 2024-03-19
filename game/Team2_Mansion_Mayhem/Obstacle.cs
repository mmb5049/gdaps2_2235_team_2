using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team2_Mansion_Mayhem
{
    // obstacles that the player and monsters can't walk through
    internal class Obstacle
    {
        // fields
        protected Vector2 position;
        protected Texture2D spriteSheet;
        // could be used to determine where certain obstacles go
        protected int windowWidth;
        protected int windowHeight;

        // other obstacles will be added during Sprint 3

        public Obstacle(Texture2D spriteSheet, int windowWidth, int windowHeight)
        {
            this.spriteSheet = spriteSheet;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        // properties
        public float X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }
        public float Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }

    }
}
