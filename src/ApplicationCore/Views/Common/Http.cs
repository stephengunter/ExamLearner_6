using Newtonsoft.Json;
using System.Text;

namespace ApplicationCore.Views;

public class JsonContent : StringContent
{
	public JsonContent(object obj) :
		 base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
	{ }
}
