using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering
{
    public abstract class Base3DObject<VertexType> : I3DObject, IDisposable where VertexType : struct
    {
        private const string FIELD_NAME_VERTEXDECLARATION = "VertexDeclaration";

        protected bool _dynamicBuffers = false;

        public Geometry<VertexType> Geometry { get; protected set; } = new Geometry<VertexType>();
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
        public bool IsOptimizable { get; set; } = true;
        public bool LoadedContent { get; set; } = false;
        public object Tag { get; set; } = null;

        internal static VertexDeclaration GetVertexDeclaration()
            => (VertexDeclaration)typeof(VertexType).GetField(FIELD_NAME_VERTEXDECLARATION).GetValue(null);

        public virtual void Update() { }

        public virtual void LoadContent()
        {
            CreateWorld();
            CreateGeometry();

            if (IsVisualObject)
                CreateBuffers();

            LoadedContent = true;
        }

        protected virtual void CreateWorld() { }

        protected virtual void CreateGeometry() { }

        protected void CreateBuffers()
        {
            var vertices = Geometry.Vertices;
            var indices = Geometry.Indices;

            if (VertexBuffer == null || IndexBuffer == null ||
                VertexBuffer.VertexCount != vertices.Length || IndexBuffer.IndexCount != indices.Length)
            {
                if (_dynamicBuffers)
                {
                    VertexBuffer = new DynamicVertexBuffer(GameInstanceProvider.Instance.GraphicsDevice, GetVertexDeclaration(), vertices.Length, BufferUsage.WriteOnly);
                    IndexBuffer = new DynamicIndexBuffer(GameInstanceProvider.Instance.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
                }
                else
                {
                    VertexBuffer = new VertexBuffer(GameInstanceProvider.Instance.GraphicsDevice, GetVertexDeclaration(), vertices.Length, BufferUsage.None);
                    IndexBuffer = new IndexBuffer(GameInstanceProvider.Instance.GraphicsDevice, typeof(int), indices.Length, BufferUsage.None);
                }
            }

            VertexBuffer.SetData(vertices);
            IndexBuffer.SetData(indices);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Base3DObject()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
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

                IsDisposed = true;
            }
        }
    }
}
