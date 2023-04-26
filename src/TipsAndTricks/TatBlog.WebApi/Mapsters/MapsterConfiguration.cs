using Mapster;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Mapsters;

public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Author, AuthorDto>();
        config.NewConfig<Author, AuthorItem>()
            .Map(dest => dest.PostCount,
                src => src.Posts == null ? 0 : src.Posts.Count);

        config.NewConfig<AuthorEditModel, Author>();

        config.NewConfig<Category, CategoryDto>();
        config.NewConfig<Category, CategoryItem>()
            .Map(dest => dest.PostCount,
                src => src.Posts == null ? 0 :src.Posts.Count);

        config.NewConfig<Post, PostDto>();
        config.NewConfig<Post, PostDetail>();
        config.NewConfig<Post, PostItem>()
              .Map(dest => dest.AuthorName, src => src.Author.FullName)
              .Map(dest => dest.Tags, src => src.Tags.Select(t => t.Name));
    }
}
