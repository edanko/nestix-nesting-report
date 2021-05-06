

/*
namespace Report
{
    public static class Program
	{
        [STAThread]
        private static void Main(string[] args)
        {
            var log = new Log();
            var wpfLog = new System.Windows.Application();
            wpfLog.Run(log);

            log.Show();

            log.AddLine("started...");

			if (!args.Any())
			{
                log.AddLine("no params");
				//Console.ReadKey();
				return;
			}

            var pathIdsList = new List<List<string>>(); 
            
            foreach (var text in args)
            {
                if (!text.StartsWith("-PARAMS="))
                {
                    continue;
                }
                
                var paramsList = text.TrimStart("-PARAMS=".ToCharArray()).Split(',').ToList();

                var size = 1000;
                    
                for (int i = 0; i < paramsList.Count; i += size)
                {
                    var end = i + size;
                    if (end > paramsList.Count)
                    {
                        end = paramsList.Count;
                    }

                    pathIdsList.Add(paramsList.GetRange(i, end - i));
                }
            }
            

            
            var db = GetDbName(@"..\..\master\settings\nestix2.ini");
            
            if (string.IsNullOrWhiteSpace(db))
            {
                log.AddLine("db name not found, exiting...");

                return;
            }

            var connectionString = $"Data Source=BK-SSK-NESH01.CORP.LOCAL;Initial Catalog=NxSC_Zvezda_{db};Integrated Security=SSPI";
            
            log.AddLine($"connecting to {db}...");

            var mdList = new List<Nest>();

            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            foreach (var pathIds in pathIdsList)
            {
                mdList.AddRange(Db.FillMasterData(sqlConnection, pathIds));
            }
            sqlConnection.Close();

            if (mdList.Count == 0)
            {
                Console.WriteLine("db query returned nothing");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"total ids: {mdList.Count}");

            var p = new PdfReport();
            var o = p.GenerateReport(mdList);

            #region save result
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var resultFilename = Path.Join(folder, p.Filename);

            File.WriteAllBytes(resultFilename, o);
            #endregion

            Console.WriteLine("done");

            #region open pdf
            var psi = new ProcessStartInfo(resultFilename)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
            #endregion
        }

        private static string GetDbName(string iniPath)
        {
            var db = "";

            if (!File.Exists(iniPath))
            {
                Console.WriteLine("ini file doesn't exists");
                return db;
            }
            

            var ini = File.ReadAllLines(iniPath);
            foreach (var l in ini)
            {
                if (!l.StartsWith("DataSource"))
                {
                    continue;
                }
                var l2 = l.TrimStart("DataSource=".ToCharArray());
                var lres = l2.Split(";");
                foreach (var s in lres)
                {
                    if (!s.StartsWith("DATABASE"))
                    {
                        continue;
                    }
                    db = s.Split("=")[1].TrimStart("NxSC_Zvezda_".ToCharArray());
                    return db;
                }
            }
            
            return db;
        }
    }
}
*/
