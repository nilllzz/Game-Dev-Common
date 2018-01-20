using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering
{
    public interface ICamera
    {
        Matrix View { get; set; }
        Matrix Projection { get; set; }
    }
}
