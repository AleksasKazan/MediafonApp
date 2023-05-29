using MediafonApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MediafonApp
{
    public class MediafonDbContext : DbContext //The class inherits from DbContext, which is a base class provided by Entity Framework Core for working with databases
    {
        // Constructor for the database context that accepts DbContextOptions
        public MediafonDbContext(DbContextOptions<MediafonDbContext> options) : base(options)
        {
        }

        // DbSet representing the "Files" table in the database
        // It allows querying, inserting, updating, and deleting records in the "Files" table
        public DbSet<FileRecordModel> Files { get; set; }
    }
}