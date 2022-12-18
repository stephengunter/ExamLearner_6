namespace ApplicationCore;

public class SettingsKeys
{
    public static string AppSettings = "AppSettings";
    public static string AuthSettings = "AuthSettings";
    public static string AdminSettings = "AdminSettings";
    public static string EcPaySettings = "EcPaySettings";
    public static string RootSubjectSettings = "RootSubjectSettings";
    public static string SubscribesSettings = "SubscribesSettings";
    public static string CloudStorageSettings = "CloudStorageSettings";
}
public enum AppRoles
{
    Boss,
    Dev,
    Subscriber
}
public static class JwtClaimIdentifiers
{
	public const string Rol = "rol";
	public const string Id = "id";
	public const string Sub = "sub";
	public const string Roles = "roles";
	public const string Provider = "provider";
	public const string Picture = "picture";
	public const string Name = "name";
}

public static class JwtClaims
{
	public const string ApiAccess = "api_access";
}

public enum PaymentTypes
{
	CREDIT,
	ATM
}

public enum ThirdPartyPayment
{
	EcPay
}
public enum HttpClients
{
	Google
}
