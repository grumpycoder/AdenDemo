using Aden.Web.Data;
using Aden.Web.Helpers;
using Aden.Web.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Aden.Web.Services
{
    public class DocumentService
    {
        private readonly AdenContext _context;

        public DocumentService(AdenContext context)
        {
            _context = context;
        }

        public void GenerateDocuments(Report report)
        {
            var version = report.CurrentDocumentVersion ?? 0 + 1;
            string filename;

            if (report.Submission.FileSpecification.IsSCH)
            {
                filename = report.Submission.FileSpecification.FileNameFormat.Replace("{level}", ReportLevel.SCH.GetDisplayName()).Replace("{version}", string.Format("v{0}.csv", version));

                var file = ExecuteDocumentCreationToFile(report, ReportLevel.SCH);
                var doc = new ReportDocument() { FileData = file, ReportLevel = ReportLevel.SCH, Filename = filename, FileSize = file.Length, Version = version };
                report.Documents.Add(doc);

            }
            if (report.Submission.FileSpecification.IsLEA)
            {
                filename = report.Submission.FileSpecification.FileNameFormat.Replace("{level}", ReportLevel.LEA.GetDisplayName()).Replace("{version}", string.Format("v{0}.csv", version));
                var file = ExecuteDocumentCreationToFile(report, ReportLevel.LEA);
                var doc = new ReportDocument() { FileData = file, ReportLevel = ReportLevel.SCH, Filename = filename, FileSize = file.Length, Version = version };
                report.Documents.Add(doc);
            }
            if (report.Submission.FileSpecification.IsSEA)
            {
                filename = report.Submission.FileSpecification.FileNameFormat.Replace("{level}", ReportLevel.SEA.GetDisplayName()).Replace("{version}", string.Format("v{0}.csv", version));
                var file = ExecuteDocumentCreationToFile(report, ReportLevel.SEA);
                var doc = new ReportDocument() { FileData = file, ReportLevel = ReportLevel.SCH, Filename = filename, FileSize = file.Length, Version = version };
                report.Documents.Add(doc);
            }
            report.GeneratedDate = DateTime.Now;
            report.CurrentDocumentVersion = version;
        }


        private byte[] ExecuteDocumentCreationToFile(Report report, ReportLevel reportLevel)
        {
            var dataTable = new DataTable();
            var ds = new DataSet();
            using (var connection = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(report.Submission.FileSpecification.ReportAction, connection))
                {
                    cmd.CommandTimeout = Constants.CommandTimeout;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DataYear", report.Submission.DataYear);
                    cmd.Parameters.AddWithValue("@ReportLevel", reportLevel.GetDisplayName());
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    adapter.Fill(ds);
                }
            }

            var version = 1; //report.GetNextFileVersionNumber(reportLevel);
            var filename = report.Submission.FileSpecification.FileNameFormat.Replace("{level}", reportLevel.GetDisplayName()).Replace("{version}", string.Format("v{0}.csv", version));

            var table1 = ds.Tables[0].UpdateFieldValue("Filename", filename).ToCsvString(false);
            var table2 = ds.Tables[1].UpdateFieldValue("Filename", filename).ToCsvString(false);


            var file = Encoding.ASCII.GetBytes(ds.Tables[0].Rows.Count > 1 ? string.Concat(table2, table1) : string.Concat(table1, table2)); ;

            return file;

        }


    }
}
