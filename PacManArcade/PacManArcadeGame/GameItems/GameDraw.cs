using System.Collections.Generic;
using System.Collections.ObjectModel;
using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameItems
{
    public class GameDraw
    {
        private readonly SpriteSet _spriteSet;
        private readonly Display _display;
        private readonly ScoreBoard _scoreBoard;

        public GameDraw(SpriteSet spriteSet, Display display, ScoreBoard scoreBoard)
        {
            _spriteSet = spriteSet;
            _display = display;
            _scoreBoard = scoreBoard;
        }

        public void DisplayReadyText()
        {
            _display.WriteLine("PLAYER ONE", TextColour.Cyan, 9, 14);
            _display.WriteLine("READY!", TextColour.Yellow, 11, 20);
        }

        public void ClearReadyText()
        {
            _display.WriteLine("          ", TextColour.Cyan, 9, 14);
            _display.WriteLine("      ", TextColour.Yellow, 11, 20);
        }

        public void DrawScore(int score, int highScore, bool hide1Up,
            bool gameOver, int credits, int lives,
            ReadOnlyCollection<Fruit> fruitList,
            int mapHeight)
        {
            _scoreBoard.Player1Text(hide1Up);
            _scoreBoard.HighScoreText();
            _scoreBoard.Player1Score(score);
            _scoreBoard.HighScore(highScore);

            if (gameOver)
            {
                _scoreBoard.Credits(credits);
                _display.WriteLine("GAME  OVER", TextColour.Red, 9, 20);
            }
            else
            {
                for (int l = 0; l < lives; l++)
                {
                    DrawBoardSprite(_spriteSet.PacMan(Direction.Left, 1, false),
                        new Location(2.5m + l * 2, mapHeight + 0.5m));
                }

                {
                    var fl = fruitList;
                    for (int i = 0; i < fl.Count; i++)
                    {
                        DrawBoardSprite(_spriteSet.Fruit[fl[i]],
                            new Location(24.5m - 2 * i, mapHeight + 0.5m));
                    }
                }
            }
        }

        public void DrawFruit(BonusFruit bonusFruit)
        {
            if (bonusFruit.ShowAsFruit)
            {
                DrawBoardSprite(_spriteSet.Fruit[bonusFruit.Type], bonusFruit.Location);
            }
            if (bonusFruit.ShowAsScore)
            {
                var set = _spriteSet.BonusScores[bonusFruit.Type];
                var loc = bonusFruit.Location.Add(-(set.Count - 1) * 0.5m, 0);
                for (int x = 0; x < set.Count; x++)
                {
                    DrawBoardSprite(set[x], loc.Add(x, 0));
                }
            }
        }

        public void DrawPowerPills(List<Location> powerPills)
        {
            foreach (var power in powerPills)
            {
                DrawBoardSprite(_spriteSet.PowerPill, power);
            }
        }

        public void DrawPacMan(PacMan pacMan)
        {
            DrawBoardSprite(_spriteSet.PacMan(pacMan), pacMan.Location);
        }

        public void DrawGhosts(IEnumerable<Ghost> ghosts)
        {
            foreach (var g in ghosts)
            {
                DrawBoardSprite(_spriteSet.Ghost(g), g.Location);
            }
        }

        private void DrawScreenSprite(SpriteSource sprite, Location location)
        {
            _display.AddSprite(sprite, location);
        }

        private void DrawBoardSprite(SpriteSource sprite, Location location)
        {
            DrawScreenSprite(sprite, location.Add(0, BoardYOffset));
        }

        public void DrawMap(Map.Map map, bool white)
        {
            _display.Fill(_spriteSet.Blank);
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    _display.Update(_spriteSet.Map(map.Cell(x, y).Piece, white), x, y + BoardYOffset);
                }
            }
        }

        public void Blank()
        {
            _display.Blank();
        }

        public void ClearSprites()
        {
            _display.ClearSprites();
        }

        public void RemovePill(int x, int y)
        {
            _display.Update(_spriteSet.Blank, x, y + BoardYOffset);
        }

        private const int BoardYOffset = 3;


    }
}