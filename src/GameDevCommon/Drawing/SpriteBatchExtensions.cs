using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameDevCommon.Drawing
{
    public static partial class SpriteBatchExtensions
    {
        /// <summary>
        /// Begins the SpriteBatch with parameters matching the specified usage.
        /// </summary>
        public static void Begin(this SpriteBatch batch, SpriteBatchUsage usage)
        {
            switch (usage)
            {
                case SpriteBatchUsage.Default:
                    batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    break;
                case SpriteBatchUsage.Font:
                    batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    break;
                case SpriteBatchUsage.RealTransparency:
                    batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    break;
            }
        }

        public static void DrawRectangle(this SpriteBatch batch, Rectangle rectangle, Color color)
            => _renderer.DrawRectangle(batch, rectangle, color);

        public static void DrawRectangle(this SpriteBatch batch, Rectangle rectangle, Color color, float rotation)
            => _renderer.DrawRectangle(batch, rectangle, color, rotation);

        public static void DrawCircle(this SpriteBatch batch, Vector2 position, int radius, Color color, double scale = 1D)
            => _renderer.DrawCircle(batch, position, radius, color, scale);

        public static void DrawEllipse(this SpriteBatch batch, Rectangle rectangle, Color color, double scale = 1D)
            => _renderer.DrawEllipse(batch, rectangle, color, scale);

        public static void DrawGradient(this SpriteBatch batch, Rectangle rectangle, Color fromColor, Color toColor, bool horizontal, double scale = 1D, int steps = -1)
            => _renderer.DrawGradient(batch, rectangle, fromColor, toColor, horizontal, scale, steps);

        public static void DrawLine(this SpriteBatch batch, Vector2 start, Vector2 end, Color color, double width)
            => _renderer.DrawLine(batch, start, end, color, width);

        private static Effect _maskEffect;
        private static DepthStencilState _maskStencil, _textureStencil;

        public static void DrawMask(this SpriteBatch batch, Action drawMask, Action drawTexture)
        {
            if (_maskEffect == null)
            {
                var projection = Matrix.CreateOrthographicOffCenter(0,
                    GameInstanceProvider.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    GameInstanceProvider.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    0, 0, 1);
                _maskEffect = new AlphaTestEffect(GameInstanceProvider.Instance.GraphicsDevice)
                {
                    Projection = projection
                };

                _maskStencil = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Always,
                    StencilPass = StencilOperation.Replace,
                    ReferenceStencil = 1,
                    DepthBufferEnable = false,
                };
                _textureStencil = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.LessEqual,
                    StencilPass = StencilOperation.Keep,
                    ReferenceStencil = 1,
                    DepthBufferEnable = false,
                };
            }

            batch.Begin(SpriteSortMode.Immediate, null, null, _maskStencil, null, _maskEffect);
            drawMask();
            batch.End();

            batch.Begin(SpriteSortMode.Immediate, null, null, _textureStencil, null, _maskEffect);
            drawTexture();
            batch.End();
        }
    }
}
