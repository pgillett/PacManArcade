using System.Windows.Forms;
using PacManArcadeGame;

namespace PacManArcadeWindowsUI
{
    public class KeyEvents
    {
        private readonly Inputs _inputs;

        public KeyEvents(Inputs inputs, Form form)
        {
            _inputs = inputs;
        }

        public void EventKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    _inputs.Direction = InputDirection.Up;
                    _lastKey = e.KeyCode;
                    break;
                case Keys.Down:
                    _inputs.Direction = InputDirection.Down;
                    _lastKey = e.KeyCode;
                    break;
                case Keys.Left:
                    _inputs.Direction = InputDirection.Left;
                    _lastKey = e.KeyCode;
                    break;
                case Keys.Right:
                    _inputs.Direction = InputDirection.Right;
                    _lastKey = e.KeyCode;
                    break;
                case Keys.C:
                    _inputs.Coin = true;
                    break;
                case Keys.D1:
                    _inputs.Player1Start = true;
                    break;
                case Keys.R:
                    _inputs.Reset = true;
                    break;
                default:
                    break;
            }
        }

        private Keys _lastKey;

        public void EventKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == _lastKey)
            {
                _inputs.Direction = InputDirection.None;
            }
        }
    }
}