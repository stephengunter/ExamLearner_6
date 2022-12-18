using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Services;
public interface IDbService
{
	string GetDbName();
	void Migrate();
	void Backup(string fileName);
}

public class DbService : IDbService
{
	private readonly DefaultContext _context;
	private readonly string _connectionString;
	private readonly string _dbName;
	public DbService(DefaultContext context)
	{
		_context = context;
		_connectionString = context.Database.GetDbConnection().ConnectionString;
		_dbName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;
	}

	public string GetDbName() => _dbName;
	public void Migrate() => _context.Database.Migrate();

	public void Backup(string fileName)
	{
		string cmdText = $"BACKUP DATABASE [{_dbName}] TO DISK = '{fileName}'";
		using (var conn = new SqlConnection(_connectionString))
		{
			conn.Open();
			using (SqlCommand cmd = new SqlCommand(cmdText, conn))
			{
				int result = cmd.ExecuteNonQuery();

			}
			conn.Close();
		}
	}
}
