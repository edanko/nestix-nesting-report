namespace NxlReader
{
    public interface IElement
    {
        public Point Start { get; set; } 
        public Point End { get; set; }
        public Point Center { get; set; }
    }
}