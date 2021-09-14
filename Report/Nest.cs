using System.Collections.Generic;

namespace Report
{
    public class Nest
    {
        public int? PartsCount { get; set; }
        public int? Nxpathid { get; set; }
        public int? Nxsheetpathid { get; set; }

        public string NxlFile { get; set; }
        public string EmfImage { get; set; }
        public string Info { get; set; }
        public float? Used { get; set; }
        public string NcName { get; set; }
        public string Machine { get; set; }
        public double? MatWeight { get; set; }
        public double? PartsWeight { get; set; }
        public double? RemnantWeight { get; set; }
        public double? RemnantArea { get; set; }
        public List<Tool> Tools { get; set; }
        public Plate Plate { get; set; }
        public List<Part> Parts { get; set; }
    }
}