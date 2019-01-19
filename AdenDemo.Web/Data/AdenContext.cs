﻿using AdenDemo.Web.Data.Configuration;
using AdenDemo.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdenDemo.Web.Data
{
    public class AdenContext : DbContext
    {
        public AdenContext()
            : base("AdenContext")
        {
            //Database.Log = msg => Debug.WriteLine(msg);
            Database.SetInitializer<AdenContext>(null);
        }

        //public DbSet<Report> Reports { get; set; }
        public DbSet<FileSpecification> FileSpecifications { get; set; }
        //public DbSet<WorkItem> WorkItems { get; set; }
        //public DbSet<WorkItemImage> WorkItemImages { get; set; }
        //public DbSet<ReportDocument> ReportDocuments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        //public DbSet<SubmissionAudit> SubmissionAudits { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Properties<string>().Configure(c => c.HasColumnType("varchar").HasMaxLength(255));
            modelBuilder.Properties<string>();

            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("smalldatetime"));


            //modelBuilder.Configurations.Add(new ReportConfiguration());
            modelBuilder.Configurations.Add(new FileSpecificationConfiguration());
            modelBuilder.Configurations.Add(new SubmissionConfiguration());
            //modelBuilder.Configurations.Add(new SubmissionAuditConfiguration());
            //modelBuilder.Configurations.Add(new WorkItemConfiguration());
            //modelBuilder.Configurations.Add(new WorkItemImageConfiguration());
            //modelBuilder.Configurations.Add(new ReportDocumentConfiguration());
        }
    }
}
