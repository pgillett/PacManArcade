using System;
//using System.Threading;
using System.Windows.Forms;
using PacManArcadeGame.UiStates;

namespace PacManArcadeWindowsUI
{
    public partial class GameWindow : Form
    {
        private readonly Timer _tick = new Timer();
        private readonly UiSystem _uiSystem;

        public GameWindow()
        {
            InitializeComponent();

            var boardRenderer = new BoardRenderer(this);
            Resize += (sender, args) => boardRenderer.Resize();

            _uiSystem = new UiSystem(boardRenderer);
            
            var keyEvents = new KeyEvents(_uiSystem.Inputs);
            KeyDown += keyEvents.EventKeyDown;
            KeyUp += keyEvents.EventKeyUp;

            base.Text = "PacMan";

      //      Task.Run(Repeat);

            _tick.Interval = 1000 / 65;
            _tick.Tick += ProcessTick;
            _tick.Enabled = true;
        }

        private void ProcessTick(object sender, EventArgs e)
        {
            _uiSystem.Tick();
        }

        // Use alternative timer as Forms.Timer is not very accurate at frames level

        //private void Repeat()
        //{
        //    var interval = 1000 / 60;
        //    var next = DateTime.Now.AddMilliseconds(interval);
        //    while (true)
        //    {
        //        if (DateTime.Now > next)
        //        {
        //            next = DateTime.Now.AddMilliseconds(interval);
        //            _uiSystem.Tick();
        //        }

        //        Thread.Sleep(1);
        //    }
        //}
    }
}
