using Microsoft.Data.Sqlite;

namespace DataBox.Services;

public class SqliteDatabase : 
	Database<SqliteDatabase, SqliteConnection>, 
	ISqliteDatabaseFileSelectionStage
{
	public string FilePath { get; private set; } = string.Empty;
	
	public static ISqliteDatabaseFileSelectionStage CreateConnection() => new SqliteDatabase();
	
	protected override void Connect(out SqliteConnection conn)
	{
		throw new NotImplementedException();
	}

	public IConnectStage<SqliteDatabase> WithFile(string path)
	{
		FilePath = path;
		return this;
	}
}

public interface ISqliteDatabaseFileSelectionStage
{
	public IConnectStage<SqliteDatabase> WithFile(string path);
}
