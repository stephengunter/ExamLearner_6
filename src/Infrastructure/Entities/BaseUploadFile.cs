namespace Infrastructure.Entities;
public class BaseUploadFile : BaseRecord
{
	public string? Path { get; set; }

	public string? PreviewPath { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public string? Type { get; set; }

	public string? Name { get; set; }

	public string? Title { get; set; }

	public string? Description { get; set; }
}
