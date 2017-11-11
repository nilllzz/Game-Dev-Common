using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameDevCommon.Drawing
{
    /// <summary>
    /// Used to store gradient configurations.
    /// </summary>
    internal struct GradientConfiguration
    {
        // These exist mainly for performance.
        // In order to draw a gradient, we need to generate a texture that is pieced together from the shades of the gradient.
        // If we don't want to do this every frame we render the gradient, we store the generated texture in a configuration.

        // This where we store the generated texture:
        private readonly Texture2D _texture;

        public GradientConfiguration(int width, int height, Color fromColor, Color toColor, bool horizontal, int steps)
        {
            _texture = GenerateTexture(width, height, fromColor, toColor, horizontal, steps);
        }

        /// <summary>
        /// Renders a rectangle filled with the gradient.
        /// </summary>
        /// <param name="r">The rectangle.</param>
        public void Draw(SpriteBatch batch, Rectangle r)
        {
            batch.Draw(_texture, r, Color.White);
        }

        private static Texture2D GenerateTexture(int width, int height, Color fromColor, Color toColor, bool horizontal, int steps)
        {
            var uSize = height;
            if (horizontal)
                uSize = width;

            double diffR = (int)toColor.R - (int)fromColor.R;
            double diffG = (int)toColor.G - (int)fromColor.G;
            double diffB = (int)toColor.B - (int)fromColor.B;
            double diffA = (int)toColor.A - (int)fromColor.A;

            double stepCount = steps;
            if (stepCount < 0)
                stepCount = uSize;

            var stepSize = (float)Math.Ceiling((float)(uSize / stepCount));

            var colorArr = new Color[width * height];

            for (var cStep = 1; cStep <= stepCount; cStep++)
            {
                var cR = (int)(((diffR / stepCount) * cStep) + (int)fromColor.R);
                var cG = (int)(((diffG / stepCount) * cStep) + (int)fromColor.G);
                var cB = (int)(((diffB / stepCount) * cStep) + (int)fromColor.B);
                var cA = (int)(((diffA / stepCount) * cStep) + (int)fromColor.A);

                if (cR < 0)
                    cR += 255;
                if (cG < 0)
                    cG += 255;
                if (cB < 0)
                    cB += 255;
                if (cA < 0)
                    cA += 255;

                if (horizontal)
                {
                    var c = new Color(cR, cG, cB, cA);

                    var length = (int)Math.Ceiling(stepSize);
                    var start = (int)((cStep - 1) * stepSize);

                    for (var x = start; x < start + length; x++)
                    {
                        for (var y = 0; y < height; y++)
                        {
                            var i = x + y * width;
                            colorArr[i] = c;
                        }
                    }
                }
                else
                {
                    var c = new Color(cR, cG, cB, cA);

                    var length = (int)Math.Ceiling(stepSize);
                    var start = (int)((cStep - 1) * stepSize);

                    for (var y = start; y < start + length; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            var i = x + y * width;
                            colorArr[i] = c;
                        }
                    }
                }
            }

            var texture = new Texture2D(GameInstanceProvider.Instance.GraphicsDevice, width, height);
            texture.SetData(colorArr);
            return texture;
        }

        // Generates a checksum for a specific configuration.
        public static string GenerateChecksum(int width, int height, Color fromColor, Color toColor, bool horizontal, int steps)
        {
            return width.ToString() + "|" + height.ToString() + "|" + fromColor.ToString() + "|" + toColor.ToString() + "|" + horizontal.ToString() + "|" + steps.ToString();
        }
    }
}
