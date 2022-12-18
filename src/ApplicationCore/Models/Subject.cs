using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models;
public class Subject : BaseCategory
{
	public virtual ICollection<Term>? Terms { get; set; }

	public virtual ICollection<Question>? Questions { get; set; }
	

	[NotMapped]
	public ICollection<Subject>? SubItems { get; private set; }

	[NotMapped]
	public ICollection<int>? SubIds { get; private set; }


	public void LoadSubItems(IEnumerable<Subject> subItems)
	{
		SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

		foreach (var item in SubItems)
		{
			item.LoadSubItems(subItems);
		}
	}

	public ICollection<int> GetSubIds()
	{
		var subIds = new List<int>();
		if (SubItems!.HasItems())
		{
			foreach (var item in SubItems!)
			{
				subIds.Add(item.Id);

				subIds.AddRange(item.GetSubIds());
			}
		}
		

		this.SubIds = subIds;
		return subIds;
	}

	public List<int> GetQuestionIds()
	{
		if (Terms.IsNullOrEmpty()) return new List<int>();
		return Terms!.SelectMany(item => item.GetQuestionIds()).Distinct().ToList();
	}
}
