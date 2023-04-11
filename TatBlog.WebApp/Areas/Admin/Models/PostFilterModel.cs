using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TatBlog.WebApp.Areas.Admin.Models;

public class PostFilterModel : SearchModel
{
    [DisplayName("Từ khóa")]
    public string Keyword { get; set; }

    [DisplayName("Đã xuất bản")]
    public bool PublishedOnly { get; set; }

    [DisplayName("Chưa xuất bản")]
    public bool NotPublished { get; set; }

    [DisplayName("Tác giả")]
    public int? AuthorId { get; set; }

    [DisplayName("Chủ đề")]
    public int? CategoryId { get; set; }

    [DisplayName("Năm")]
    public int? Year { get; set; }

    [DisplayName("Tháng")]
    public int? Month { get; set; }

    [DisplayName("Ngày")]
    public int? Day { get; set; }

    public IEnumerable<SelectListItem> AuthorList { get; set; }
    public IEnumerable<SelectListItem> CategoryList { get; set; }
    public IEnumerable<SelectListItem> MonthList { get; set; }

    public PostFilterModel()
    {
        MonthList = Enumerable.Range(1, 12)
                              .Select(m => new SelectListItem()
                              {
                                  Value = m.ToString(),
                                  Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
                              }).ToList();
    }
}
