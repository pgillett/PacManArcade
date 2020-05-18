using PacManArcadeGame.GameItems;

namespace PacManArcadeGame.UiStates
{
    public class DemoMode : Game, IUiMode
    {
        public DemoMode(UiSystem uiSystem, GameSetup.LevelSetup levelSetup) : base(uiSystem, levelSetup, true)
        {
        }
    }
}