using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering.Texture
{
    public interface IGeometryTextureDefintion
    {
        void NextElement();
        Vector2 Transform(Vector2 normalVector);
    }
}
