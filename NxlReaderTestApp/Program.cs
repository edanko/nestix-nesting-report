using System.IO;
using System.Linq;
using NxlReader;
using NxlReader.Drawer;

namespace NxlReaderTestApp
{
    static class Program
    {
        static void Main()
        {
            //const string filename = @"E:\work\файлы для шаблона карты раскроя\nxl\19634.nest.nxl";// 19635 1000 225 20083 20705
            const string filename = @"E:\work\файлы для шаблона карты раскроя\nxl\20083.nest.nxl";// 19635 1000 225 20083 20705
            //const string filename = @"E:\work\файлы для шаблона карты раскроя\nxl\18727.nest.nxl";// 19635 1000 225 20083 20705
            //const string filename = @"E:\work\файлы для шаблона карты раскроя\nxl\4726.nest.nxl";// 19635 1000 225 20083 20705

            var n = new Nest();
            n.Read(filename);

            var svg = Drawer.DrawNest(n);

            File.WriteAllText(@"C:\users\egor\desktop\foo.svg",svg);

        }
    }
}
