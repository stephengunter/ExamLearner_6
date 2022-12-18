using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views;

public class SetPasswordRequest
{
	[Required(ErrorMessage = "必須填寫密碼")]
	[StringLength(12, ErrorMessage = "密碼長度超出限制")]
	public string Password { get; set; } = string.Empty;
}
