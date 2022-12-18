using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class Category : BaseCategory
{
    public string? Key { get; set; }
    public virtual ICollection<Article>? Articles { get; set; }
}

public static class CategoryKeys
{
    public static string Experience = "Experience";
}
