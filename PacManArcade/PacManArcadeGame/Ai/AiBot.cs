using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacManArcadeGame.GameItems;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.Ai
{
    public class AiBot
    {
        private readonly AiMap _aiMap;
        private int _counter;
        private Direction _last;
        private int _lastX;
        private int _lastY;

        public AiBot(Map.Map map)
        {
            _aiMap = new AiMap(map);
        }

        public Direction BestMove(Location location, Direction currentDirection, IEnumerable<Ghost> ghosts)
        {
            // Don't change direction if still of the same cell

            if (_lastX == location.CellX && _lastY == location.CellY)
                return _last;

            if (_counter > 0)
            {
                _counter--;
                return _last;
            }

            // Don't change direction for 3 ticks

            _counter = 2;

            _lastX = location.CellX;
            _lastY = location.CellY;

            // Get the distance to each cell

            var x = Math.Min(Math.Max(0, location.CellX), _aiMap.Width - 1);
            var y = Math.Min(Math.Max(0, location.CellY), _aiMap.Height - 1);
            _aiMap.CalcDistance(x, y);

            // Find the closest pill

            var cell = _aiMap.ClosestPill();

            // Find the closest frightened ghost
            
            var frightenedDistances = ghosts.Where(g=>g.Frightened)
                .Select(g => new GhostDistance(g, _aiMap.Cell(g.Location.CellX, g.Location.CellY).Distance))
                .OrderBy(gd => gd.Distance).ToList();

            // Change direction if there is one and it's a short distance away

            if (frightenedDistances.Count > 0 && frightenedDistances[0].Distance < 8)
                cell = _aiMap.Cell(frightenedDistances[0].Ghost.Location.CellX,
                    frightenedDistances[0].Ghost.Location.CellY);

            // Get list of available moves

            var moves = new List<Direction>();

            var pacMan = _aiMap.Cell(location.CellX, location.CellY);
            if (pacMan.Above.PlayArea)
                moves.Add(Direction.Up);
            if (pacMan.Below.PlayArea)
                moves.Add(Direction.Down);
            if (pacMan.Left.PlayArea )
                moves.Add(Direction.Left);
            if (pacMan.Right.PlayArea)
                moves.Add(Direction.Right);

            //if (moves.Count > 1)
            //    moves.Remove(currentDirection.Opposite());

            // Work out which direction the pill is in

            var pillDirection = _aiMap.WorkBackTo(cell.X, cell.Y, x, y);

            // Get the distances of each alive ghost

            var ghostDistances = ghosts.Where(AvoidGhost)
                .Select(g => new GhostDistance(g, _aiMap.Cell(g.Location.CellX, g.Location.CellY).Distance))
                .OrderBy(gd => gd.Distance);
            
            // Avoid the direction of the ghost

            foreach (var ghostDistance in ghostDistances.Where(g=>g.Distance<8f))
            {
                var ghostDirection = _aiMap.WorkBackTo(ghostDistance.Ghost.Location.CellX, ghostDistance.Ghost.Location.CellY, x, y);
                if (moves.Contains(ghostDirection) && moves.Count > 1)
                {
                    moves.Remove(ghostDirection);
                }
            }

            // Go in direction of pill if safe

            if (moves.Contains(pillDirection))
            {
                _last = pillDirection;
                return pillDirection;
            }
            
            // Otherwise go in safe direction

            _last = moves[0];
            return moves[0];
        }

        /// <summary>
        /// Is a ghost dangerous
        /// </summary>
        /// <param name="ghost"></param>
        /// <returns></returns>
        private bool AvoidGhost(Ghost ghost)
        {
            return !ghost.Frightened &&
                   (ghost.State == GhostState.Alive
                    || ghost.State == GhostState.GhostDoor
                    || ghost.State == GhostState.LeaveHouse);
        }
    }
}
