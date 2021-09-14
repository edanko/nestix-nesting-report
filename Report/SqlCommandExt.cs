using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Report
{
    public static class SqlCommandExt
    {
        public static SqlParameter[] AddArrayParameters<T>(this SqlCommand cmd, string paramNameRoot,
            IEnumerable<T> values, SqlDbType? dbType = null, int? size = null)
        {
            var list = new List<SqlParameter>();
            var list2 = new List<string>();
            int num = 1;
            foreach (var value in values)
            {
                var text = $"@{paramNameRoot}{num++}";
                list2.Add(text);
                var sqlParameter = new SqlParameter(text, value);
                if (dbType.HasValue)
                {
                    sqlParameter.SqlDbType = dbType.Value;
                }

                if (size.HasValue)
                {
                    sqlParameter.Size = size.Value;
                }

                cmd.Parameters.Add(sqlParameter);
                list.Add(sqlParameter);
            }

            cmd.CommandText = cmd.CommandText.Replace("{" + paramNameRoot + "}", string.Join(",", list2));
            return list.ToArray();
        }
    }
}