using System.Collections.Generic;
using GameDevCommon.Rendering.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering.Composers
{
    public static class CuboidComposer
    {
        public static VertexPositionNormalTexture[] Create(float length)
            => Create(length, length, length, DefaultGeometryTextureDefinition.Instance);

        public static VertexPositionNormalTexture[] Create(float length, IGeometryTextureDefintion textureDefinition)
            => Create(length, length, length, textureDefinition);

        public static VertexPositionNormalTexture[] Create(float width, float height, float depth)
            => Create(width, height, depth, DefaultGeometryTextureDefinition.Instance);
        
        public static VertexPositionNormalTexture[] Create(float width, float height, float depth,
            IGeometryTextureDefintion textureDefinition)
        {
            var vertices = new List<VertexPositionNormalTexture>();

            var halfWidth = width / 2f;
            var halfHeight = height / 2f;
            var halfDepth = depth / 2f;
            var front = RectangleComposer.Create(new[]
            {
                new Vector3(halfWidth, halfHeight, -halfDepth),
                new Vector3(-halfWidth, halfHeight, -halfDepth),
                new Vector3(halfWidth, -halfHeight, -halfDepth),
                new Vector3(-halfWidth, -halfHeight, -halfDepth),
            }, textureDefinition);
            textureDefinition.NextElement();

            var back = RectangleComposer.Create(new[]
            {
                new Vector3(-halfWidth, halfHeight, halfDepth),
                new Vector3(halfWidth, halfHeight, halfDepth),
                new Vector3(-halfWidth, -halfHeight, halfDepth),
                new Vector3(halfWidth, -halfHeight, halfDepth),
            }, textureDefinition);
            textureDefinition.NextElement();

            var left = RectangleComposer.Create(new[]
            {
                new Vector3(-halfWidth, halfHeight, -halfDepth),
                new Vector3(-halfWidth, halfHeight, halfDepth),
                new Vector3(-halfWidth, -halfHeight, -halfDepth),
                new Vector3(-halfWidth, -halfHeight, halfDepth),
            }, textureDefinition);
            textureDefinition.NextElement();

            var right = RectangleComposer.Create(new[]
            {
                new Vector3(halfWidth, halfHeight, halfDepth),
                new Vector3(halfWidth, halfHeight, -halfDepth),
                new Vector3(halfWidth, -halfHeight, halfDepth),
                new Vector3(halfWidth, -halfHeight, -halfDepth),
            }, textureDefinition);
            textureDefinition.NextElement();

            var top = RectangleComposer.Create(new[]
            {
                new Vector3(-halfWidth, halfHeight, halfDepth),
                new Vector3(halfWidth, halfHeight, halfDepth),
                new Vector3(-halfWidth, halfHeight, -halfDepth),
                new Vector3(halfWidth, halfHeight, -halfDepth),
            }, textureDefinition);
            textureDefinition.NextElement();

            var bottom = RectangleComposer.Create(new[]
            {
                new Vector3(-halfWidth, -halfHeight, -halfDepth),
                new Vector3(halfWidth, -halfHeight, -halfDepth),
                new Vector3(-halfWidth, -halfHeight, halfDepth),
                new Vector3(halfWidth, -halfHeight, halfDepth),
            }, textureDefinition);

            vertices.AddRange(front);
            vertices.AddRange(back);
            vertices.AddRange(right);
            vertices.AddRange(left);
            vertices.AddRange(top);
            vertices.AddRange(bottom);

            return vertices.ToArray();
        }

        public static VertexPositionNormalTexture[] Create(Vector3[] edges, IGeometryTextureDefintion textureDefinition)
        {
            var tlb = edges[0];
            var tlf = edges[1];
            var trf = edges[2];
            var trb = edges[3];
            var blb = edges[4];
            var blf = edges[5];
            var brf = edges[6];
            var brb = edges[7];
            
            var vertices = new List<VertexPositionNormalTexture>();

            var front = RectangleComposer.Create(new[]
            {
                tlf,
                trf,
                blf,
                brf
            }, textureDefinition);
            textureDefinition.NextElement();
            var back = RectangleComposer.Create(new[]
            {
                trb,
                tlb,
                brb,
                blb
            }, textureDefinition);
            textureDefinition.NextElement();
            var left = RectangleComposer.Create(new[]
            {
                tlb,
                tlf,
                blb,
                blf
            }, textureDefinition);
            textureDefinition.NextElement();
            var right = RectangleComposer.Create(new[]
            {
                trb,
                trf,
                brb,
                brf
            }, textureDefinition);
            textureDefinition.NextElement();
            var top = RectangleComposer.Create(new[]
            {
                tlb,
                trb,
                tlf,
                trf
            }, textureDefinition);
            textureDefinition.NextElement();
            var bottom = RectangleComposer.Create(new[]
            {
                blb,
                brb,
                blf,
                brf
            }, textureDefinition);

            vertices.AddRange(top);
            vertices.AddRange(bottom);
            vertices.AddRange(left);
            vertices.AddRange(right);
            vertices.AddRange(front);
            vertices.AddRange(back);

            return vertices.ToArray();
        }
    }
}
