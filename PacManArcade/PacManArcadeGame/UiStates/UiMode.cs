namespace PacManArcadeGame.UiStates
{
    public interface IUiMode
    {
        /// <summary>
        /// Execute 1 tick
        /// </summary>
        /// <returns>True if still active</returns>
        bool Tick();
    }
}
