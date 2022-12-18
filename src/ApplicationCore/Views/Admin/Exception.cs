namespace ApplicationCore.Views;

public class ExceptionViewModel
{
	public ExceptionViewModel() { }

	public ExceptionViewModel(Exception ex)
	{
		TypeName = ex.GetType().Name;
		Content = $"{ex}";
	}

	public string Id { get; set; } = Guid.NewGuid().ToString();

	public string? TypeName { get; set; }

	public string? Content { get; set; }

	public DateTime DateTime { get; set; } = DateTime.Now;

	public bool Checked { get; set; }

	public string DateTimeText => DateTime.ToString("yyyy-MM-dd H:mm:ss");

}
