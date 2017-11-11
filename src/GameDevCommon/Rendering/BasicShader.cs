using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering
{
    public class BasicShader : Shader
    {
        private BasicEffect BE => (BasicEffect)Effect;

        public BasicShader()
            : base(new BasicEffect(GameInstanceProvider.Instance.GraphicsDevice))
        {
            BE.TextureEnabled = true;
            BE.EnableDefaultLighting();
        }

        protected override void RenderVertices(I3DObject obj)
        {
            BE.Alpha = obj.Alpha;
            BE.Texture = obj.Texture;

            base.RenderVertices(obj);
        }

        public override Matrix World { set => BE.World = value; }
        public override Matrix View { set => BE.View = value; }
        public override Matrix Projection { set => BE.Projection = value; }
    }
}
