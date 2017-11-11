using Microsoft.Xna.Framework;

namespace GameDevCommon
{
    public interface IGame
    {
        Game GetGame();
        ComponentManager GetComponentManager();
    }
}
