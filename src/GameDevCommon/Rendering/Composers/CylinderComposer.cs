using System.Collections.Generic;
using GameDevCommon.Rendering.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering.Composers
{
    public static class CylinderComposer
    {
        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount)
            => Create(radius, height, edgeCount, DefaultTextureDefinition.Instance, DefaultTextureDefinition.Instance);

        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount,
            ITextureDefintion sideTexture, ITextureDefintion endTexture)
            => Create(radius, height, edgeCount, sideTexture, endTexture, endTexture);

        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount,
            ITextureDefintion sideTexture, ITextureDefintion endTexture1, ITextureDefintion endTexture2)
        {
            var vertices = new List<VertexPositionNormalTexture>();

            var sides = TubeComposer.Create(radius, height, edgeCount, sideTexture);
            var end1 = CircleComposer.Create(radius, edgeCount, endTexture1);
            var end2 = CircleComposer.Create(radius, edgeCount, endTexture2);

            VertexTransformer.Rotate(end1, new Vector3(0, 0, MathHelper.PiOver2));
            VertexTransformer.Offset(end1, new Vector3(height / 2f, 0, 0));
            VertexTransformer.Rotate(end2, new Vector3(0, 0, MathHelper.PiOver2));
            VertexTransformer.Offset(end2, new Vector3(-height / 2f, 0, 0));

            vertices.AddRange(sides);
            vertices.AddRange(end1);
            vertices.AddRange(end2);

            return vertices.ToArray();
        }
    }
}
