using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Rendering
{
    public interface I3DObject
    {
        VertexBuffer VertexBuffer { get; set; }
        IndexBuffer IndexBuffer { get; set; }
        Matrix World { get; set; }
        bool IsVisible { get; set; }
        BlendState BlendState { get; set; }
        bool IsVisualObject { get; set; }
        float Alpha { get; set; }
        Texture2D Texture { get; set; }
        bool IsOpaque { get; set; }
        bool IsOptimizable { get; set; }
        bool LoadedContent { get; set; }
        object Tag { get; set; }

        void LoadContent();
        void Update();
    }
}
