using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameDevCommon.Drawing.Font
{
    [XmlRoot("font")]
    public class FontFile : IDisposable
    {
        [XmlElement("info")]
        public FontInfo Info { get; set; }

        [XmlElement("common")]
        public FontCommon Common { get; set; }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public List<FontPage> Pages { get; set; }

        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public List<FontChar> Chars { get; set; }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public List<FontKerning> Kernings { get; set; }

        public void Dispose()
        {
            Pages?.Clear();
            Chars?.Clear();
            Kernings?.Clear();
        }
    }
}