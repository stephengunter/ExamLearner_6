using ApplicationCore.Models;
using Infrastructure.Views;
using Microsoft.AspNetCore.Http;
using ApplicationCore.Helpers;

namespace ApplicationCore.Views;
public class AttachmentViewModel : BaseRecordView
{
	public int PostId { get; set; }

	public string? PostType { get; set; }

	public string? Type { get; set; }

	public string? Path { get; set; }

	public string? PreviewPath { get; set; }

	public string? Name { get; set; }

	public string? Title { get; set; }


	public int Width { get; set; }

	public int Height { get; set; }


}

public class UploadForm
{
	public string? PostType { get; set; }

	public int PostId { get; set; }
	public List<IFormFile>? Files { get; set; }


	public PostType GetPostType()
	{
		if (String.IsNullOrEmpty(this.PostType)) return Models.PostType.None;

		try
		{
			var type = this.PostType.ToEnum<PostType>();
			return type;
		}
		catch (Exception)
		{
			return Models.PostType.None;
		}
	}

}

