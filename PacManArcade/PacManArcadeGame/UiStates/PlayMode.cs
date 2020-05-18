using PacManArcadeGame.GameItems;

namespace PacManArcadeGame.UiStates
{
    public class PlayMode : Game, IUiMode
    {
        public PlayMode(UiSystem uiSystem, GameSetup.LevelSetup levelSetup) : base(uiSystem, levelSetup, false)
        {
        }
    }
}
