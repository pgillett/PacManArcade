﻿using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame
{
    public class PacMan
    {
        public Location Location { get; private set; }
        public Direction Direction { get; private set; }
        public Animation Animation { get; private set; }
        public bool Dying { get;private set; }

        public PacMan(Location location, Direction direction)
        {
            Location = location;
            Direction = direction;
            Animation = new Animation(4, 2);
            Dying = false;
        }
        
        public void Die()
        {
            Animation = new Animation(12, 3, false);
            Dying = true;
        }

        public void ChangeDirection(Direction direction)
        {
            Direction = direction;
        }

        public void Move(decimal x, decimal y)
        {
            Location = Location.Add(x, y);
            Animation.Tick();
        }

        public void MoveTowards(Location location)
        {
            var dx = (Location.X < location.X ? 0.125m : 0) +
                     (Location.X > location.X ? -0.125m : 0);
            var dy = (Location.Y < location.Y ? 0.125m : 0) +
                     (Location.Y > location.Y ? -0.125m : 0);
            if(dx!=0 || dy !=0)
                Move(dx,dy);
        }
    }
}