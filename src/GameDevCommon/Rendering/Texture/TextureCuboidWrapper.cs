using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering.Texture
{
    public class TextureCuboidWrapper : ITextureDefintion
    {
        private Dictionary<CuboidSide, ITextureDefintion> _definitions;
        private int _currentSideIndex = 1;

        public TextureCuboidWrapper()
        {
            _definitions = new Dictionary<CuboidSide, ITextureDefintion>();
        }

        public TextureCuboidWrapper(Dictionary<CuboidSide, Rectangle> definitions, Rectangle textureBounds)
        {
            _definitions = definitions.ToDictionary(
                p => p.Key,
                p => new TextureRectangle(p.Value, textureBounds) as ITextureDefintion);
        }

        public void AddSide(CuboidSide side, ITextureDefintion textureDefinition)
        {
            _definitions.Add(side, textureDefinition);
        }

        public void AddSide(CuboidSide[] sides, ITextureDefintion textureDefinition)
        {
            foreach (var side in sides)
                _definitions.Add(side, textureDefinition);
        }

        public void NextElement()
        {
            _currentSideIndex++;
        }

        public void Reset()
        {
            _currentSideIndex = 1;
        }

        public Vector2 Transform(Vector2 normalVector)
        {
            var currentSide = (CuboidSide)_currentSideIndex;
            if (_definitions.ContainsKey(currentSide))
                return _definitions[currentSide].Transform(normalVector);
            else
                return normalVector;
        }
    }
}
