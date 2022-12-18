namespace ApplicationCore.Hubs;
public interface IHubConnectionManager
{
	void KeepUserConnection(string user, string connectionId);
	void RemoveUserConnection(string connectionId);
	List<string> GetUserConnections(string user);

}

public class HubConnectionManager : IHubConnectionManager
{
	private static Dictionary<string, List<string>> _userConnectionMap = new Dictionary<string, List<string>>();
	private static string _userConnectionMapLocker = string.Empty;

	public void KeepUserConnection(string userId, string connectionId)
	{
		lock (_userConnectionMapLocker)
		{
			if (!_userConnectionMap.ContainsKey(userId))
			{
				_userConnectionMap[userId] = new List<string>();
			}
			_userConnectionMap[userId].Add(connectionId);
		}
	}

	public void RemoveUserConnection(string connectionId)
	{
		//Remove the connectionId of user 
		lock (_userConnectionMapLocker)
		{
			foreach (var userId in _userConnectionMap.Keys)
			{
				if (_userConnectionMap.ContainsKey(userId))
				{
					if (_userConnectionMap[userId].Contains(connectionId))
					{
						_userConnectionMap[userId].Remove(connectionId);
						break;
					}
				}
			}
		}
	}
	public List<string> GetUserConnections(string userId)
	{
		var conn = new List<string>();
		try
		{
			lock (_userConnectionMapLocker)
			{
				conn = _userConnectionMap[userId];
			}
		}
		catch
		{
			conn = new List<string>();
		}

		return conn;
	}
}
