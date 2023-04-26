using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings;

public class CategoryMap : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.UrlSlug)
            .HasMaxLength (50)
            .IsRequired();

        builder.Property(c => c.ShowOnMenu)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
