using Microsoft.EntityFrameworkCore;
using LogService.Models;

namespace LogService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<LogEntry> Logs => Set<LogEntry>();
}