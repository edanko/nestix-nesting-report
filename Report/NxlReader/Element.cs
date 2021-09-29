namespace NxlReader
{
    public interface IElement
    {
        Point Start { get; set; }
        Point End { get; set; }
        Point Center { get; set; }
    }
}