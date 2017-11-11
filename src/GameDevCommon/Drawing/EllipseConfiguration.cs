using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Drawing
{
    public static class EllipseHelper
    {
        public struct SimpleEllipse
        {
            public Rectangle Bounds;
            public Color FillColor;
        }

        /// <summary>
        /// Creates a joined texture from multiple ellipses.
        /// </summary>
        /// <param name="outerWidth">The full width of the joined texture.</param>
        /// <param name="outerHeight">The full height of the joined texture.</param>
        /// <param name="ellipses">Ellipse defintion (size and fill color).</param>
        public static Texture2D CreateJoined(int outerWidth, int outerHeight, SimpleEllipse[] ellipses)
        {
            // The objects at the same index in the ellipses and colors arrays are corresponding.

            var colorArr = new Color[outerWidth * outerHeight];
            var returnTexture = new Texture2D(GameInstanceProvider.Instance.GraphicsDevice, outerWidth, outerHeight);

            // By default, the return texture is entirely transparent:
            for (var i = 0; i < colorArr.Length; i++)
            {
                colorArr[i] = Color.Transparent;
            }

            // Loop through the ellipses and fill the color array with the ellipse colors:
            for (var i = 0; i < ellipses.Length; i++)
            {
                var ellipse = ellipses[i];
                var color = ellipses[i].FillColor;

                var ellipseTextureData = EllipseConfiguration.GenerateTextureData(ellipse.Bounds.Width, ellipse.Bounds.Height);

                for (var x = 0; x < ellipse.Bounds.Width; x++)
                {
                    for (var y = 0; y < ellipse.Bounds.Height; y++)
                    {
                        var index = y * ellipse.Bounds.Width + x;
                        var colIndex = (y + ellipse.Bounds.Y) * outerWidth + (x + ellipse.Bounds.X);

                        // Only fill in when the ellipse's color is not transparent:
                        if (ellipseTextureData[index] != Color.Transparent)
                        {
                            colorArr[colIndex] = color;
                        }
                    }
                }
            }

            returnTexture.SetData(colorArr);

            return returnTexture;
        }
    }

    /// <summary>
    /// Used to store ellipse configurations.
    /// </summary>
    internal struct EllipseConfiguration
    {
        private readonly Texture2D _texture;

        internal EllipseConfiguration(int width, int height)
        {
            var texture = new Texture2D(GameInstanceProvider.Instance.GraphicsDevice, width, height);
            texture.SetData(GenerateTextureData(width, height));
            _texture = texture;
        }

        /// <summary>
        /// Renders the ellipse.
        /// </summary>
        internal void Draw(SpriteBatch batch, Rectangle rectangle, Color color)
        {
            batch.Draw(_texture, rectangle, color);
        }

        internal static Color[] GenerateTextureData(int width, int height)
        {
            // width and height are x and y diameter.

            var colorArr = new Color[width * height];

            var center = new Point(width / 2, height / 2);

            var xRadius = width / 2d;
            var yRadius = height / 2d;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var index = y * width + x;

                    var normalized = new Point(x - center.X, y - center.Y);

                    if (((normalized.X * normalized.X) / (xRadius * xRadius)) + ((normalized.Y * normalized.Y) / (yRadius * yRadius)) <= 1.0)
                        colorArr[index] = Color.White;
                    else
                        colorArr[index] = Color.Transparent;
                }
            }

            return colorArr;
        }

        /// <summary>
        /// Generates a checksum for the given variables.
        /// </summary>
        internal static string GenerateChecksum(int width, int height)
        {
            return width.ToString() + "," + height.ToString();
        }
    }
}
