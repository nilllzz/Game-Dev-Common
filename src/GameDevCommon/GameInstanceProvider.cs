using Microsoft.Xna.Framework;

namespace GameDevCommon
{
    public static class GameInstanceProvider
    {
        private static IGame _game;

        public static void SetInstance(IGame game)
        {
            _game = game;
        }

        internal static Game Instance => _game.GetGame();
        internal static IGame IGame => _game;
    }
}
