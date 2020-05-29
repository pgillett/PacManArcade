namespace PacManArcadeGame.GameItems
{
    public class SpeedCounter
    {
        public int _counter;
        private int _frequency;

        private int _current;
        private SpeedType _type;

        public void SetSpeed(int speedPercent)
        {
            if (_current != speedPercent)
            {
                if (speedPercent == 80)
                {
                    _type = SpeedType.Fixed;
                }
                else if(speedPercent<80)
                {
                    _type = SpeedType.Skip;
                    _frequency = 80 / (80 - speedPercent);
                }
                else
                {
                    _type = SpeedType.Extra;
                    _frequency = 80 / (speedPercent - 80);
                }

                _current = speedPercent;
            }
        }

        public void Tick()
        {
            _counter++;
        }

        public bool SkipFrame => _type == SpeedType.Skip && _counter % _frequency == 0;
        public bool ExtraFrame => _type == SpeedType.Extra && _counter % _frequency == 0;

        public enum SpeedType
        {
            Fixed,
            Skip,
            Extra
        }
    }
}