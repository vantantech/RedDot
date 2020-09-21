
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class CSVWriter
    {
    public static void WriteDataTable(DataTable sourceTable,string filename , bool includeHeaders) 
    {

       StreamWriter writer = new StreamWriter(filename);


        if (includeHeaders) {
            IEnumerable<String> headerValues = sourceTable.Columns
                .OfType<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));
                
            writer.WriteLine(String.Join(",", headerValues));
        }

        IEnumerable<String> items = null;

        foreach (DataRow row in sourceTable.Rows) {
            items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
            writer.WriteLine(String.Join(",", items));
        }

        writer.Flush();
        writer.Close();
    }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

    }
}
