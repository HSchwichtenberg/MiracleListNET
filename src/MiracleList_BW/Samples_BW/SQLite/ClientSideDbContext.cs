//   Verträgt sich nicht mit aktiviertem Multi-Threading in BW :-(

//using Microsoft.EntityFrameworkCore;
//namespace LocalData;

//internal class ClientSideDbContext : DbContext
//{
// public DbSet<Log> LogSet { get; set; } = default!;

// public ClientSideDbContext(DbContextOptions<ClientSideDbContext> options)
//     : base(options)
// {
// }

// public ClientSideDbContext()
// {

// }

// protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// {
//  optionsBuilder.UseSqlite($"Filename=Log.db");
// }


// protected override void OnModelCreating(ModelBuilder modelBuilder)
// {
//  base.OnModelCreating(modelBuilder);

//  modelBuilder.Entity<Log>().HasIndex(nameof(Log.Text));

// }
//}
