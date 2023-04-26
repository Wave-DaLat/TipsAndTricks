using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations;

public class CategoryValidator : AbstractValidator<CategoryEditModel>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(p => p.Name)
        .NotEmpty()
        .WithMessage("Tên chủ đề không được để trống")
        .MaximumLength(500)
        .WithMessage("Tên chủ đề dài tối đa '{MaxLength}'");

        RuleFor(p => p.Description)
        .NotEmpty()
        .WithMessage("Mô tả về chủ đề không được để trống");

        RuleFor(p => p.UrlSlug)
        .NotEmpty()
        .WithMessage("Slug của chủ đề không được để trống")
        .MaximumLength(1000)
        .WithMessage("Slug dài tối đa '{MaxLength}'");

        RuleFor(p => p.UrlSlug)
        .MustAsync(async (slug, cancellationToken) => !await _categoryRepository.CheckCategorySlugExisted(0, slug, cancellationToken))
        .WithMessage("Slug '{PropertyValue}' đã được sử dụng");
    }
}
