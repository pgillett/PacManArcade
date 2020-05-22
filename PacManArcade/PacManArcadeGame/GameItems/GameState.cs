using System.Reflection;

namespace PacManArcadeGame.GameItems
{
    public enum GameState
    {
        Initialise,
        Intro,
        StartOfLife,
        GetReady, 
        Playing,
        Frightened,
        Caught,
        Dying,
        GameOver,
        Complete,
        Flash,
        NewLevel
    }
}