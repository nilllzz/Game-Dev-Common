using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace GameDevCommon.Rendering
{
    public sealed class Morphed3DObject<VertexType> : I3DObject, IDisposable where VertexType : struct
    {
        private IEnumerable<I3DObject> _objects;

        public Geometry<VertexType> Geometry { get; private set; } = new Geometry<VertexType>();
        public bool IsDisposed { get; private set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public Matrix World { get; set; } = Matrix.Identity;
        public BlendState BlendState { get; set; } = null;
        public bool IsVisible { get; set; } = true;
        public bool IsVisualObject { get; set; } = true;
        public float Alpha { get; set; } = 1f;
        public Texture2D Texture { get; set; } = null;
        public bool IsOpaque { get; set; } = true;
        public bool IsOptimizable { get; set; } = false;
        public bool LoadedContent { get; set; } = false;
        public object Tag { get; set; } = null;

        internal Morphed3DObject(IEnumerable<I3DObject> objects)
        {
            _objects = objects;
            var idObject = objects.ElementAt(0);

            BlendState = idObject.BlendState;
            IsVisible = idObject.IsVisible;
            IsVisualObject = idObject.IsVisualObject;
            Alpha = idObject.Alpha;
            Texture = idObject.Texture;
            IsOpaque = idObject.IsOpaque;
        }

        public void LoadContent()
        {
            // create geometry:
            foreach (var obj in _objects)
            {
                var objVertices = new VertexType[obj.VertexBuffer.VertexCount];
                obj.VertexBuffer.GetData(objVertices);

                if (obj.World != Matrix.Identity)
                    VertexTransformer.TransformToWorld(objVertices, obj.World);

                var objIndices = new int[obj.IndexBuffer.IndexCount];
                obj.IndexBuffer.GetData(objIndices);
                var indexedVertices = objIndices.Select(index => objVertices[index]).ToArray();

                Geometry.AddVertices(indexedVertices);
            }

            var vertices = Geometry.Vertices;
            var indices = Geometry.Indices;

            if (VertexBuffer == null || IndexBuffer == null ||
                VertexBuffer.VertexCount != vertices.Length || IndexBuffer.IndexCount != indices.Length)
            {
                VertexBuffer = new VertexBuffer(GameInstanceProvider.Instance.GraphicsDevice, Base3DObject<VertexType>.GetVertexDeclaration(), vertices.Length, BufferUsage.WriteOnly);
                IndexBuffer = new IndexBuffer(GameInstanceProvider.Instance.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            }

            VertexBuffer.SetData(vertices);
            IndexBuffer.SetData(indices);

            LoadedContent = true;
        }

        public void Update() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Morphed3DObject()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (VertexBuffer != null && !VertexBuffer.IsDisposed) VertexBuffer.Dispose();
                    if (IndexBuffer != null && !IndexBuffer.IsDisposed) IndexBuffer.Dispose();
                    if (Geometry != null && !Geometry.IsDisposed) Geometry.Dispose();
                }

                VertexBuffer = null;
                IndexBuffer = null;
                Geometry = null;
                _objects = null;

                IsDisposed = true;
            }
        }
    }
}
