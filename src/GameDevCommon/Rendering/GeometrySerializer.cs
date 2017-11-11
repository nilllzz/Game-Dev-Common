using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace GameDevCommon.Rendering
{
    public static class GeometrySerializer
    {
        // format
        // int indexcount
        // int vertexcount
        // int[] <indices>
        // vertex[] <vertices>
        // vertex: (float, float float), (float float, float), (float, float) [Position, Normal, Texture]

        public static void Save(Geometry<VertexPositionNormalTexture> geometry, string file)
        {
            using (var stream = new FileStream(file, FileMode.Create))
            {
                using (var bw = new BinaryWriter(stream))
                {
                    bw.Write(geometry._indices.Count);
                    bw.Write(geometry._vertices.Count);
                    for (int i = 0; i < geometry._indices.Count; i++)
                        bw.Write(geometry._indices[i]);
                    for (int i = 0; i < geometry._vertices.Count; i++)
                    {
                        bw.Write(geometry._vertices[i].Position.X);
                        bw.Write(geometry._vertices[i].Position.Y);
                        bw.Write(geometry._vertices[i].Position.Z);
                        bw.Write(geometry._vertices[i].Normal.X);
                        bw.Write(geometry._vertices[i].Normal.Y);
                        bw.Write(geometry._vertices[i].Normal.Z);
                        bw.Write(geometry._vertices[i].TextureCoordinate.X);
                        bw.Write(geometry._vertices[i].TextureCoordinate.Y);
                    }
                }
            }
        }

        public static void Load(Geometry<VertexPositionNormalTexture> geometry, string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
            {
                using (var br = new BinaryReader(stream))
                {
                    var indexCount = br.ReadInt32();
                    var vertexCount = br.ReadInt32();

                    for (int i = 0; i < indexCount; i++)
                        geometry._indices.Add(br.ReadInt32());
                    for (int i = 0; i < vertexCount; i++)
                    {
                        geometry._vertices.Add(new VertexPositionNormalTexture
                        {
                            Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                            Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                            TextureCoordinate = new Vector2(br.ReadSingle(), br.ReadSingle())
                        });
                    }
                }
            }
        }
    }
}
