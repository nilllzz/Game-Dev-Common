using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevCommon.Drawing.Font
{
    public class FontRenderer : IDisposable
    {
        private const string FontXmlDataFileExtension = "fnt";

        private FontFile FontFile { get; }
        private Texture2D[] Textures { get; }

        private Dictionary<char, FontChar> GlyphMap { get; } = new Dictionary<char, FontChar>();
        private float FontScale { get; }

        private static string GetDefaultFontFolder()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Fonts");
        }

        public FontRenderer(string fileName)
            : this(GetDefaultFontFolder(), fileName)
        { }

        public FontRenderer(string fontFolder, string fileName)
        {
            #region File

            var deserializer = new XmlSerializer(typeof(FontFile));

            if (File.Exists(Path.Combine(fontFolder, $"{fileName}.{FontXmlDataFileExtension}")))
                using (var stream = File.OpenRead(Path.Combine(fontFolder, $"{fileName}.{FontXmlDataFileExtension}")))
                using (var textReader = new StreamReader(stream))
                    FontFile = (FontFile)deserializer.Deserialize(textReader);

            #endregion File


            #region Texture

            Textures = new Texture2D[FontFile.Pages.Count];
            foreach (var fontPage in FontFile.Pages) {
                using (var stream = File.OpenRead(Path.Combine(fontFolder, fontPage.File)))
                    Textures[fontPage.ID] = Texture2D.FromStream(GameInstanceProvider.Instance.GraphicsDevice, stream);
            }

            #endregion Texture


            foreach (var glyph in FontFile.Chars)
                GlyphMap.Add((char)glyph.ID, glyph);

            FontScale = FontFile.Info.Size / 16f;
            //FontScale = FontFile.Info.Size / 8f;
            if (FontScale < 0)
                FontScale *= -1f;

        }

        public Vector2 MeasureText(string text)
        {
            var width = 0;
            var height = 0;
            foreach (var c in text) {
                if (GlyphMap.TryGetValue(c, out FontChar fc)) {
                    width += fc.XAdvance + fc.XOffset;

                    if (fc.Height + fc.YOffset > height)
                        height = fc.Height + fc.YOffset;
                }
            }

            return new Vector2(width, height);
        }
        private float ScaleToMin(float value)
        {
            return (float)(Math.Round(value * FontScale, MidpointRounding.ToEven) / FontScale);
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale = ScaleToMin(min);

            var dx = borders.X;
            var dy = borders.Y;

            foreach (var c in text) {
                if (GlyphMap.TryGetValue(c, out FontChar fc)) {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int)(fc.XAdvance * scale);// + FontScale * scale);
                }
            }
        }
        public void DrawTextStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.X;
            var dy = borders.Y;

            foreach (var c in text) {
                if (GlyphMap.TryGetValue(c, out FontChar fc)) {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int)(fc.XAdvance * scale + FontScale * scale);
                }
            }
        }

        public void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale = ScaleToMin(min);

            var dx = borders.Center.X - (int)(size.X * scale * 0.5f) - text.Length;
            var dy = borders.Y;

            foreach (var c in text) {
                if (GlyphMap.TryGetValue(c, out FontChar fc)) {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int)(fc.XAdvance * scale);// + FontScale * scale);
                }
            }
        }
        public void DrawTextCenteredStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.Center.X - (int)(size.X * scale * 0.5f) - (int)(2 * text.Length * 0.5f);
            var dy = borders.Y;

            foreach (var c in text) {
                if (GlyphMap.TryGetValue(c, out FontChar fc)) {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int)(fc.XAdvance * scale + FontScale * scale);
                }
            }
        }

        public void Dispose()
        {
            GlyphMap?.Clear();

            FontFile?.Dispose();

            if (Textures != null)
                foreach (var texture in Textures)
                    texture.Dispose();
        }
    }
}