using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Report
{
	public static class Db
	{
        private static List<Part> FillParts(SqlConnection sqlConnection, int nxpathid)
        {
            var parts = new List<Part>();

            try
            {
                var c = new SqlCommand(DefaultPartQuery, sqlConnection);
                c.Parameters.AddWithValue("@pathid", nxpathid);
                var r = c.ExecuteReader();

                while (r.Read())
		        {
                    var part = new Part
                    {
                        Order = r["Order1"]?.ToString(),
                        Section = r["Section"]?.ToString(),
                        DetailCode = r["DetailCode"] is DBNull ? 0 : (int?) r["DetailCode"],
                        DetailCount = r["DetailCount"] is DBNull ? 0 : (int?) r["DetailCount"],
                        //Nxpathid = r["nxpathid"] is DBNull ? 0 : (int?) r["nxpathid"],
                        //Nxsheetpathid = r["nxsheetpathid"] is DBNull ? 0 : (int?) r["nxsheetpathid"],
                        Pos = r["Pos"]?.ToString(), 
                        Length = r["Length"] is DBNull ? 0f : (float?) r["Length"],
                        Width = r["Width"] is DBNull ? 0f : (float?) r["Width"],
                        TotalWeight = r["TotalWeight"] is DBNull ? 0f : (float?) r["TotalWeight"],
                        Weight = r["Weight"] is DBNull ? 0f : (float?) r["Weight"]
                    };
                    parts.Add(part);
		        }
                r.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return parts;
        }

        private static List<Tool> FillTools(SqlConnection sqlConnection, int nxpathid)
		{
            var tools = new List<Tool>();

            try
            {
                var c = new SqlCommand(DefaultToolQuery, sqlConnection);
                c.Parameters.AddWithValue("@pathid", nxpathid);
                var r = c.ExecuteReader();

			    while (r.Read())
			    {
                    var tool = new Tool
                    {
                        DistanceM = r["Distance_m"] is DBNull ? 0.0 : (double?) r["Distance_m"],
                        MoveTime = r["MoveTime"] is DBNull ? 0.0 : (double?) r["MoveTime"],
                        Pathid = r["pathid"] is DBNull ? 0 : (int?) r["pathid"],
                        Speed = r["Speed"] is DBNull ? 0.0 : (double?) r["Speed"],
                        StartCount = r["StartCount"] is DBNull ? 0 : (int?) r["StartCount"],
                        StartTimeMin = r["StartTime_min"] is DBNull ? 0.0 : (double?) r["StartTime_min"],
                        ToolName = r["ToolName"]?.ToString(),
                        TotalTimeMin = r["TotalTime_min"] is DBNull ? 0.0 : (double?) r["TotalTime_min"]
                    };
                    tools.Add(tool);
			    }
                r.Close();
            }
            catch
            {
                Console.WriteLine("tools not found");
            }
            return tools;
		}

        private static List<Remnant> FillRemnants(SqlConnection sqlConnection, int nxsheetpathid)
		{
			var remnants = new List<Remnant>();
            
            try
            {
                var c = new SqlCommand(DefaultRemnantQuery, sqlConnection);
                c.Parameters.AddWithValue("@sheetpathid", nxsheetpathid);
                var r = c.ExecuteReader();

			    while (r.Read())
			    {
                    var remnant = new Remnant
                    {
                        Nxpathid = r["nxpathid"] is DBNull ? 0 : (int?) r["nxpathid"],
                        Nxproductid = r["nxproductid"] is DBNull ? 0 : (int?) r["nxproductid"],
                        RemnantCount = r["RemnantCount"] is DBNull ? 0 : (int?) r["RemnantCount"],
                        RemnantImage = r["RemnantImage"]?.ToString(),
                        RemnantLength = r["RemnantLength"] is DBNull ? 0f : (float?) r["RemnantLength"],
                        RemnantName = r["RemnantName"]?.ToString(),
                        RemnantWeight = r["RemnantWeight"] is DBNull ? 0.0 : (double?) r["RemnantWeight"],
                        RemnantWidth = r["RemnantWidth"] is DBNull ? 0f : (float?) r["RemnantWidth"]
                    };
                    remnants.Add(remnant);
			    }
                r.Close();
            }
            catch
            {
                Console.WriteLine("remnants not found");
            }
            return remnants;
		}

        private static Plate FillPlate(SqlConnection sqlConnection, int nxsheetpathid)
        {
            var plate = new Plate();

            try
            {
                var c = new SqlCommand(DefaultPlateQuery, sqlConnection);
                c.Parameters.AddWithValue("@sheetpathid", nxsheetpathid);
                var r = c.ExecuteReader();

			    r.Read();

                plate.Quality = r["Quality"]?.ToString();
                plate.Thickness = r["Thickness"] is DBNull ? 0f : (float?) r["Thickness"];
                plate.UsedWeight = r["UsedWeight"] is DBNull ? 0f : (float?) r["UsedWeight"];
                plate.NestGrossWeight = r["NestGrossWeight"] is DBNull ? 0.0 : (double?) r["NestGrossWeight"];
                plate.MatWeight = r["MatWeight"] is DBNull ? 0.0 : (double?) r["MatWeight"];
                plate.Used = r["Used"] is DBNull ? 0f : (float?) r["Used"];
                plate.UsedArea = r["UsedArea"] is DBNull ? 0f : (float?) r["UsedArea"];
                plate.PlateCount = r["PlateCount"] is DBNull ? 0 : (int?) r["PlateCount"];
                plate.Length = r["Length"] is DBNull ? 0f : (float?) r["Length"];
                plate.Width = r["Width"] is DBNull ? 0f : (float?) r["Width"];
                plate.PrecedingCnc = r["PrecedingCnc"]?.ToString();

                r.Close();
            }
            catch
            {
                Console.WriteLine("plate not found");
            }
            return plate;
        }

        public static List<Nest> FillMasterData(SqlConnection sqlConnection, List<string> pathids)
        {
            var all = new List<Nest>();

            var c = new SqlCommand(DefaultMainQuery, sqlConnection);
            c.AddArrayParameters("pathid", pathids);
            var r = c.ExecuteReader();
            try
            {
                while (r.Read())
                {
                    var masterData = new Nest
                    {
                        PartsCount = r["PartsCount"] is DBNull ? 0 : (int?) r["PartsCount"],
                        NcName = r["NcName"]?.ToString(),
                        Machine = r["Machine"]?.ToString(),
                        EmfImage = r["EmfImage"]?.ToString(),
                        Info = r["Info"]?.ToString(),
                        NxlFile = r["NxlFile"]?.ToString(),
                        Used = r["Used"] is DBNull ? 0f : (float?) r["Used"],
                        Nxpathid = r["nxpathid"] is DBNull ? 0 : (int?) r["nxpathid"],
                        Nxsheetpathid = r["nxsheetpathid"] is DBNull ? 0 : (int?) r["nxsheetpathid"],
                        PartsWeight = r["PartsWeight"] is DBNull ? 0.0 : (double?) r["PartsWeight"],
                        RemnantArea = r["RemnantArea"] is DBNull ? 0.0 : (double?) r["RemnantArea"],
                        RemnantsCount = r["RemnantsCount"] is DBNull ? 0 : (int?) r["RemnantsCount"],
                        RemnantWeight = r["RemnantWeight"] is DBNull ? 0.0 : (double?) r["RemnantWeight"]
                    };
                    all.Add(masterData);
                }
            }
            catch
            {
                Console.WriteLine("Не удалось выполнить чтение данных");
            }
            r.Close();

            foreach (var m in all)
            {
                m.Plate = FillPlate(sqlConnection, m.Nxsheetpathid.Value);
                if (m.RemnantsCount > 0)
                {
                    m.Remnants = FillRemnants(sqlConnection, m.Nxsheetpathid.Value);
                }

                m.Tools = FillTools(sqlConnection, m.Nxpathid.Value);
                if (m.PartsCount > 0)
                {
                    m.Parts = FillParts(sqlConnection, m.Nxpathid.Value);
                }
            }

            return all;
		}

        #region queries

        private const string DefaultMainQuery = @"SELECT
  SYSTEM_USER as UserName,
  (SELECT COUNT(*) FROM nxsheetpathdet spd WHERE spd.nxsheetpathid = nxsheetpath.nxsheetpathid and spd.nxorderlineid is not null) as PartsCount,
  (SELECT COUNT(*) FROM nxsheetpathdet spd WHERE spd.nxsheetpathid = nxsheetpath.nxsheetpathid and spd.nxproductid is not null) as RemnantsCount,
  nxpath.nxpathid,
  nxsheetpath.nxsheetpathid, 
  
  nxpath.nxpthcreator as NestCreator,
  nxpath.nxpthfilename as NxlFile,
  nxpath.nxpthcreated as NestDueDate,
  nxpath.nxpathinfo as Info,

  nxsheetpath.nxused as Used,
  nxpath.nxpthmetafile as EmfImage,
  nxinventory.nxinvcharge as MatCharge,

  nxpath.nxname as NcName,
  machine.name as Machine,
  nxinventory.nxinvplateno as MatPlateNo,
  nxproduct.nxprquality as Quality,
  nxproduct.nxproductno as MatArticle,
  ISNULL(mol.nxollength, nxpath.nxmainlength) as MatLength,
  ISNULL(mol.nxolwidth, nxpath.nxmainheight) as MatHeight,
  ISNULL(nxinventory.nxinvthick, nxproduct.nxprthick) as MatThick,
  
  ISNULL(nxinventory.nxinvweight, nxproduct.nxprweight) as MatWeight,
  (SELECT SUM(ISNULL(spd.nxarea * pr.nxprthick * pr.nxprdensity, col.nxolweight) * spd.nxdetailcount)
    FROM nxsheetpathdet as spd 
      INNER JOIN nxorderline as col on spd.nxorderlineid = col.nxorderlineid 
      LEFT OUTER JOIN nxproduct as pr on pr.nxproductid = nxproduct.nxproductid
    WHERE spd.nxsheetpathid = nxsheetpath.nxsheetpathid
  ) as PartsWeight,
  (SELECT SUM(remnant.nxprarea) * nxproduct.nxprthick * nxproduct.nxprdensity FROM nxsheetpathdet as spd with (nolock) inner join nxproduct as remnant with (nolock) on spd.nxproductid = remnant.nxproductid WHERE spd.nxsheetpathid = nxsheetpath.nxsheetpathid) as RemnantWeight,
  (SELECT SUM(remnant.nxprarea) FROM nxsheetpathdet as spd with (nolock) inner join nxproduct as remnant with (nolock) on spd.nxproductid = remnant.nxproductid WHERE spd.nxsheetpathid = nxsheetpath.nxsheetpathid) as RemnantArea
FROM 
  nxpath with (nolock) 
  INNER JOIN nxsheetpath with (nolock) ON nxsheetpath.nxpathid = nxpath.nxpathid 
  INNER JOIN nxorderline mol with (nolock) ON mol.nxorderlineid = nxsheetpath.nxmatorderlineid
  INNER JOIN nxproduct with (nolock) ON nxproduct.nxproductid = mol.nxproductid
  INNER JOIN machine with (nolock) ON machine.machineid = nxpath.nxmachineid
  LEFT OUTER JOIN nxinventory with (nolock) ON mol.nxorderlineid = nxinventory.nxinvmatorderlineid and nxinventory.nxorderlineid is null
WHERE nxpath.nxpathid in ({pathid})
ORDER BY 
  nxpath.nxpathid";

        private const string DefaultPartQuery = @"select top 1000 
nxsheetpath.nxpathid,
nxsheetpath.nxsheetpathid,
cast (nxsheetpathdet.nxdetailcode as int) as DetailCode,
isnull(nxorderline.nxororderno,'') as Order1,
isnull(nxorderline.nxolsection,'') as Section,
isnull(nxproduct.nxprpartno,'') as Pos,
isnull(nxsheetpathdet.nxdetailcount*matpos.nxolordercount, 0) as DetailCount,
nxproduct.nxprlength as Length,
nxproduct.nxprwidth as Width,
nxsheetpathdet.nxarea * nxorderline.nxolthick * PrPlate.nxprdensity as [Weight],
(nxsheetpathdet.nxarea * nxorderline.nxolthick * PrPlate.nxprdensity) * nxsheetpathdet.nxdetailcount as TotalWeight
from nxproduct with(nolock) 
inner join nxorderline with(nolock) on nxorderline.nxpartid = nxproduct.nxproductid 
inner join nxproduct as posmat with(nolock) on nxorderline.nxproductid = posmat.nxproductid
inner join nxsheetpathdet with(nolock) 
    inner join nxsheetpath with(nolock) on nxsheetpathdet.nxsheetpathid = nxsheetpath.nxsheetpathid
    inner join nxorderline as matpos with(nolock) on nxsheetpath.nxmatorderlineid = matpos.nxorderlineid
on nxsheetpathdet.nxorderlineid = nxorderline.nxorderlineid
  INNER JOIN nxorderline as matol with (nolock) on matol.nxorderlineid = nxsheetpath.nxmatorderlineid
  INNER JOIN nxproduct as PrPlate with (nolock) on PrPlate.nxproductid = matol.nxproductid
WHERE nxpathid = (@pathid)
--where nxpathid = 20151
order by DetailCode";

        private const string DefaultPlateQuery = @"SELECT nxinventory.nxinventoryid,
  CASE WHEN nxinventory.nxinvactcode = 6 
    THEN ISNULL((SELECT p.nxname 
      FROM nxpath as p with (nolock)
      INNER JOIN nxsheetpath as sp with (nolock) on sp.nxpathid = p.nxpathid
      INNER JOIN nxsheetpathdet as spd with (nolock) on spd.nxsheetpathid = sp.nxsheetpathid
      INNER JOIN nxinventory as i with (nolock) on i.nxpartid = spd.nxproductid
      WHERE i.nxinventoryid = nxinventory.nxinventoryid),'')
    ELSE ''
  END as PrecedingCnc,
  --nxstplace.nxstpname as StorageName,
  'StorageName' as StorageName,
  --isnull(nxinventory.nxinvplateno, '') + ' ' + isnull(nxinventory.nxinvremnantno, '') as PlateNo,
  nxproduct.nxprquality as Quality,
  nxproduct.nxproductno as MatProductNo,
  nxproduct.nxprthick as Thickness,
  ISNULL(nxinventory.nxinvlength, matol.nxollength) as Length,
  ISNULL(nxinventory.nxinvwidth, matol.nxolwidth) as Width,
  nxsheetpath.nxusedarea as UsedArea,
  CASE WHEN ISNULL(nxsheetpath.nxused, 0) = 0 THEN -1 ELSE nxsheetpath.nxused END as Used,
  (nxproduct.nxprthick * nxproduct.nxprdensity * nxsheetpath.nxusedarea) as UsedWeight,
  SUM((isnull(nxinventory.nxinvcount, matol.nxolordercount) * nxsheetpathdet.nxdetailcount * nxproduct.nxprthick * nxproduct.nxprdensity * nxsheetpathdet.nxarea)) MatWeight,
  SUM((isnull(nxinventory.nxinvcount, matol.nxolordercount) * nxsheetpathdet.nxdetailcount * nxproduct.nxprthick * nxproduct.nxprdensity * nxsheetpathdet.nxusedarea)) NestGrossWeight,
  ISNULL(nxinventory.nxinvcount, matol.nxolordercount) as PlateCount,
  nxsheetpath.nxsheetpathid
FROM nxsheetpath with (nolock)
  INNER JOIN nxsheetpathdet with (nolock) on nxsheetpath.nxsheetpathid = nxsheetpathdet.nxsheetpathid
  INNER JOIN nxorderline as matol with (nolock) on matol.nxorderlineid = nxsheetpath.nxmatorderlineid
  INNER JOIN nxproduct with (nolock) on nxproduct.nxproductid = matol.nxproductid
  LEFT OUTER JOIN nxinventory with (nolock) on nxinventory.nxinvmatorderlineid = matol.nxorderlineid
    AND nxinventory.nxorderlineid is null AND nxinventory.nxinvactcode not in (11, 14)
  --LEFT OUTER JOIN nxstplace with (nolock) on nxstplace.nxstplaceid = nxinventory.nxstplaceid
WHERE nxsheetpath.nxsheetpathid = (@sheetpathid)
--WHERE nxsheetpath.nxsheetpathid = 228
GROUP BY
  nxinventory.nxinvplateno, 
  nxproduct.nxprquality, 
  nxproduct.nxproductno,
  --nxstplace.nxstpname, 
  nxproduct.nxprthick, 
  nxinventory.nxinvlength, 
  matol.nxollength, 
  nxinventory.nxinvwidth, 
  matol.nxolwidth,
  nxsheetpath.nxusedarea, 
  nxsheetpath.nxused,
  --nxinventory.nxinvremnantno, 
  nxproduct.nxprthick,
  nxproduct.nxprdensity, 
  nxsheetpath.nxusedarea, 
  matol.nxolordercount,
  nxinventory.nxinvcount, 
  nxinventory.nxinvactcode, 
  nxinventory.nxinventoryid,nxsheetpath.nxsheetpathid";

        private const string DefaultToolQuery = @"select 
--tooltype_w_beveltype as ToolName,
concat(tooltype, ' ', toolname) as ToolName,
machdist/1000 as Distance_m,
machdist/machtime as Speed,
machtime as MoveTime,
startcount as StartCount,
starttime as StartTime_min,
machtime + starttime as TotalTime_min,
pathid
from (SELECT
                          ROW_NUMBER() OVER (ORDER BY nxpathid ASC) AS machstaticid, nxpathid AS pathid, rtrim(ltrim(tool.value('./toolname[1]', 'nvarchar(50)'))) AS tooltype, rtrim(ltrim(tool.value('./bevel[1]/beveltext[1]', 'nvarchar(50)'))) AS toolname, (dbo.NxDbCharToFloat(tool.value('./totaltime[1]', 'nvarchar(50)')) 
/ CASE tool.value('./totaltimeunit[1]', 'nvarchar(50)') WHEN 'min' THEN 1 ELSE 60 END) - (dbo.NxDbCharToFloat(tool.value('./piercetime[1]', 'nvarchar(50)')) / CASE tool.value('./piercetimeunit[1]', 'nvarchar(50)') 
WHEN 'min' THEN 1 ELSE 60 END) AS machtime, tool.value('./piercecnt[1]', 'int') AS startcount, dbo.NxDbCharToFloat(tool.value('./piercetime[1]', 'nvarchar(50)')) / CASE tool.value('./piercetimeunit[1]', 'nvarchar(50)') 
WHEN 'min' THEN 1 ELSE 60 END AS starttime, dbo.NxDbCharToFloat(tool.value('./totallength[1]', 'nvarchar(50)')) * CASE tool.value('./totallengthunit[1]', 'nvarchar(50)') WHEN 'mm' THEN 1 ELSE 1000 END AS machdist, 0 AS compvalue, 
'min' AS timeunit, 'mm' AS distunit
FROM            nxpath WITH (nolock) CROSS apply nxpath.nxpthtooldata.nodes('//nestdata/machstatic/tool') AS t (tool)) as machstatic
where pathid = (@pathid)";

        private const string DefaultRemnantQuery = @"select
  nxpath.nxpathid,
  remnant.nxproductid,
  remnant.nxproductno as RemnantName,
  --isnull(nxinventory.nxinvplateno, '') + isnull(remnant.nxprpartname, '') as PlateNo,
  remnant.nxprlength as RemnantLength, 
  remnant.nxprwidth as RemnantWidth, 
  remnant.nxprmeta as RemnantImage,
  remnant.nxprarea as RemnantArea,
  sum(convert(float, remnant.nxprarea * mol.nxolthick * isnull(nxproduct.nxprdensity,8)) * convert(float, mol.nxolordercount)) as RemnantWeight,
  mol.nxolordercount as RemnantCount
FROM nxpath with (nolock)
  INNER JOIN nxsheetpath with (nolock) on nxpath.nxpathid = nxsheetpath.nxpathid
  INNER JOIN nxorderline as mol with (nolock) on nxsheetpath.nxmatorderlineid = mol.nxorderlineid
  INNER JOIN nxproduct with (nolock) on mol.nxproductid = nxproduct.nxproductid
  INNER JOIN nxsheetpathdet with (nolock) on nxsheetpath.nxsheetpathid = nxsheetpathdet.nxsheetpathid
  INNER JOIN nxproduct as remnant with (nolock) on nxsheetpathdet.nxproductid = remnant.nxproductid and remnant.nxprtype = 12
  LEFT OUTER JOIN nxinventory with (nolock) on nxinventory.nxpartid = remnant.nxproductid
WHERE nxsheetpath.nxsheetpathid = (@sheetpathid)
--WHERE nxsheetpath.nxsheetpathid = 228
GROUP BY
  nxpath.nxpathid,
  remnant.nxproductid,
  remnant.nxproductno,
  --isnull(nxinventory.nxinvplateno, '') + isnull(remnant.nxprpartname, ''),
  remnant.nxprlength, 
  remnant.nxprwidth, 
  remnant.nxprmeta,
  remnant.nxprarea,
  mol.nxolordercount";

        #endregion
    }
}
