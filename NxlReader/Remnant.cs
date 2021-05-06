using System.Collections.Generic;

namespace NxlReader
{
    public class Remnant
    {
        public string OriginalSheet { get; set; }
        public Matrix33 Matrix { get; set; }
        public List<IElement> Elements { get; set; } = new List<IElement>();
        public List<TextProfile> Texts { get; set; } = new List<TextProfile>();
    }
}