using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering.Texture
{
    public class GeometryTextureMultiplier : IGeometryTextureDefintion
    {
        private readonly Vector2 _multiplier;

        public GeometryTextureMultiplier(float multiplier)
        {
            _multiplier = new Vector2(multiplier);
        }

        public GeometryTextureMultiplier(Vector2 multiplier)
        {
            _multiplier = multiplier;
        }

        public Vector2 Transform(Vector2 normalVector)
            => normalVector * _multiplier;

        public void NextElement() { }
    }
}
