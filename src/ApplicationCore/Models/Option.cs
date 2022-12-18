using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class Option : EntityBase
{
	public string Title { get; set; } = String.Empty;
	public bool Correct { get; set; }
	public int QuestionId { get; set; }

	[Required]
	public virtual Question? Question { get; set; }

	[NotMapped]
	public ICollection<UploadFile>? Attachments { get; set; }


	public void LoadAttachments(IEnumerable<UploadFile> uploadFiles)
	{
		var attachments = uploadFiles.Where(x => x.PostType == PostType.Option && x.PostId == Id);
		this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();
	}

}
