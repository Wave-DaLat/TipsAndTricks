using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings;

public class CommentMap : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.PostDate)
               .IsRequired()
               .HasColumnType("datetime");

        builder.Property(c => c.Content)
               .IsRequired()
               .HasMaxLength(5000);

        builder.Property(c => c.Censored)
               .IsRequired()
               .HasDefaultValue(false);

        builder.HasOne(c => c.Post)
               .WithMany(p => p.Comments)
               .HasForeignKey(c => c.PostID)
               .HasConstraintName("FK_Posts_Comments")
               .OnDelete(DeleteBehavior.Cascade);
    }
}