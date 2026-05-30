using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Board>(builder =>
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasMany(b => b.Columns)
                .WithOne(c => c.Board)
                .HasForeignKey(c => c.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(b => b.Columns)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Column>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Order)
                .IsRequired();

            builder.HasMany(c => c.Tasks)
                .WithOne(t => t.Column)
                .HasForeignKey(t => t.ColumnId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Tasks)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<TaskItem>(builder =>
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Order)
                .IsRequired();
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.ExpiresAt)
                .IsRequired();

            entity.Property(x => x.Revoked)
                .IsRequired();

            entity.HasIndex(x => x.Token)
                .IsUnique();
        });
    }
}
