using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Aden.Web.Helpers
{
    public static class DataTableHelper
    {
        public static byte[] ToCsvBytes(this DataTable dtDataTable, bool withHeaderRow = true)
        {
            var sb = new StringBuilder();

            //headers  
            if (withHeaderRow)
            {
                for (var i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sb.Append(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();
            }

            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (var i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = $"\"{value}\"";
                            sb.Append(value);
                        }
                        else
                        {
                            sb.Append(dr[i]);
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        public static string ToCsvString(this DataTable dtDataTable, bool withHeaderRow = true)
        {
            var sb = new StringBuilder();

            //headers  
            if (withHeaderRow)
            {
                for (var i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sb.Append(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();
            }

            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (var i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = $"\"{value}\"";
                            sb.Append(value);
                        }
                        else
                        {
                            sb.Append(dr[i]);
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static DataTable UpdateFieldValue(this DataTable dtDataTable, string fieldname, string filename)
        {
            if (!dtDataTable.Columns.Contains(fieldname)) return dtDataTable;
            foreach (DataRow row in dtDataTable.Rows)
            {
                row.SetField(fieldname, filename);
            }
            return dtDataTable;
        }

        public static void ToCsvFile(this DataTable dtDataTable, string strFilePath, bool withHeaderRow = true)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            if (withHeaderRow)
            {
                for (var i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sw.Write(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }

            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (var i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = $"\"{value}\"";
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
