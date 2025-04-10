using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace AggregatedTimeDatabase;

public class AggregatedTimeContext(DbContextOptions<AggregatedTimeContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobTotal>(jobTotal =>
        {
            jobTotal.HasKey(jt => new { jt.JobId, jt.UserId });
        });
        modelBuilder.Entity<Job>(job => {
            job.Property(j => j.Id).ValueGeneratedNever();
            job.Property(j => j.Description).HasMaxLength(500);
        });
        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.Id).ValueGeneratedNever();
            user.Property(u => u.Name).HasMaxLength(200);
        });
    }

    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<JobTotal> JobTotals { get; set; } = null!;
    public DbSet<Overtime> OvertimeRecords { get; set; } = null!;
}
