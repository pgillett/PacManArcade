using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PacManArcadeGame
{
    public class Game
    {
        private Sprites _sprites;

        public Inputs Inputs;

        public int Height => Map.Height;
        public int Width => Map.Width;

        public UiState UiState;

        public PacMan PacMan;

        public Ghost Blinky;
        public Ghost Pinky;
        public Ghost Inky;
        public Ghost Clyde;

        public IEnumerable<Ghost> Ghosts => new[] {Blinky, Pinky, Inky, Clyde};

        public Location GhostHouse;

        public Map Map;

        public Display Display;

        private GameSetup _gameSetup;

        private int Ticks = 0;

        private AttractMode _attractMode;
        private CoinsInMode _coinsInMode;

        public Game(GameSetup gameSetup, Display display, Sprites sprites)
        {
            Display = display;
            _sprites = sprites;

            _gameSetup = gameSetup;
            ResetGame();
            

            
        }

        private void ResetGame()
        {
            Map = _gameSetup.Map;
            ResetGhostsAndPacMan();
        }

        private void ResetGhostsAndPacMan()
        {
            Blinky = _gameSetup.InitalBlinky.Ghost;
            Pinky = _gameSetup.InitalPinky.Ghost;
            Inky = _gameSetup.InitalInky.Ghost;
            Clyde = _gameSetup.IntialClyde.Ghost;
            PacMan = _gameSetup.InitialPacMan.PacMan;
        }

        private const int BoardYOffset = 3;
        public Action UiChangeState;

        public void Tick()
        {
            Ticks++;
            Display.ClearSprites();

            if (UiState == UiState.Attract)
            {
                _attractMode.Tick();
            }
            else if (UiState == UiState.CoinsIn)
            {
_coinsInMode.Tick();
            }
            else if (UiState == UiState.Playing)
            {
                DrawMap();



                foreach (var power in Map.PowerPills)
                {
                    DrawBoardSprite(_sprites.PowerPill(TickPosition(PowerPillAnim)), power);
                }

                foreach (var ghost in Ghosts)
                {
                    DrawBoardSprite(_sprites.Ghost(ghost.Colour, ghost.Direction, TickPosition(GhostAnim)), ghost.Location);
                }

                DrawBoardSprite(_sprites.PacMan(PacMan.Direction, 0, false), PacMan.Location);
            }
            else // UiState == UiState.HighScore
            {

            }

            
        }

        public int RenderTick = 0;
        public int TickTarget = 0;
        public int AttractStep = 0;
        public void ProcessAttract()
        {


        }

       

        private const int PowerPillAnim = 10;
        private const int GhostAnim = 8;
        private const int PacManAliveAnim = 2;
        private const int PacManDeadAnim = 31 + 7;

        private int TickPosition(int cycleLength, int ticksPerCycle) => (Ticks / ticksPerCycle) % cycleLength;

        private bool TickPosition(int ticksPerCycle) => TickPosition(2, ticksPerCycle) == 0;

        private void DrawSprite(SpriteSource sprite, Location location)
        {
            Display.AddSprite(sprite, location);
        }

        private void DrawBoardSprite(SpriteSource sprite, Location location)
        {
            DrawSprite(sprite, location.Add(0, BoardYOffset));
        }


        private bool _inRender;

        private void ClearScreen()
        {
            Display.Fill(_sprites.Blank);
        }

        private void DrawMap()
        {
            Display.Fill(_sprites.Blank);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Display.Update(_sprites.Map(Map.Cell(x, y).Piece), x, y + BoardYOffset);
                }
            }
        }
    }
}
