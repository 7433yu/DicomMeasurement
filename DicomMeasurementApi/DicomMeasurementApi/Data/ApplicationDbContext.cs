using DicomMeasurementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DicomMeasurementApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
        }
        public DbSet<Measurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //配置Measurement实体
            modelBuilder.Entity<Measurement>(entity => 
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(m => m.UserId)
                    .IsRequired()
                    .HasMaxLength(64);
                entity.Property(m => m.FileId)
                    .IsRequired()
                    .HasMaxLength(128);
                entity.Property(m=>m.StudyInstanceUid)
                    .IsRequired()
                    .HasMaxLength(128);
                entity.Property(m=>m.SeriesInstanceUid)
                    .IsRequired()
                    .HasMaxLength(128);
                entity.Property(m=>m.SopInstanceUid)
                    .HasMaxLength(128);
                entity.Property(m=>m.FrameNumber)
                    .HasDefaultValue(1);
                entity.Property(m=>m.MeasurementType)
                    .IsRequired()
                    .HasDefaultValue(MeasurementType.Length);
                entity.Property(m => m.Label)
                    .HasMaxLength(256);
                entity.Property(m => m.Description)
                    .HasMaxLength(512);
                entity.Property(m => m.Value);
                entity.Property(m => m.Color)
                    .HasDefaultValue("#FF0000");
                entity.Property(m => m.Visible)
                    .HasDefaultValue(true);
                entity.Property(m => m.MeasurementData)
                    .HasDefaultValue("{}");
                entity.Property(m => m.Unit)
                    .HasMaxLength(16)
                    .HasDefaultValue("mm");
                entity.Property(m => m.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(m => m.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                //创建索引
                modelBuilder.Entity<Measurement>()
                            .HasIndex(m => new { m.UserId, m.FileId, m.StudyInstanceUid, m.SeriesInstanceUid, m.SopInstanceUid, m.FrameNumber })
                            .HasDatabaseName("IX_Measurement_Key");
            });
        }
    }
}


