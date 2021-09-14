using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace NxlReader.Drawer
{
    public static class Dxf
    {
        public static void Draw(string filename, Nest n)
        {
            var dxfFile = new DxfFile();
            dxfFile.Entities.Add(new DxfLine(new DxfPoint(0, 0, 0), new DxfPoint(50, 50, 0)));
            // ...

            dxfFile.Save(filename);
        }
    }
}