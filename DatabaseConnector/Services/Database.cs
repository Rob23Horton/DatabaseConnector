namespace DatabaseConnector.Services;

public static class Database
{
	public static IMySqlDatabaseServerSelectionStage UseMySql() => MySqlDatabase.CreateConnection();
	public static ISqliteDatabaseFileSelectionStage UseSqlite() => SqliteDatabase.CreateConnection();
}

public abstract class Database<TSelf, TConnection> : IConnectStage<TSelf> where TSelf : Database<TSelf, TConnection>
{
	protected TConnection? Connection { get; private set; }
	
	protected abstract void Connect(out TConnection action);
	public TSelf Connect()
	{
		Connect(out var conn);
		Connection = conn;
		return (TSelf)this;
	}
}

public interface IConnectStage<T>
{
	public T Connect();
}
