using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Reflection;

namespace GameDevCommon.Drawing
{
    public static class GraphicsDeviceHelper
    {
        private static SamplerState _pointFilterSampleState = new SamplerState
        {
            Filter = TextureFilter.Point
        };

        public static void ClearFull(this GraphicsDevice graphicsDevice, Color color)
        {
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, color, 1.0f, 0);
        }

        public static void ResetFull(this GraphicsDevice graphicsDevice)
        {
            graphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
            graphicsDevice.SamplerStates[0] = _pointFilterSampleState;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public static void SetGraphicsProfile(this GraphicsDevice graphicsDevice, GraphicsProfile profile)
        { 
            // HACK
            // removes chromosomes
            var fields = typeof(GraphicsDevice).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            fields.First(f => f.Name == "_graphicsProfile").SetValue(graphicsDevice, GraphicsProfile.HiDef);
        }
    }
}
