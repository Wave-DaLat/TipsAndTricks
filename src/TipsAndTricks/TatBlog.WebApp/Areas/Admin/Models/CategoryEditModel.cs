using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace TatBlog.WebApp.Areas.Admin.Models;

public class CategoryEditModel
{
    public int Id { get; set; }

    [DisplayName("Tên chủ đề")]
    public string Name { get; set; }

    [DisplayName("Nội dung")]
    public string Description { get; set; }

    [DisplayName("Slug")]
    [Remote("VerifyCategorySlug", "Categories", "Admin", HttpMethod = "POST", AdditionalFields = "Id")]
    public string UrlSlug { get; set; }

    [DisplayName("Hiển thị trên Menu")]
    public bool ShowOnMenu { get; set; }
}
