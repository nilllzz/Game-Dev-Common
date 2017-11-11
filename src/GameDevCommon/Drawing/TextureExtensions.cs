using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace GameDevCommon.Drawing
{
    public static class TextureExtensions
    {
        public static Texture2D Slice(this Texture2D texture, Rectangle rectangle)
        {
            var data = texture.GetData(rectangle);
            var newTexture = new Texture2D(texture.GraphicsDevice, rectangle.Width, rectangle.Height);
            newTexture.SetData(data);
            return newTexture;
        }

        public static Color[] GetData(this Texture2D texture)
        {
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            return data;
        }

        public static Color[] GetData(this Texture2D texture, Rectangle rectangle)
        {
            var data = new Color[rectangle.Width * rectangle.Height];
            texture.GetData(0, rectangle, data, 0, data.Length);
            return data;
        }

        public static void Replace(this Texture2D texture, Color oldColor, Color newColor)
        {
            var data = texture.GetData();
            if (data.Contains(oldColor))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == oldColor)
                        data[i] = newColor;
                }
                texture.SetData(data);
            }
        }

        public static Texture2D Clone(this Texture2D texture)
        {
            var newTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height)
            {
                Name = texture.Name
            };

            var data = texture.GetData();
            newTexture.SetData(data);

            return newTexture;
        }
    }
}
