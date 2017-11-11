#region Copyright
//-----------------------------------------------------------------------------
// Copyright (c) 2008-2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameDevCommon.Drawing
{

    /// <summary>
    /// A Gaussian blur filter kernel class.
    /// </summary>
    internal class GaussianBlur : IDisposable
    {
        // A Gaussian blur filter kernel is
        // perfectly symmetrical and linearly separable. This means we can split
        // the full 2D filter kernel matrix into two smaller horizontal and
        // vertical 1D filter kernel matrices and then perform the Gaussian blur
        // in two passes. Contrary to what you might think performing the Gaussian
        // blur in this way is actually faster than performing the Gaussian blur
        // in a single pass using the full 2D filter kernel matrix.
        
        private Effect _effect;
        private SpriteBatch _spriteBatch;
        private int _radius;
        private float _amount, _sigma;
        private float[] _kernel;
        private Vector2[] _offsetsHoriz, _offsetsVert;

        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// This overloaded constructor instructs the GaussianBlur class to
        /// load and use its GaussianBlur.fx effect file that implements the
        /// two pass Gaussian blur operation on the GPU.
        /// </summary>
        internal GaussianBlur(Effect gaussianBlurEffect)
        {
            _effect = gaussianBlurEffect;
            _spriteBatch = new SpriteBatch(GameInstanceProvider.Instance.GraphicsDevice);
        }

        /// <summary>
        /// Calculates the Gaussian blur filter kernel. This implementation is
        /// ported from the original Java code appearing in chapter 16 of
        /// "Filthy Rich Clients: Developing Animated and Graphical Effects for
        /// Desktop Java".
        /// </summary>
        /// <param name="blurRadius">The blur radius in pixels.</param>
        /// <param name="blurAmount">Used to calculate sigma.</param>
        internal void ComputeKernel(int blurRadius, float blurAmount)
        {
            _radius = blurRadius;
            _amount = blurAmount;

            _kernel = null;
            _kernel = new float[_radius * 2 + 1];
            _sigma = _radius / _amount;

            var twoSigmaSquare = 2.0f * _sigma * _sigma;
            var sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
            var total = 0.0f;

            for (var i = -_radius; i <= _radius; ++i)
            {
                float distance = i * i;
                var index = i + _radius;
                _kernel[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += _kernel[index];
            }

            for (var i = 0; i < _kernel.Length; ++i)
                _kernel[i] /= total;
        }

        /// <summary>
        /// Calculates the texture coordinate offsets corresponding to the
        /// calculated Gaussian blur filter kernel. Each of these offset values
        /// are added to the current pixel's texture coordinates in order to
        /// obtain the neighboring texture coordinates that are affected by the
        /// Gaussian blur filter kernel. This implementation has been adapted
        /// from chapter 17 of "Filthy Rich Clients: Developing Animated and
        /// Graphical Effects for Desktop Java".
        /// </summary>
        /// <param name="textureWidth">The texture width in pixels.</param>
        /// <param name="textureHeight">The texture height in pixels.</param>
        internal void ComputeOffsets(float textureWidth, float textureHeight)
        {
            _offsetsHoriz = null;
            _offsetsHoriz = new Vector2[_radius * 2 + 1];

            _offsetsVert = null;
            _offsetsVert = new Vector2[_radius * 2 + 1];

            var xOffset = 1.0f / textureWidth;
            var yOffset = 1.0f / textureHeight;

            for (var i = -_radius; i <= _radius; ++i)
            {
                var index = i + _radius;
                _offsetsHoriz[index] = new Vector2(i * xOffset, 0.0f);
                _offsetsVert[index] = new Vector2(0.0f, i * yOffset);
            }
        }

        /// <summary>
        /// Performs the Gaussian blur operation on the source texture image.
        /// The Gaussian blur is performed in two passes: a horizontal blur
        /// pass followed by a vertical blur pass. The output from the first
        /// pass is rendered to renderTarget1. The output from the second pass
        /// is rendered to renderTarget2. The dimensions of the blurred texture
        /// is therefore equal to the dimensions of renderTarget2.
        /// </summary>
        /// <param name="srcTexture">The source image to blur.</param>
        /// <param name="renderTarget1">Stores the output from the horizontal blur pass.</param>
        /// <param name="renderTarget2">Stores the output from the vertical blur pass.</param>
        /// <returns>The resulting Gaussian blurred image.</returns>
        internal Texture2D PerformGaussianBlur(Texture2D srcTexture,
                                             RenderTarget2D renderTarget1,
                                             RenderTarget2D renderTarget2)
        {
            if (_effect == null)
                throw new InvalidOperationException("GaussianBlur.fx effect not loaded.");

            Texture2D outputTexture = null;
            var srcRect = new Rectangle(0, 0, srcTexture.Width, srcTexture.Height);
            var destRect1 = new Rectangle(0, 0, renderTarget1.Width, renderTarget1.Height);
            var destRect2 = new Rectangle(0, 0, renderTarget2.Width, renderTarget2.Height);

            // Perform horizontal Gaussian blur.

            var currentTarget = RenderTargetManager.CurrentRenderTarget;
            GameInstanceProvider.Instance.GraphicsDevice.SetRenderTarget(renderTarget1);

            _effect.CurrentTechnique = _effect.Techniques["GaussianBlur"];
            _effect.Parameters["weights"].SetValue(_kernel);
            _effect.Parameters["colorMapTexture"].SetValue(srcTexture);
            _effect.Parameters["offsets"].SetValue(_offsetsHoriz);

            _spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            _spriteBatch.Draw(srcTexture, destRect1, Color.White);
            _spriteBatch.End();

            // Perform vertical Gaussian blur.

            GameInstanceProvider.Instance.GraphicsDevice.SetRenderTarget(renderTarget2);
            outputTexture = (Texture2D)renderTarget1;

            _effect.Parameters["colorMapTexture"].SetValue(outputTexture);
            _effect.Parameters["offsets"].SetValue(_offsetsVert);

            _spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            _spriteBatch.Draw(outputTexture, destRect2, Color.White);
            _spriteBatch.End();

            // Return the Gaussian blurred texture.

            GameInstanceProvider.Instance.GraphicsDevice.SetRenderTarget(currentTarget);
            return renderTarget2;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~GaussianBlur()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (_spriteBatch != null && !_spriteBatch.IsDisposed) _spriteBatch.Dispose();
                }

                _effect = null;
                _spriteBatch = null;

                IsDisposed = true;
            }
        }
    }
}
