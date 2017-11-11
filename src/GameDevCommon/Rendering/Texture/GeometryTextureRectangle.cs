using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering.Texture
{
    public class GeometryTextureRectangle : IGeometryTextureDefintion
    {
        private readonly Vector2 _textureStart, _textureEnd;

        public GeometryTextureRectangle(Rectangle textureRectangle, Rectangle textureBounds)
        {
            _textureStart = new Vector2((float)textureRectangle.Left / textureBounds.Width,
                (float)textureRectangle.Top / textureBounds.Height);
            _textureEnd = new Vector2((float)textureRectangle.Width / textureBounds.Width,
                (float)textureRectangle.Height / textureBounds.Height);
        }

        public GeometryTextureRectangle(Rectangle textureRectangle, Texture2D texture)
             : this(textureRectangle, texture.Bounds)
        { }

        public GeometryTextureRectangle(float x, float y, float width, float height)
        {
            _textureStart = new Vector2(x, y);
            _textureEnd = new Vector2(width, height);
        }

        public Vector2 Transform(Vector2 normalVector)
            => _textureStart + normalVector * _textureEnd;

        public void NextElement() { }
    }
}
