namespace NxlReader
{
    public class AnnotationLength
    {
        public DimensionLineAnnotation DimensionLineAnnotation { get; set; }
        public ElSource ElSource1 { get; set; }
        public ElSource ElSource2 { get; set; }
        public Line LineLeftContour { get; set; }
        public Line LineRightContour { get; set; }
        public string AnnotationSubType { get; set; }
        public string Distance { get; set; } 
    }
}