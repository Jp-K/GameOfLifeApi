using Microsoft.EntityFrameworkCore;
using GameOfLifeApi.Models;

namespace GameOfLifeApi.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Cell> Cells { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cell>()
            .HasIndex(c => new { c.Iteration, c.X, c.Y, c.Uuid })
            .IsUnique();
    }
}