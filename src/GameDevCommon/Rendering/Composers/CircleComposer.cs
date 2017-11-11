using System;
using System.Collections.Generic;
using System.Linq;
using GameDevCommon.Rendering.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering.Composers
{
    public static class CircleComposer
    {
        internal static Vector2[] GetEdgePoints(float radius, int edgeCount)
        {
            var edgePoints = new Vector2[edgeCount];

            var angle = 0d;
            var anglePerEdge = Math.PI * 2 / edgeCount;

            for (int i = 0; i < edgeCount; i++)
            {
                edgePoints[i] = new Vector2((float)(Math.Cos(angle) * radius), (float)(Math.Sin(angle) * radius));
                angle += anglePerEdge;
            }

            return edgePoints;
        }

        public static VertexPositionNormalTexture[] Create(float radius, int edgeCount)
            => Create(radius, edgeCount, DefaultGeometryTextureDefinition.Instance);
        
        public static VertexPositionNormalTexture[] Create(float radius, int edgeCount, IGeometryTextureDefintion textureDefinition)
        {
            var edgePoints = GetEdgePoints(radius, edgeCount);
            var vertices = new List<VertexPositionNormalTexture>();
            var diameter = radius * 2f;

            for (int i = 0; i < edgeCount; i++)
            {
                var edgePoint = edgePoints[i];
                var nextEdgePoint = edgePoints[0];
                if (i != edgeCount - 1)
                    nextEdgePoint = edgePoints[i + 1];

                vertices.AddRange(new[] { edgePoint, nextEdgePoint, Vector2.Zero }
                    .Select(v => new VertexPositionNormalTexture
                    {
                        Normal = new Vector3(0, 1, 0),
                        Position = new Vector3(v.X, 0f, v.Y),
                        TextureCoordinate = textureDefinition.Transform(new Vector2((v.X + radius) / diameter, (v.Y + radius) / diameter))
                    }));
            }

            return vertices.ToArray();
        }
    }
}
