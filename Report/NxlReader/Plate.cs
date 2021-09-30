using System.Collections.Generic;
using System.Xml.Linq;

namespace Report.NxlReader
{
    public class Plate
    {
        public List<Profile> Profiles { get; set; } = new();
        public List<TextProfile> Texts { get; set; } = new();

        public Plate(XElement n)
        {
            Read(n);
        }

        public void Read(XElement n)
        {
            if (n.Element("Profiles") != null)
            {
                foreach (var prof in n.Element("Profiles")?.Elements()!)
                {
                    var p = new Profile();
                    p.Read(prof);
                    Profiles.Add(p);
                }
            }

            if (n.Element("Texts") != null)
            {
                foreach (var node in n.Element("Texts")?.Elements()!)
                {
                    if (node.Name.LocalName == "TextProfile")
                    {
                        var textProfile = TextProfile.Read(node);
                        Texts.Add(textProfile);
                    }
                }
            }
        }
    }
}