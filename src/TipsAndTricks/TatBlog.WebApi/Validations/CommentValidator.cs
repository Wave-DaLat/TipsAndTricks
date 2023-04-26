using FluentValidation;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations;

public class CommentValidator : AbstractValidator<CommentEditModel>
{
    public CommentValidator()
    {
        RuleFor(a => a.UserName)
            .NotEmpty()
            .WithMessage("Tên độc giả không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên độc giả dài tối đa '{MaxLength}' kí tự");

        RuleFor(a => a.Content)
            .NotEmpty()
            .WithMessage("Nội dung bình luận không được để trống")
            .MaximumLength(1000)
            .WithMessage("Nội dung bình luận dài tối đa '{MaxLength}' kí tự");

        RuleFor(a => a.PostDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Ngày bình luận không hợp lệ");
    }
}
