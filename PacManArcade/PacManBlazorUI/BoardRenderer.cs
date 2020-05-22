using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.JSInterop;
using PacManArcadeGame.Graphics;

namespace PacManBlazorUI
{
    public class BoardRenderer : IRenderer
    {
        public const int CellSize = 8;

        private Stopwatch _fpsStopwatch;

        private int _fpsCounter;
        private int _fps;
        private int _fpsTotal;

        private IJSRuntime _jsRuntime;

        public BoardRenderer(IJSRuntime jsRuntime)
        {
            jsRuntime = _jsRuntime;
            _fpsStopwatch=new Stopwatch();
            _fpsStopwatch.Start();
        }

        private SpriteSource[,] _screenBuffer;

        public void Render(Display display)
        {
            JSData=new List<int>();

            if (_screenBuffer == null)
            {
                _screenBuffer = new SpriteSource[display.Width, display.Height];
            }

            DrawBackground(display);
            DrawSprites(display);

            Height = display.Height*PixelGrid;
            Width = display.Width*PixelGrid;

//            _jsRuntime.InvokeVoidAsync("SpriteSet", display.Width * 8, display.Height * 8, JSData.ToArray());
        }

        public int Height;
        public int Width;
        public List<int> JSData;

        private void DrawBackground(Display display)
        {
            for (var y = 0; y < display.Height; y++)
            {
                for (var x = 0; x < display.Width; x++)
                {
                    var onDisplay = display.Get(x, y);
                    var onBuffer = _screenBuffer[x, y];
                    if (onBuffer == null || onBuffer != onDisplay)
                    {
                        _screenBuffer[x, y] = onDisplay;
                        DrawSprite(true, x, y, onDisplay);
                    }
                }
            }
        }

        public const int PixelGrid = 8;

        private void DrawSprite(bool onBuffer, decimal x, decimal y, SpriteSource source)
        {
            x = x + (1 - source.Size) *0.5m;
            y = y + (1- source.Size) *0.5m;
            var size = PixelGrid * source.Size;
            JSData.AddRange(new[]
            {
                onBuffer ? 1 : 0,
                PixelGrid * source.XPos, PixelGrid * source.YPos, size,
                (int) (PixelGrid * x), (int) (PixelGrid * y)
            });
        }

        private void DrawSprites(Display display)
        {
            foreach (var sprite in display.Sprites)
            {
                DrawSprite(false, sprite.Location.X, sprite.Location.Y, sprite.Sprite);
            }
        }
    }
}
