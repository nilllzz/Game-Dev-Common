using GameDevCommon.Rendering.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace GameDevCommon.Rendering.Composers
{
    public static class TriComposer
    {
        public static VertexPositionNormalTexture[] Create(Vector3 p1, Vector3 p2, Vector3 p3, IGeometryTextureDefintion textureDefinition)
            => Create(new[] { p1, p2, p3 }, textureDefinition);

        public static VertexPositionNormalTexture[] Create(Vector3[] positions, IGeometryTextureDefintion textureDefinition)
        {
            var normal = GetNormal(positions[0], positions[1], positions[2]);

            var triMin = new Vector2(positions.Min(p => p.X), positions.Min(p => p.Z));
            var triMax = new Vector2(positions.Max(p => p.X), positions.Max(p => p.Z));
            var triSize = triMax - triMin;

            Vector2 getTextureCoordinate(Vector3 pos)
            {
                var triPos = new Vector2(pos.X, pos.Z) - triMin;
                return new Vector2(triPos.X / triSize.X, triPos.Y / triSize.Y);
            }

            return positions.Select(p =>
            {
                return new VertexPositionNormalTexture
                {
                    Position = p,
                    Normal = normal,
                    TextureCoordinate = textureDefinition.Transform(getTextureCoordinate(p))
                };
            }).ToArray();
        }

        internal static Vector3 GetNormal(Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            var side1 = pos2 - pos1;
            var side2 = pos3 - pos1;
            var cross = -Vector3.Cross(side1, side2);
            cross.Normalize();
            return cross;
        }
    }
}
