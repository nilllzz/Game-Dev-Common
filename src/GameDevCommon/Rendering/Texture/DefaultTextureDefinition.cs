using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering.Texture
{
    public class DefaultTextureDefinition : ITextureDefintion
    {
        private static DefaultTextureDefinition _instance;

        public static DefaultTextureDefinition Instance
            => _instance ?? (_instance = new DefaultTextureDefinition());

        private DefaultTextureDefinition() { }

        public Vector2 Transform(Vector2 normalVector) => normalVector;

        public void NextElement() { }
    }
}
