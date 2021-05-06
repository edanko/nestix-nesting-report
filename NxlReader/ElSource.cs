using System;
using System.Xml.Linq;

namespace NxlReader
{
    public class ElSource
    {
        public IElement Element { get; set; }
        

        public static ElSource Read(XElement node)
        {
            var e = new ElSource();
        


            foreach (var g in node.Elements())
            {
                switch (g.Name.LocalName)
                {
                    case "Line":

                        e.Element = Line.Read(g);

                        break;

                    case "Arc":

                        e.Element = Arc.Read(g);

                        break;
                    default:
                        Console.WriteLine("unknown elsource element: {0}", node.Name.LocalName);
                        break;
                }
            }

            return e;
        }
    }

    

}