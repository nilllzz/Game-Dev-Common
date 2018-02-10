using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering.Texture
{
    public class TextureMultiplier : ITextureDefintion
    {
        private readonly Vector2 _multiplier;

        public TextureMultiplier(float multiplier)
        {
            _multiplier = new Vector2(multiplier);
        }

        public TextureMultiplier(Vector2 multiplier)
        {
            _multiplier = multiplier;
        }

        public Vector2 Transform(Vector2 normalVector)
            => normalVector * _multiplier;

        public void NextElement() { }
    }
}
