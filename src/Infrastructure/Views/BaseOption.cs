namespace Infrastructure.Views;
public class BaseOption<Tkey>
{
	public BaseOption(Tkey value, string text)
	{
		this.Value = value;
		this.Text = text;
	}
	public Tkey Value { get; set; }
	public string Text { get; set; }

}

