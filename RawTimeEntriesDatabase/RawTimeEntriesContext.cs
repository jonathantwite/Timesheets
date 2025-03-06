using Microsoft.EntityFrameworkCore;
using RawTimeEntriesDatabase.Entities;

namespace RawTimeEntriesDatabase;

public class RawTimeEntriesContext(DbContextOptions<RawTimeEntriesContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {

    }

    public DbSet<TimeEntry> TimeEntries { get; set; }
}
