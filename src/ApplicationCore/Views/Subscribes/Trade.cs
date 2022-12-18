namespace ApplicationCore.Views;

public class TradeRequestModel
{
	public string? AppId { get; set; }

	public string? Code { get; set; }

	public int Amount { get; set; }

	public int ValidDays { get; set; }
}

public class TradeResultModel
{
	public string? Provider { get; set; }

	public string? PayWay { get; set; }

	public string Code { get; set; } = String.Empty;

	public bool Simulate { get; set; }

	public string? TradeNo { get; set; }

	public int Amount { get; set; }

	public string? BankCode { get; set; }

	public string? BankAccount { get; set; }

	public string? ExpireDate { get; set; }

	public bool Payed { get; set; }

	public string? PayedDate { get; set; }

	public string? Data { get; set; } //json string

}
