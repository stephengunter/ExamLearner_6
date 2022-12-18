using Infrastructure.Entities;
using ApplicationCore.Helpers;
using System.ComponentModel.DataAnnotations.Schema;


namespace ApplicationCore.Models;

public class Recruit : BaseCategory
{
	public int Year { get; set; }

	public DateTime? Date { get; set; }

	public bool Done { get; set; }

	public int SubjectId { get; set; }

	public int Points { get; set; }

	public OptionType OptionType { get; set; }

	public int OptionCount { get; set; }

	public bool MultiAnswers { get; set; }

	public string? PS { get; set; }

	public virtual ICollection<RecruitQuestion>? RecruitQuestions { get; set; }


	#region Helpers

	[NotMapped]
	public RecruitEntityType RecruitEntityType
	{
		get
		{
			if (ParentId == 0) return RecruitEntityType.Year;
			if (SubjectId > 0 && ParentId > 0) return RecruitEntityType.SubItem;
			if (ParentId > 0 && SubjectId == 0) return RecruitEntityType.Part;

			return RecruitEntityType.Unknown;
		}
	}

	[NotMapped]
	public Recruit? Parent { get; set; }

	[NotMapped]
	public Subject? Subject { get; set; }

	[NotMapped]
	public ICollection<int> SubjectIds { get; set; } = new List<int>();

	[NotMapped]
	public ICollection<Recruit>? SubItems { get; private set; }

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
		return subIds;
	}


	public void LoadSubItems(IEnumerable<Recruit> subItems)
	{
		SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

		foreach (var item in SubItems)
		{
			item.LoadSubItems(subItems);
		}
	}

	public void LoadParent(IEnumerable<Recruit> allItems) => Parent = GetParent(allItems);

	public Recruit GetParent(IEnumerable<Recruit> allItems) => allItems.FirstOrDefault(x => x.Id == ParentId)!;

	public int GetYear()
	{
		if (RecruitEntityType == RecruitEntityType.Year) return Year;
		if (RecruitEntityType == RecruitEntityType.SubItem) return Parent != null ? Parent.Year : 0;
		return 0;
	}

	public void LoadParents(IEnumerable<Recruit> allItems)
	{
		var entity = this;
		Recruit? root = null;
		if (ParentId > 0)
		{

			do
			{
				entity.LoadParent(allItems);
				if (entity.Parent == null) throw new Exception($"Recruit not found. id = {ParentId}");

				if (entity.Parent.IsRootItem) root = entity.Parent;
				else entity = entity.Parent;

			} while (root == null);
		}

	}

	#endregion

}


public enum RecruitEntityType
{
	Year = 0,
	SubItem = 1,
	Part = 2,
	Unknown = -1
}

