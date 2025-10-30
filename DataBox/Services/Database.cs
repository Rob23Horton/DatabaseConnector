using System.Data.Common;

namespace DataBox.Services;

public static class Database
{
	public static IMySqlDatabaseServerSelectionStage UseMySql() => MySqlDatabase.CreateConnection();
	public static ISqliteDatabaseFileSelectionStage UseSqlite() => SqliteDatabase.CreateConnection();
}

public abstract class Database<TSelf, TConnection> : 
	IConnectStage<TSelf>,
	IDisposable
	where TSelf : Database<TSelf, TConnection> 
	where TConnection : DbConnection
{
	protected TConnection? Connection { get; private set; }
	
	protected abstract void Connect(out TConnection action);
	public TSelf Connect()
	{
		Connect(out var conn);
		Connection = conn;
		Connection.Open();
		return (TSelf)this;
	}

	void IDisposable.Dispose()
	{
		Connection?.Close();
		GC.SuppressFinalize(this);
	}
}

public interface IConnectStage<T>
{
	public T Connect();
}
