using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;

public class Question : BaseRecord
{
	public string Title { get; set; } = string.Empty;
	public bool MultiAnswers { get; set; }
	public int SubjectId { get; set; }
	public virtual Subject? Subject { get; set; }
	public string? TermIds { get; set; }//  Example: 1,6

	public virtual ICollection<Option> Options { get; set; } = new List<Option>();

	public virtual ICollection<Resolve>? Resolves { get; set; } = new List<Resolve>();

	public virtual ICollection<RecruitQuestion>? RecruitQuestions { get; set; } = new List<RecruitQuestion>();


	[NotMapped]
	public ICollection<UploadFile>? Attachments { get; set; }

	[NotMapped]
	public ICollection<Recruit>? Recruits
	{
		get
		{
			if (this.RecruitQuestions.IsNullOrEmpty()) return null;
			return this.RecruitQuestions!.Select(item => item.Recruit).ToList()!;
		}
	}


	[NotMapped]
	public ICollection<Term>? Terms { get; private set; }

	public void LoadTerms(IEnumerable<Term> allTerms)
	{
		var termIds = TermIds.SplitToIds();
		this.Terms = allTerms.Where(x => termIds.Contains(x.Id)).ToList();
	}
	public void LoadAttachments(IEnumerable<UploadFile> uploadFiles)
	{
		var attachments = uploadFiles.Where(x => x.PostType == PostType.Question && x.PostId == Id);
		this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();
	}

}
