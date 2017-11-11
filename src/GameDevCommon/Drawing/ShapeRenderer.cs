using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GameDevCommon.Drawing
{
    public static partial class SpriteBatchExtensions
    {
        private static readonly ShapeRenderer _renderer = new ShapeRenderer();

        /// <summary>
        /// Used to render shapes like rectangles and ellipses.
        /// </summary>
        private sealed class ShapeRenderer
        {
            private readonly Texture2D _pixel;

            internal ShapeRenderer()
            {
                // We create a 1,1 pixel large texture here, with a single white pixel.
                // That texture will get stretched and colored when using it to draw a rectangle.
                _pixel = new Texture2D(GameInstanceProvider.Instance.GraphicsDevice, 1, 1);
                _pixel.SetData(new Color[] { Color.White });
            }

            /// <summary>
            /// Draws a rectangle in a given color.
            /// </summary>
            /// <param name="rectangle">The rectangle to draw.</param>
            /// <param name="color">The color of the rectangle.</param>
            internal void DrawRectangle(SpriteBatch batch, Rectangle rectangle, Color color)
            {
                batch.Draw(_pixel, rectangle, color);
            }

            internal void DrawRectangle(SpriteBatch batch, Rectangle rectangle, Color color, float rotation)
            {
                rectangle.Offset(-rectangle.Width / 2f, -rectangle.Height / 2f);
                batch.Draw(_pixel, rectangle, _pixel.Bounds, color, rotation,
                    new Vector2(0.5f / rectangle.Width, 0.5f / rectangle.Height), SpriteEffects.None, 0f);
            }

            /// <summary>
            /// Draws a line in a given color.
            /// </summary>
            /// <param name="start">The starting point of the line.</param>
            /// <param name="end">The ending point of the line.</param>
            /// <param name="color">The color of the line.</param>
            /// <param name="width">The width of the line.</param>
            internal void DrawLine(SpriteBatch batch, Vector2 start, Vector2 end, Color color, double width)
            {
                var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
                double length = Vector2.Distance(start, end);

                batch.Draw(_pixel, start, null, color, (float)angle, Vector2.Zero, new Vector2((float)length, (float)width), SpriteEffects.None, 0);
            }

            private readonly Dictionary<string, GradientConfiguration> _gradientConfigs = new Dictionary<string, GradientConfiguration>();

            /// <summary>
            /// Draws a gradient.
            /// </summary>
            internal void DrawGradient(SpriteBatch batch, Rectangle rectangle, Color fromColor, Color toColor, bool horizontal, double scale = 1D, int steps = -1)
            {
                if (rectangle.Width > 0 && rectangle.Height > 0)
                {
                    GradientConfiguration gradient;
                    var checksum = GradientConfiguration.GenerateChecksum(rectangle.Width, rectangle.Height, fromColor, toColor, horizontal, steps);

                    if (_gradientConfigs.ContainsKey(checksum))
                    {
                        gradient = _gradientConfigs[checksum];
                    }
                    else
                    {
                        gradient = new GradientConfiguration((int)(rectangle.Width / scale), (int)(rectangle.Height / scale), fromColor, toColor, horizontal, steps);
                        _gradientConfigs.Add(checksum, gradient);
                    }

                    // Finally, draw the configuration:
                    gradient.Draw(batch, rectangle);
                }
            }


            // To draw circles, we generate an ellipse texture given a radius for x and y and store those.
            // Once we want to draw an ellipse with that specific radius, we grab the corresponding texture and render it.

            private Dictionary<string, EllipseConfiguration> _ellipseConfigs = new Dictionary<string, EllipseConfiguration>();

            /// <summary>
            /// Draws an ellipse with a specified color.
            /// </summary>
            internal void DrawEllipse(SpriteBatch batch, Rectangle rectangle, Color color, double scale = 1D)
            {
                EllipseConfiguration ellipse;
                var checksum = EllipseConfiguration.GenerateChecksum(rectangle.Width, rectangle.Height);

                if (_ellipseConfigs.ContainsKey(checksum))
                {
                    ellipse = _ellipseConfigs[checksum];
                }
                else
                {
                    ellipse = new EllipseConfiguration((int)Math.Ceiling(rectangle.Width / scale), (int)Math.Ceiling(rectangle.Height / scale));
                    _ellipseConfigs.Add(checksum, ellipse);
                }

                // Finally, draw the configuration:
                ellipse.Draw(batch, rectangle, color);
            }

            // To draw a circle, we draw an ellipse with x and y radius being the same:
            /// <summary>
            /// Draws a circle with specified radius and color.
            /// </summary>
            internal void DrawCircle(SpriteBatch batch, Vector2 position, int radius, Color color, double scale = 1D)
            {
                DrawEllipse(batch, new Rectangle((int)position.X, (int)position.Y, radius, radius), color, scale);
            }
        }
    }
}
