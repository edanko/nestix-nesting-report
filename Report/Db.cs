using System.Collections.Generic;
using System.Data.SqlClient;

namespace Report
{
    public static class Db
    {
        private static List<Part> FillParts(SqlConnection sqlConnection, int nxpathid)
        {
            var parts = new List<Part>();

            var c = new SqlCommand(DefaultPartQuery, sqlConnection);
            c.Parameters.AddWithValue("@pathid", nxpathid);
            var r = c.ExecuteReader();

            while (r.Read())
            {
                var part = new Part();
                part.Project = (string)r["Project"];
                part.Section = (string)r["Section"];
                part.DetailCode = (int)r["DetailCode"];
                part.DetailCount = (int)r["DetailCount"];
                part.Pos = (string)r["Pos"];
                part.Length = (float)r["Length"];
                part.Width = (float)r["Width"];
                part.Weight = (float)r["Weight"];
                parts.Add(part);
            }

            r.Close();

            parts.Sort((x, y) => x.DetailCode.CompareTo(y.DetailCode));

            return parts;
        }

        private static List<Tool> FillTools(SqlConnection sqlConnection, int nxpathid)
        {
            var tools = new List<Tool>();

            var c = new SqlCommand(DefaultToolQuery, sqlConnection);
            c.Parameters.AddWithValue("@pathid", nxpathid);
            var r = c.ExecuteReader();

            while (r.Read())
            {
                var tool = new Tool
                {
                    DistanceM = (double)r["Distance_m"],
                    MoveTime = (double)r["MoveTime"],
                    Pathid = (int)r["pathid"],
                    //Speed = (double)r["Speed"],
                    StartCount = (int)r["StartCount"],
                    StartTimeMin = (double)r["StartTime_min"],
                    ToolName = (string)r["ToolName"],
                    TotalTimeMin = (double)r["TotalTime_min"]
                };
                tools.Add(tool);
            }

            r.Close();

            return tools;
        }

        private static Plate FillPlate(SqlConnection sqlConnection, int nxsheetpathid)
        {
            var plate = new Plate();

            var c = new SqlCommand(DefaultPlateQuery, sqlConnection);
            c.Parameters.AddWithValue("@sheetpathid", nxsheetpathid);
            var r = c.ExecuteReader();

            r.Read();

            plate.Quality = (string)r["Quality"];
            plate.Thickness = (float)r["Thickness"];
            plate.UsedWeight = (float)r["UsedWeight"];
            plate.NestGrossWeight = (double)r["NestGrossWeight"];
            plate.MatWeight = (double)r["MatWeight"];
            plate.Length = (float)r["Length"];
            plate.Width = (float)r["Width"];

            r.Close();

            return plate;
        }

        public static List<Nest> FillMasterData(SqlConnection sqlConnection, List<string> pathids)
        {
            var all = new List<Nest>();

            var c = new SqlCommand(DefaultMainQuery, sqlConnection);
            c.AddArrayParameters("@pathid", pathids);
            var r = c.ExecuteReader();
            while (r.Read())
            {
                var masterData = new Nest();
                masterData.Nxpathid = (int)r["nxpathid"];
                masterData.Nxsheetpathid = (int)r["nxsheetpathid"];
                masterData.NcName = (string)r["NcName"];
                masterData.Machine = (string)r["Machine"];
                masterData.Used = (float)r["Used"];
                masterData.Info = (string)r["Info"];
                masterData.NxlFile = (string)r["NxlFile"];
                masterData.EmfImage = (string)r["EmfImage"];
                masterData.RemnantArea = (double)r["RemnantArea"];
                masterData.RemnantWeight = (double)r["RemnantWeight"];
                all.Add(masterData);
            }

            r.Close();

            foreach (var m in all)
            {
                m.Plate = FillPlate(sqlConnection, m.Nxsheetpathid);
                m.Parts = FillParts(sqlConnection, m.Nxpathid);
                m.Tools = FillTools(sqlConnection, m.Nxpathid);
            }

            return all;
        }

        #region queries

        private const string DefaultMainQuery = @"SELECT
  nxpath.nxpathid,
  nxsheetpath.nxsheetpathid,
  nxpath.nxname AS NcName,
  machine.name AS Machine,
  nxsheetpath.nxused AS Used,
  ISNULL(nxpath.nxpathinfo, '') AS Info,
  nxpath.nxpthfilename AS NxlFile,
  nxpath.nxpthmetafile AS EmfImage,

  ISNULL((SELECT SUM(remnant.nxprarea) * nxproduct.nxprthick * nxproduct.nxprdensity FROM nxsheetpathdet as spd with (nolock) inner join nxproduct as remnant with (nolock) on spd.nxproductid = remnant.nxproductid WHERE spd.nxsheetpathid = nxsheetpath.nxsheetpathid), 0) as RemnantWeight,
  ISNULL((SELECT SUM(remnant.nxprarea) FROM nxsheetpathdet as spd with (nolock) inner join nxproduct as remnant with (nolock) on spd.nxproductid = remnant.nxproductid WHERE spd.nxsheetpathid = nxsheetpath.nxsheetpathid), 0) as RemnantArea
FROM
  nxpath with (nolock)
  INNER JOIN nxsheetpath with (nolock) ON nxsheetpath.nxpathid = nxpath.nxpathid
  INNER JOIN nxorderline mol with (nolock) ON mol.nxorderlineid = nxsheetpath.nxmatorderlineid
  INNER JOIN nxproduct with (nolock) ON nxproduct.nxproductid = mol.nxproductid
  INNER JOIN machine with (nolock) ON machine.machineid = nxpath.nxmachineid
WHERE nxpath.nxpathid IN ({@pathid})";

