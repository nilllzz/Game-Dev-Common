using GameDevCommon.Rendering.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameDevCommon.Rendering.Composers
{
    public static class PyramidComposer
    {
        private static IGeometryTextureDefintion[] Repeat(IGeometryTextureDefintion textureDefinition, int amount)
        {
            var arr = new IGeometryTextureDefintion[amount];
            for (int i = 0; i < amount; i++)
                arr[i] = textureDefinition;
            return arr;
        }

        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount, int levels)
            => Create(radius, height, edgeCount, levels, Repeat(DefaultGeometryTextureDefinition.Instance, levels));

        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount, int levels, IGeometryTextureDefintion textureDefinition)
            => Create(radius, height, edgeCount, levels, Repeat(textureDefinition, levels));

        public static VertexPositionNormalTexture[] Create(float radius, float height, int edgeCount, int levels,
            params IGeometryTextureDefintion[] textureDefinitions)
        {
            var levelHeight = height / levels;
            var halfHeight = levelHeight / 2f;
            var vertices = new List<VertexPositionNormalTexture>();

            for (int level = 0; level < levels; level++)
            {
                var levelRadius = radius - (radius / levels * (level));
                var levelRadiusUp = radius - (radius / levels * (level + 1));

                var edgePoints = CircleComposer.GetEdgePoints(levelRadius, edgeCount);
                var edgePointsUp = CircleComposer.GetEdgePoints(levelRadiusUp, edgeCount);
                var textureDefinition = textureDefinitions[level];

                for (int i = 0; i < edgeCount; i++)
                {
                    var edgePoint = edgePoints[i];
                    var nextEdgePoint = edgePoints[0];
                    var edgePointUp = edgePointsUp[i];
                    var nextEdgePointUp = edgePointsUp[0];
                    if (i != edgeCount - 1)
                    {
                        nextEdgePoint = edgePoints[i + 1];
                        nextEdgePointUp = edgePointsUp[i + 1];
                    }
                    var levelHeightPos = levelHeight * level;

                    vertices.AddRange(RectangleComposer.Create(new[]
                    {
                        new Vector3(levelHeightPos - halfHeight, edgePoint.X, edgePoint.Y),
                        new Vector3(levelHeightPos + halfHeight, edgePointUp.X, edgePointUp.Y),
                        new Vector3(levelHeightPos - halfHeight, nextEdgePoint.X, nextEdgePoint.Y),
                        new Vector3(levelHeightPos + halfHeight, nextEdgePointUp.X, nextEdgePointUp.Y),
                    }, textureDefinition));

                    textureDefinition.NextElement();
                }
            }
            return vertices.ToArray();
        }
    }
}
