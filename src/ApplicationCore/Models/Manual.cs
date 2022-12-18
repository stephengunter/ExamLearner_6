using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models;

public class Manual : BaseCategory
{
	public bool Free { get; set; }

	public string? Summary { get; set; }

	public string? Content { get; set; }

	public virtual ICollection<Feature>? Features { get; set; } = new List<Feature>();

	[NotMapped]
	public ICollection<Manual>? SubItems { get; private set; }

	[NotMapped]
	public ICollection<UploadFile>? Attachments { get; set; }

	#region Helpers

	public void LoadAttachments(IEnumerable<UploadFile> uploadFiles)
	{
		var attachments = uploadFiles.Where(x => x.PostType == PostType.Manual && x.PostId == Id);
		this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();
	}

	public ICollection<int> GetSubIds()
	{
		var subIds = new List<int>();
		if (SubItems!.IsNullOrEmpty()) return subIds;

		foreach (var item in SubItems!)
		{
			subIds.Add(item.Id);

			subIds.AddRange(item.GetSubIds());
		}
		return subIds;
	}
	public void LoadSubItems(IEnumerable<Manual> subItems)
	{
		SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

		foreach (var item in SubItems)
		{
			item.LoadSubItems(subItems);
		}
	}
	public void LoadParentIds(IEnumerable<Manual> allManuals)
	{
		var parentIds = new List<int>();
		Manual root = null;
		if (ParentId > 0)
		{
			int parentId = ParentId;

			do
			{
				var parent = allManuals.Where(item => item.Id == parentId).FirstOrDefault();
				if (parent == null) throw new Exception($"Manual not found. id = {parentId}");

				if (parent.IsRootItem) root = parent;
				parentId = parent.Id;
				parentIds.Insert(0, parentId);

			} while (root == null);
		}
	}

	#endregion
}


public class Feature : BaseRecord
{
	public int ManualId { get; set; }

	public string? Key { get; set; }

	public string Title { get; set; } = string.Empty;

	public string? Summary { get; set; }

	public string? Content { get; set; }

	[NotMapped]
	public ICollection<UploadFile>? Attachments { get; set; }
}
