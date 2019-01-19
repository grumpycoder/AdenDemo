using AdenDemo.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdenDemo.Web.Data.Configuration
{
    public class FileSpecificationConfiguration : EntityTypeConfiguration<FileSpecification>
    {
        public FileSpecificationConfiguration()
        {
            ToTable("Aden.FileSpecifications");
            Property(s => s.Id).HasColumnName("FileSpecificationId");
            Property(s => s.FileNumber).HasMaxLength(8);
            Property(s => s.FileName).HasMaxLength(250);
            //Property(s => s.FileNameFormat).HasMaxLength(50);
            //Property(s => s.DueDate).HasColumnType("datetime2");

        }
    }
}
