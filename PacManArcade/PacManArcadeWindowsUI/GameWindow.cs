using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accessibility;
using PacManArcadeGame;
using Timer = System.Windows.Forms.Timer;

namespace PacManArcadeWindowsUI
{
    public partial class GameWindow : Form
    {
        private readonly Timer _tick = new Timer();
        public readonly UiSystem _uiSystem;
        private readonly KeyEvents _keyEvents;
        private readonly BoardRenderer _boardRenderer;

        public GameWindow()
        {
            InitializeComponent();

            _boardRenderer = new BoardRenderer(this);

            Resize += (sender, args) => _boardRenderer.Resize();

            _uiSystem = new UiSystem(_boardRenderer);
            
            _keyEvents = new KeyEvents(_uiSystem.Inputs, this);

            KeyDown += _keyEvents.EventKeyDown;
            KeyUp += _keyEvents.EventKeyUp;

            Text = "PacMan";

      //      Task.Run(Repeat);

            _tick.Interval = 1000 / 65;
            _tick.Tick += ProcessTick;
            _tick.Enabled = true;
        }

        private void ProcessTick(object? sender, EventArgs e)
        {
            _uiSystem.Tick();
        }

        private void Repeat()
        {
            var interval = 1000 / 60;
            var next = DateTime.Now.AddMilliseconds(interval);
            while (true)
            {
                if (DateTime.Now > next)
                {
                    next = DateTime.Now.AddMilliseconds(interval);
                    _uiSystem.Tick();
                }

                Thread.Sleep(1);
            }
        }
    }
}
