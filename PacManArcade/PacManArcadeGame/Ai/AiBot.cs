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
            if (_lastX == location.CellX && _lastY == location.CellY) return currentDirection;

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

            var pacMan = _aiMap.Cell(location.CellX, location.CellY);

            var moves = new[] {Direction.Up, Direction.Down, Direction.Left, Direction.Right}
                .Where(d => pacMan.CellInDirection(d).IsPlayArea)
                .ToList();

            if (_counter > 0)
            {
                _counter--;
            }
            if (moves.Count > 1 && _counter > 0)
            {
                // Don't swap back on self if moved recently

                moves.Remove(currentDirection.Opposite());
            }

            // Work out which direction the pill/firghtened ghost is in

            var idealDirection = _aiMap.WorkBackTo(cell.X, cell.Y, x, y);

            // Get the distances of each alive ghost

            var ghostDistances = ghosts.Where(AvoidGhost)
                .Select(g => new GhostDistance(g, _aiMap.Cell(g.Location.CellX, g.Location.CellY).Distance))
                .OrderBy(gd => gd.Distance);
            
            // Avoid the direction of the ghost

            foreach (var ghostDistance in ghostDistances.Where(g=>g.Distance<11))
            {
                var nextTarget = ghostDistance.Ghost.NextTarget;
                var nextDistance = _aiMap.Cell(nextTarget.CellX, nextTarget.CellY).Distance;

                // Check if ghost is moving closer or is very close

                if (nextDistance < ghostDistance.Distance || ghostDistance.Distance < 11)
                {
                    var ghostDirection = _aiMap.WorkBackTo(ghostDistance.Ghost.Location.CellX,
                        ghostDistance.Ghost.Location.CellY, x, y);
                    if (moves.Contains(ghostDirection) && moves.Count > 1)
                    {
                        moves.Remove(ghostDirection);
                    }
                }
            }

            // Go in direction of pill if safe

            var bestDirection = moves.Contains(idealDirection) ? idealDirection : moves[0];
            
            if (bestDirection != _last)
            {
                _counter = 2;
            }

            _last = bestDirection;

            return bestDirection;
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
