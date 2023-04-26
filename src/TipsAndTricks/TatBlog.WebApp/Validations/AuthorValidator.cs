using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations;

public class AuthorValidator : AbstractValidator<AuthorEditModel>
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorValidator(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;

        RuleFor(a => a.FullName)
        .NotEmpty()
        .WithMessage("Tên tác giả không được để trống")
        .MaximumLength(500)
        .WithMessage("Tên tác giả dài tối đa '{MaxLength}'");

        RuleFor(a => a.UrlSlug)
        .NotEmpty()
        .WithMessage("Slug của tác giả không được để trống")
        .MaximumLength(1000)
        .WithMessage("Slug dài tối đa '{MaxLength}'");

        RuleFor(a => a.UrlSlug)
        .MustAsync(async (slug, cancellationToken) => !await _authorRepository.CheckAuthorSlugExisted(0, slug, cancellationToken))
        .WithMessage("Slug '{PropertyValue}' đã được sử dụng");

        RuleFor(a => a.Email)
        .NotEmpty()
        .WithMessage("Email của tác giả không được để trống")
        .EmailAddress()
        .WithMessage("Phải là một email");

        When(a => a.Id <= 0, () => {
            RuleFor(a => a.ImageFile)
            .Must(f => f is { Length: > 0 })
            .WithMessage("Bạn phải chọn hình ảnh cho tác giả");
        })
        .Otherwise(() => {
            RuleFor(a => a.ImageFile)
        .MustAsync(SetImageIfNotExist)
        .WithMessage("Bạn phải chọn hình ảnh cho tác giả");
        });
    }

    private async Task<bool> SetImageIfNotExist(AuthorEditModel editModel, IFormFile imageFile, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetAuthorByIdAsync(editModel.Id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(author?.ImageUrl))
            return true;

        return imageFile is { Length: > 0 };
    }
}
