using Microsoft.EntityFrameworkCore;
using Models;

namespace PatientService.Data;

public class PatientDbContext : DbContext
{
    public virtual DbSet<Patient> Patients { get; set; }

    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public PatientDbContext() : base()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasKey(p => p.Ssn);
        modelBuilder.Entity<Patient>().Property(p => p.Email).IsRequired();
        modelBuilder.Entity<Patient>().Property(p => p.Name).IsRequired();
    }
}