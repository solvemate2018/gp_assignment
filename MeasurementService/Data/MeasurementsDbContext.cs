using Microsoft.EntityFrameworkCore;
using Models;

namespace MeasurementService.Data;

public class MeasurementsDbContext : DbContext
{
    public virtual DbSet<Measurement> Measurements { get; set; }

    public MeasurementsDbContext(DbContextOptions<MeasurementsDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public MeasurementsDbContext() : base()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Measurement>().HasKey(m => m.Id);
        modelBuilder.Entity<Measurement>().Property(m => m.PatientSSN).HasMaxLength(10).IsRequired();
        modelBuilder.Entity<Measurement>().Property(m => m.Date);
    }
}