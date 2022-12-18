using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Entities;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models;
public class Article : BaseRecord
{
	public int CategoryId { get; set; }
	public string? Title { get; set; }
	public string? Content { get; set; }
	public string? Summary { get; set; }

	[Required]
	public virtual Category? Category { get; set; }


	[NotMapped]
	public virtual ICollection<UploadFile>? Attachments { get; set; }

	public void LoadAttachments(IEnumerable<UploadFile> uploadFiles)
	{
		var attachments = uploadFiles.Where(x => x.PostType == PostType.Article && x.PostId == Id);
		this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();
	}
}
