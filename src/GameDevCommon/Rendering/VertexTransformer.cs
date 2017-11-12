using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering
{
    public static class VertexTransformer
    {
        public static void Offset(VertexPositionNormalTexture[] vertices, Vector3 offset)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Position += offset;
        }

        public static void Rotate(VertexPositionNormalTexture[] vertices, Vector3 rotation)
        {
            var transformation = Matrix.CreateRotationX(rotation.X) *
                Matrix.CreateRotationY(rotation.Y) *
                Matrix.CreateRotationZ(rotation.Z);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position += Vector3.Transform(vertices[i].Position, transformation) - vertices[i].Position;
                vertices[i].Normal += Vector3.Transform(vertices[i].Normal, transformation) - vertices[i].Normal;
            }
        }

        public static void Scale(VertexPositionNormalTexture[] vertices, Vector3 scale)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Position *= scale;
        }

        internal static void TransformToWorld<VertexType>(VertexType[] vertices, Matrix transform)
        {
            var positionField = typeof(VertexType).GetField("Position");
            for (int i = 0; i < vertices.Length; i++)
            {
                var tr = __makeref(vertices[i]);
                positionField.SetValueDirect(tr, Vector3.Transform((Vector3)positionField.GetValueDirect(tr), transform));
            }
        }
    }
}