        private const string DefaultPartQuery = @"SELECT
    nxsheetpath.nxpathid,
    nxsheetpath.nxsheetpathid,
    CAST(nxsheetpathdet.nxdetailcode AS int) AS DetailCode,
    ISNULL(nxorderline.nxororderno, '') AS Project,
    ISNULL(nxorderline.nxolsection, '') AS Section,
    ISNULL(nxproduct.nxprpartno,'') AS Pos,
    ISNULL(nxsheetpathdet.nxdetailcount*matpos.nxolordercount, 0) AS DetailCount,
    ISNULL(nxproduct.nxprminrlen, 0) AS Length,
    ISNULL(nxproduct.nxprminrwidth, 0) AS Width,
    ISNULL(nxsheetpathdet.nxarea * nxorderline.nxolthick * PrPlate.nxprdensity, 0) AS Weight

FROM nxproduct with(nolock)
    INNER JOIN nxorderline with(nolock) ON nxorderline.nxpartid = nxproduct.nxproductid
    INNER JOIN nxproduct AS posmat with(nolock) ON nxorderline.nxproductid = posmat.nxproductid
    INNER JOIN nxsheetpathdet with(nolock) ON nxsheetpathdet.nxorderlineid = nxorderline.nxorderlineid
    INNER JOIN nxsheetpath with(nolock) ON nxsheetpathdet.nxsheetpathid = nxsheetpath.nxsheetpathid
    INNER JOIN nxorderline AS matpos with(nolock) ON nxsheetpath.nxmatorderlineid = matpos.nxorderlineid
    INNER JOIN nxorderline AS matol with (nolock) ON matol.nxorderlineid = nxsheetpath.nxmatorderlineid
    INNER JOIN nxproduct AS PrPlate with (nolock) ON PrPlate.nxproductid = matol.nxproductid
  WHERE nxpathid = (@pathid)";

        private const string DefaultPlateQuery = @"SELECT nxproduct.nxprquality as Quality,
    nxproduct.nxprthick as Thickness,
    ISNULL(matol.nxollength, 0) as Length,
    ISNULL(matol.nxolwidth, 0) as Width,
    (nxproduct.nxprthick * nxproduct.nxprdensity * nxsheetpath.nxusedarea) as UsedWeight,
    SUM((isnull(matol.nxolordercount, 0) * nxsheetpathdet.nxdetailcount * nxproduct.nxprthick * nxproduct.nxprdensity * nxsheetpathdet.nxarea)) AS MatWeight,
    SUM((isnull(matol.nxolordercount, 0) * nxsheetpathdet.nxdetailcount * nxproduct.nxprthick * nxproduct.nxprdensity * nxsheetpathdet.nxusedarea)) AS NestGrossWeight


FROM nxsheetpath with (nolock)
    INNER JOIN nxsheetpathdet with (nolock) on nxsheetpath.nxsheetpathid = nxsheetpathdet.nxsheetpathid
    INNER JOIN nxorderline as matol with (nolock) on matol.nxorderlineid = nxsheetpath.nxmatorderlineid
    INNER JOIN nxproduct with (nolock) on nxproduct.nxproductid = matol.nxproductid

WHERE nxsheetpath.nxsheetpathid = (@sheetpathid)
GROUP BY
  nxproduct.nxprquality, 
  nxproduct.nxprthick, 
  matol.nxollength, 
  matol.nxolwidth,
  nxsheetpath.nxusedarea,
  nxproduct.nxprthick,
  nxproduct.nxprdensity,
  nxsheetpath.nxsheetpathid";

        private const string DefaultToolQuery = @"select
--tooltype_w_beveltype as ToolName,
concat(tooltype, ' ', toolname) as ToolName,
machdist/1000 as Distance_m,
--machdist/machtime as Speed,
machtime as MoveTime,
startcount as StartCount,
starttime as StartTime_min,
machtime + starttime as TotalTime_min,
pathid
from (SELECT ROW_NUMBER() OVER (ORDER BY nxpathid ASC) AS machstaticid, nxpathid AS pathid, rtrim(ltrim(tool.value('./toolname[1]', 'nvarchar(50)'))) AS tooltype, rtrim(ltrim(tool.value('./bevel[1]/beveltext[1]', 'nvarchar(50)'))) AS toolname, (dbo.NxDbCharToFloat(tool.value('./totaltime[1]', 'nvarchar(50)')) 
/ CASE tool.value('./totaltimeunit[1]', 'nvarchar(50)') WHEN 'min' THEN 1 ELSE 60 END) - (dbo.NxDbCharToFloat(tool.value('./piercetime[1]', 'nvarchar(50)')) / CASE tool.value('./piercetimeunit[1]', 'nvarchar(50)') 
WHEN 'min' THEN 1 ELSE 60 END) AS machtime, tool.value('./piercecnt[1]', 'int') AS startcount, dbo.NxDbCharToFloat(tool.value('./piercetime[1]', 'nvarchar(50)')) / CASE tool.value('./piercetimeunit[1]', 'nvarchar(50)') 
WHEN 'min' THEN 1 ELSE 60 END AS starttime, dbo.NxDbCharToFloat(tool.value('./totallength[1]', 'nvarchar(50)')) * CASE tool.value('./totallengthunit[1]', 'nvarchar(50)') WHEN 'mm' THEN 1 ELSE 1000 END AS machdist, 0 AS compvalue, 
'min' AS timeunit, 'mm' AS distunit
FROM nxpath WITH (nolock) CROSS apply nxpath.nxpthtooldata.nodes('//nestdata/machstatic/tool') AS t (tool)) as machstatic
where pathid = (@pathid)";

        #endregion
    }
}