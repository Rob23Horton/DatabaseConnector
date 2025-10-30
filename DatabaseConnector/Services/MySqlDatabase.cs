using MySqlConnector;

namespace DatabaseConnector.Services;

public class MySqlDatabase : 
	Database<MySqlDatabase, MySqlConnection>, 
	IMySqlDatabaseServerSelectionStage, 
	IMySqlDatabaseDatabaseSelectionStage,
	IMySqlDatabaseAuthenticationSelectionStage
{
	public string ServerAddress { get; private set; } = string.Empty;
	public string DatabaseName { get; private set; } = string.Empty;
	
	protected bool IntegratedSecurity { get; set; }
	protected string? LoginUsername { get; set; }
	protected string? LoginPassword { get; set; }
	
	public static IMySqlDatabaseServerSelectionStage CreateConnection() => new MySqlDatabase();
	
	protected override void Connect(out MySqlConnection conn)
	{
		throw new NotImplementedException();
	}

	public IMySqlDatabaseDatabaseSelectionStage ForServer(string server)
	{
		ServerAddress = server;
		return this;
	}

	public IMySqlDatabaseAuthenticationSelectionStage ForDatabase(string database)
	{
		DatabaseName = database;
		return this;
	}

	public IConnectStage<MySqlDatabase> AsLogin(string username, string password)
	{
		IntegratedSecurity = false;
		LoginUsername = username;
		LoginPassword = password;
		return this;
	}

	public IConnectStage<MySqlDatabase> AsIntegratedSecurity()
	{
		IntegratedSecurity = true;
		return this;
	}
}


public interface IMySqlDatabaseServerSelectionStage
{
	public IMySqlDatabaseDatabaseSelectionStage ForServer(string server);
}

public interface IMySqlDatabaseDatabaseSelectionStage
{
	public IMySqlDatabaseAuthenticationSelectionStage ForDatabase(string database);
}

public interface IMySqlDatabaseAuthenticationSelectionStage
{
	public IConnectStage<MySqlDatabase> AsLogin(string username, string password);
	public IConnectStage<MySqlDatabase> AsIntegratedSecurity();
}
