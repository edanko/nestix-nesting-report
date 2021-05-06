namespace Report
{
	public class Plate
	{
		public string PrecedingCnc { get; set; }
        //public string StorageName { get; set; }
        public string Quality { get; set; }

		//public string MatProductNo { get; set; }
		public float? Thickness { get; set; }
		public float? Length { get; set; }
		public float? Width { get; set; }
		public float? UsedArea { get; set; }
		public float? Used { get; set; }
		public float? UsedWeight { get; set; }
		public double? MatWeight { get; set; }
		public double? NestGrossWeight { get; set; }
		public int? PlateCount { get; set; }
		//public int? Nxsheetpathid { get; set; }
	}
}
