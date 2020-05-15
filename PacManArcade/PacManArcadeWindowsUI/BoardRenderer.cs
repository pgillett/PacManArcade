using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using PacManArcadeGame;
using PacManArcadeGame.Graphics;

namespace PacManArcadeWindowsUI
{
    public class BoardRenderer : IRenderer
    {
        public const int CellSize = 8;

        private Form _form;

        private Bitmap _sprites;

        private Bitmap _staticBoard;
        private Graphics _staticBoardGraphics;

        private Bitmap _gameBuffer;
        private Graphics _gameBufferGraphics;

        private Stopwatch _fpsStopwatch;

        private BufferedGraphicsContext _bufferedGraphicsContext;
        private BufferedGraphics _bufferedGraphics;
        private Graphics _screenGraphics;

        public BoardRenderer(Form form)
        {
            _form = form;
            _sprites = new Bitmap("gfx.png");
            _fpsStopwatch=new Stopwatch();
            _fpsStopwatch.Start();
        }

        private SpriteSource[,] _screenBuffer;

        public void Render(Display display)
        {
            if (_staticBoard == null) Setup(display.Width, display.Height);

            UpdateBackground(display);
            StartRedraw();
            DrawSprites(display);
            BufferToScreen();
        }

        private void UpdateBackground(Display display)
        {
            for (var y = 0; y < display.Height; y++)
            {
                for (var x = 0; x < display.Width; x++)
                {
                    var d = display.Get(x, y);
                    var b = _screenBuffer[x, y];
                    if (b == null || b != d)
                    {
                        _screenBuffer[x, y] = d;
                        DrawToBackground(d, x, y);
                    }
                }
            }
        }

        private void DrawSprites(Display display)
        {
            foreach (var sprite in display.Sprites)
            {
                DrawToBuffer(sprite.Sprite, sprite.Location.X, sprite.Location.Y);
            }
        }

        public void Setup(int width, int height)
        {
            _staticBoard?.Dispose();
            _staticBoardGraphics?.Dispose();
            _gameBuffer?.Dispose();
            _gameBufferGraphics?.Dispose();

            _staticBoard = new Bitmap(width * CellSize, height * CellSize);
            _staticBoardGraphics = Graphics.FromImage(_staticBoard);
            _gameBuffer = new Bitmap(_staticBoard.Width, _staticBoard.Height);
            _gameBufferGraphics = Graphics.FromImage(_gameBuffer);

            _staticBoardGraphics.SmoothingMode = SmoothingMode.None;
            _staticBoardGraphics.InterpolationMode = InterpolationMode.Low;

            _gameBufferGraphics.SmoothingMode = SmoothingMode.None;
            _gameBufferGraphics.InterpolationMode = InterpolationMode.Low;

            _screenBuffer = new SpriteSource[width, height];

            Resize();
        }

        public void Resize()
        {
            _bufferedGraphics?.Dispose();
            _bufferedGraphicsContext?.Dispose();

            _bufferedGraphicsContext = BufferedGraphicsManager.Current;
            _bufferedGraphics = _bufferedGraphicsContext.Allocate(_form.CreateGraphics(), _form.DisplayRectangle);
            _screenGraphics = _bufferedGraphics.Graphics;

            _screenGraphics.SmoothingMode = SmoothingMode.None;
            _screenGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        private void DrawToBackground(SpriteSource sprite, decimal toCellX, decimal toCellY)
        {
            DrawToContext(_staticBoardGraphics, sprite, toCellX, toCellY);
        }

        private void StartRedraw()
        {
            _gameBufferGraphics.DrawImage(_staticBoard, 0, 0);
        }

        private void DrawToBuffer(SpriteSource sprite, decimal toCellX, decimal toCellY)
        {
            DrawToContext(_gameBufferGraphics, sprite, toCellX, toCellY);
        }

        private void DrawToContext(Graphics g, SpriteSource sprite, decimal toCellX, decimal toCellY)
        {
            var x = (int) ((toCellX + 0.5m * (1 - sprite.Size)) * CellSize);
            var y = (int) ((toCellY + 0.5m * (1 - sprite.Size)) * CellSize);
            var size = CellSize * sprite.Size;
            g.DrawImage(_sprites, new Rectangle(x, y, size, size),
                CellSize * sprite.XPos, CellSize * sprite.YPos, size, size,
                GraphicsUnit.Pixel);
        }

        private void BufferToScreen()
        {
            var scale = Math.Min((float) _form.ClientSize.Width / _gameBuffer.Width,
                (float) _form.ClientSize.Height / _gameBuffer.Height);
            var x = (_form.ClientSize.Width - _gameBuffer.Width * scale) / 2;
            var y = (_form.ClientSize.Height - _gameBuffer.Height * scale) / 2;
            _screenGraphics.DrawImage(_gameBuffer,
                new RectangleF(x, y, _gameBuffer.Width * scale, _gameBuffer.Height * scale),
                new RectangleF(0, 0, _gameBuffer.Width, _gameBuffer.Height),
                GraphicsUnit.Pixel);

            FPS(_screenGraphics);

            _bufferedGraphics.Render();
        }

        private int _fpsCounter;
        private int _fps;
        private int _fpsTotal;
        private readonly Font _scoreFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        private readonly Brush _fpsBrush = new SolidBrush(Color.DarkRed);

        private void FPS(Graphics g)
        {
            g.FillRectangle(_fpsBrush, 0, 0, 50, 20);
            g.DrawString($"{_fps} {_fpsTotal}", _scoreFont, Brushes.White, 0, 0);
            _fpsCounter++;
            _fpsTotal++;
            if (_fpsStopwatch.ElapsedMilliseconds > 1000)
            {
                _fps = _fpsCounter;
                _fpsStopwatch.Restart();
                _fpsCounter = 0;
            }
        }
    }
}
