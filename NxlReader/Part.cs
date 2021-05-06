using System.Collections.Generic;

namespace NxlReader
{
    public class Part
    {
        public List<IElement> Elements { get; set; }= new List<IElement>();
        public Matrix33 Matrix { get; set; } = new Matrix33();
        public string Annotations { get; set; } 
        public List<TextProfile> Texts { get; set; }  = new List<TextProfile>();
        public TextProfile DetailId { get; set; } = new TextProfile();

        public List<IElement> OrigElements { get; set; }= new List<IElement>();
        public List<TextProfile> OrigTexts { get; set; }  = new List<TextProfile>();


        public string OrderlineInfo { get; set; }
    }
}