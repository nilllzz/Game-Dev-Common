using System.Collections.Generic;
using GameDevCommon.Rendering.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering.Composers
{
    public static class TubeComposer
    {
        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount)
            => Create(radius, height, edgeCount, DefaultGeometryTextureDefinition.Instance);

        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount, IGeometryTextureDefintion textureDefinition)
        {
            var edgePoints = CircleComposer.GetEdgePoints(radius, edgeCount);
            var vertices = new List<VertexPositionNormalTexture>();
            var halfHeight = height / 2f;

            for (int i = 0; i < edgeCount; i++)
            {
                var edgePoint = edgePoints[i];
                var nextEdgePoint = edgePoints[0];
                if (i != edgeCount - 1)
                    nextEdgePoint = edgePoints[i + 1];

                vertices.AddRange(RectangleComposer.Create(new[]
                {
                    new Vector3(-halfHeight, edgePoint.X, edgePoint.Y),
                    new Vector3(halfHeight, edgePoint.X, edgePoint.Y),
                    new Vector3(-halfHeight, nextEdgePoint.X, nextEdgePoint.Y),
                    new Vector3(halfHeight, nextEdgePoint.X, nextEdgePoint.Y),
                }, textureDefinition));

                textureDefinition.NextElement();
            }

            return vertices.ToArray();
        }
    }
}
