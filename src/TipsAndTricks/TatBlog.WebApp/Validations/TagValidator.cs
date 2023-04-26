using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations;

public class TagValidator : AbstractValidator<TagEditModel>
{
    private readonly ITagRepository _tagRepository;

    public TagValidator(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;

        RuleFor(a => a.Name)
        .NotEmpty()
        .WithMessage("Tên thẻ không được để trống")
        .MaximumLength(30)
        .WithMessage("Tên thẻ dài tối đa '{MaxLength}'");

        RuleFor(a => a.UrlSlug)
        .NotEmpty()
        .WithMessage("Slug của thẻ không được để trống")
        .MaximumLength(1000)
        .WithMessage("Slug dài tối đa '{MaxLength}'");

        RuleFor(a => a.UrlSlug)
        .MustAsync(async (slug, cancellationToken) => !await _tagRepository.CheckTagSlugExisted(0, slug, cancellationToken))
        .WithMessage("Slug '{PropertyValue}' đã được sử dụng");

        RuleFor(a => a.Description)
        .NotEmpty()
        .WithMessage("Mô tả của thẻ không được để trống");
    }
}
