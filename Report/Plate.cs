namespace Report
{
    public class Plate
    {
        public string Quality { get; set; }
        public float Thickness { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float UsedWeight { get; set; }
        public double MatWeight { get; set; }
        public double NestGrossWeight { get; set; }

        public override string ToString()
        {
            return $"{Thickness} {Quality} {Length}x{Width}";
        }
    }
}