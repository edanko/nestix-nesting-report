using System.Collections.Generic;

namespace Report.NxlReader
{
    public class Remnant
    {
        public Matrix33 Matrix { get; set; }
        public List<Profile> Profiles { get; set; } = new();
        public List<TextProfile> Texts { get; set; } = new();
    }
}