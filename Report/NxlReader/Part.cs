using System.Collections.Generic;

namespace Report.NxlReader
{
    public class Part
    {
        public List<Profile> Profiles { get; set; } = new();
        public Matrix33 Matrix { get; set; } = new();
        public List<TextProfile> Texts { get; set; } = new();
        public TextProfile DetailId { get; set; } = new();
        public string OrderlineInfo { get; set; }

        public Part DeepCopy()
        {
            return FastClone.Cloner.Clone(this);
        }
    }
}