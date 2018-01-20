using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameDevCommon.Rendering
{
    public abstract class Shader : IDisposable
    {
        protected PrimitiveType _primitiveType = PrimitiveType.TriangleList;

        public Effect Effect { get; private set; }
        public bool IsDisposed { get; private set; }
        public abstract Matrix World { set; }
        public abstract Matrix View { set; }
        public abstract Matrix Projection { set; }

        public Shader(Effect effect)
        {
            Effect = effect;
            Effect.CurrentTechnique = Effect.Techniques[0];
        }

        public Shader(Effect effect, string defaultTechnique)
        {
            Effect = effect;
            Effect.CurrentTechnique = Effect.Techniques[defaultTechnique];
        }

        public virtual void Prepare(ICamera camera)
        {
            View = camera.View;
            Projection = camera.Projection;
        }

        public virtual void Render(I3DObject obj)
        {
            if (obj.IsVisible && obj.IsVisualObject) {
                if (obj.BlendState != null && obj.BlendState.Name != GameInstanceProvider.Instance.GraphicsDevice.BlendState.Name)
                    GameInstanceProvider.Instance.GraphicsDevice.BlendState = obj.BlendState;
                else
                    GameInstanceProvider.Instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                World = obj.World;
                RenderVertices(obj);
            }
        }

        public virtual void ApplyPass(I3DObject obj)
        {
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        protected virtual void RenderVertices(I3DObject obj)
        {
            ApplyPass(obj);

            GameInstanceProvider.Instance.GraphicsDevice.Indices = obj.IndexBuffer;
            GameInstanceProvider.Instance.GraphicsDevice.SetVertexBuffer(obj.VertexBuffer);
            GameInstanceProvider.Instance.GraphicsDevice.DrawIndexedPrimitives(_primitiveType, 0, 0, obj.IndexBuffer.IndexCount / 3);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed) {
                if (disposing) {
                    if (Effect != null && !Effect.IsDisposed) Effect.Dispose();
                }

                Effect = null;

                IsDisposed = true;
            }
        }
    }
}
