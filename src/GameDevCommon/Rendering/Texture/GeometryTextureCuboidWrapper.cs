using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering.Texture
{
    public class GeometryTextureCuboidWrapper : IGeometryTextureDefintion
    {
        private Dictionary<CuboidSide, IGeometryTextureDefintion> _definitions;
        private int _currentSideIndex = 1;

        public GeometryTextureCuboidWrapper()
        {
            _definitions = new Dictionary<CuboidSide, IGeometryTextureDefintion>();
        }

        public GeometryTextureCuboidWrapper(Dictionary<CuboidSide, Rectangle> definitions, Rectangle textureBounds)
        {
            _definitions = definitions.ToDictionary(
                p => p.Key, 
                p => new GeometryTextureRectangle(p.Value, textureBounds) as IGeometryTextureDefintion);
        }

        public void AddSide(CuboidSide side, IGeometryTextureDefintion textureDefinition)
        {
            _definitions.Add(side, textureDefinition);
        }

        public void AddSide(CuboidSide[] sides, IGeometryTextureDefintion textureDefinition)
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
