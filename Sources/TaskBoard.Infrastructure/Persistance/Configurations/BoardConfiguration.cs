using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Persistence.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("Boards");

        builder.HasKey(b => b.Id);

        builder.HasIndex(b => b.UserId);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);

        builder.Property(b => b.Description).HasMaxLength(1000);

        builder.HasMany(b => b.Tasks).WithOne().HasForeignKey(t => t.BoardId).OnDelete(DeleteBehavior.Cascade);
    }
}