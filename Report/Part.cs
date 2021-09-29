namespace Report
{
    public class Part
    {
        public int DetailCode { get; set; }
        public string Project { get; set; }
        public string Section { get; set; }
        public string Pos { get; set; }
        public int DetailCount { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Weight { get; set; }

        public override string ToString()
        {
            return $"{DetailCode:D3}: prj:{Project} sec:{Section} pos:{Pos}";
        }
    }
}