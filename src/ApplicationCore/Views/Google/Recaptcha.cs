using Newtonsoft.Json;

namespace ApplicationCore.Views;

public class GoogleRecaptchaResponse
{
	public bool Success { get; set; }

	[JsonProperty("challenge_ts")]
	public string? ChallengeTime { get; set; }

	[JsonProperty("hostname")]
	public string? Hostname { get; set; }

	[JsonProperty("error-codes")]
	public IList<string>? ErrorCodes { get; set; }

}
