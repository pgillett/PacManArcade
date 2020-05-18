using System.Reflection;

namespace PacManArcadeGame.GameItems
{
    public enum GameState
    {
        Intro,
        GetReady, 
        Playing,
        Frightened,
        Caught,
        Dying,
        GameOver,
        Complete,
        Flash
    }

    public class GameState2
    {
        public bool Intro => Current == GameState.Intro;
        public bool GetReady => Current == GameState.GetReady;
        public GameState Current;
        public GameState Last;

        public GameState2()
        {

        }

        public bool Changed
        {
            get
            {
                var c = Last != Current;
                Last = Current;
                return c;
            }
        }
    }
}