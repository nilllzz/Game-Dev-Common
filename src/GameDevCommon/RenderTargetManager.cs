using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon
{
    /// <summary>
    /// Class to manage the render targets assigned to the game's <see cref="GraphicsDevice"/>.
    /// </summary>
    public static class RenderTargetManager
    {
        private static int _width, _height;

        // the default render target for the game.
        public static RenderTarget2D DefaultTarget { get; private set; }

        public static void Initialize(int renderWidth, int renderHeight)
        {
            _width = renderWidth;
            _height = renderHeight;
            DefaultTarget = CreateScreenTarget();
        }

        public static RenderTarget2D CreateScreenTarget()
        {
            return new RenderTarget2D(GameInstanceProvider.Instance.GraphicsDevice, _width, _height, false, default(SurfaceFormat), DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
        }

        public static RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return new RenderTarget2D(GameInstanceProvider.Instance.GraphicsDevice, width, height, false, default(SurfaceFormat), DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
        }

        /// <summary>
        /// Resets the render target to the default.
        /// </summary>
        public static void ResetRenderTarget()
        {
            GameInstanceProvider.Instance.GraphicsDevice.SetRenderTarget(DefaultTarget);
        }

        public static void BeginRenderToTarget(RenderTarget2D target)
        {
            //set to new render target 
            GameInstanceProvider.Instance.GraphicsDevice.SetRenderTarget(target);
        }

        /// <summary>
        /// Ends rendering the screen to a render target.
        /// </summary>
        public static void EndRenderToTarget()
        {
            BeginRenderToTarget(DefaultTarget);
        }

        /// <summary>
        /// Returns the currently active render target.
        /// </summary>
        public static RenderTarget2D CurrentRenderTarget
            => (RenderTarget2D)GameInstanceProvider.Instance.GraphicsDevice.GetRenderTargets()[0].RenderTarget;
    }
}
