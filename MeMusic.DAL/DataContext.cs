using DAL.Entites;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attach> Attaches => Set<Attach>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostContent> PostContents => Set<PostContent>();

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("Main"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasIndex(f => f.Email)
                .IsUnique();

            modelBuilder
                .Entity<Avatar>()
                .ToTable(nameof(Avatars));

            modelBuilder
                .Entity<PostContent>()
                .ToTable(nameof(PostContent));
        }
    }
}
