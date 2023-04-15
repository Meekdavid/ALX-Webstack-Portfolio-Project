using catalogueService.Interfaces;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace catalogueService.Repositories
{
    public class JsonFormatter : IJsonFormatter
    {
        public async Task<string> JsonFormat(DataTable objectValue)
        {
            var jsonStringBuilder = new StringBuilder();
            if (objectValue.Rows.Count > 0)
            {
                jsonStringBuilder.Append("[\r\n");
                for (int i = 0; i < objectValue.Rows.Count; i++)
                {
                    jsonStringBuilder.Append("  {\r\n");
                    for (int j = 0; j < objectValue.Columns.Count; j++)
                        jsonStringBuilder.AppendFormat("   {0}:\"{1}\"{2}",
                                objectValue.Columns[j].ColumnName.ToString(),
                                objectValue.Rows[i][j].ToString(),
                                j < objectValue.Columns.Count - 1 ? ",\r\n" : string.Empty);
                    jsonStringBuilder.Append(i == objectValue.Rows.Count - 1 ? "}\r\n" : "\r\n  },\r\n\r\n");
                }
                jsonStringBuilder.Append("]");
            }
            return jsonStringBuilder.ToString();
        }
    }
}
