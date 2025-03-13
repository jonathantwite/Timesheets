using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace AggregatedTimeDatabase;

public class AggregatedTimeContext(DbContextOptions<AggregatedTimeContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobTotal>().HasKey(jt => new { jt.JobId, jt.UserId });
    }

    public DbSet<Job> Jobs { get; }
    public DbSet<User> Users { get; }
}
