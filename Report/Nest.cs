using System.Collections.Generic;
using System.Linq;

namespace Report
{
    public class Nest
    {
        public int Nxpathid { get; set; }
        public int Nxsheetpathid { get; set; }
        public string NcName { get; set; }
        public string Machine { get; set; }
        public string Info { get; set; }
        public float Used { get; set; }
        public string NxlFile { get; set; }
        public string EmfImage { get; set; }
        public double RemnantWeight { get; set; }
        public double RemnantArea { get; set; }
        public List<Tool> Tools { get; set; }
        public Plate Plate { get; set; }
        public List<Part> Parts { get; set; }

        public override string ToString()
        {
            return $"id: {Nxpathid}, name: {NcName}, parts: {Parts.Count}";
        }

        public double PartsWeight()
        {
            return Parts.Sum(x => x.Weight);
        }
    }


}