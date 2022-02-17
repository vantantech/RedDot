
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
            string debug = "[" + filename + "]";

            debug = debug + "[row count:" + sourceTable.Rows.Count + "]";

            try
            {
               

                using (StreamWriter writer = new StreamWriter(filename))
                {

                    if (includeHeaders)
                    {
                        IEnumerable<String> headerValues = sourceTable.Columns
                            .OfType<DataColumn>()
                            .Select(column => QuoteValue(column.ColumnName));

                        writer.WriteLine(String.Join(",", headerValues));
                        debug = debug + "[header done]";
                    }

                    IEnumerable<String> items = null;

                    int i = 0;
                    foreach (DataRow row in sourceTable.Rows)
                    {
                        i++;
                        items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                        writer.WriteLine(String.Join(",", items));
                        writer.Flush();
                        debug = debug + "row:" + i;
                    }

                    debug += "Done looping";
                    writer.Close();
                    
                }



            }catch(Exception ex)
            {
                TouchMessageBox.Show("Write Data Table:" + ex.Message);
                TouchMessageBox.Show("debug:" + debug);
            }
       
    }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

    }
}
