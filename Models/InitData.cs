namespace Tasktrix.Models
{
	public static class InitData
	{
		public const string DatabaseFileName = "TodoSQLite.db3";

		public const SQLite.SQLiteOpenFlags Flags =
			SQLite.SQLiteOpenFlags.ReadWrite |
			SQLite.SQLiteOpenFlags.Create |
			SQLite.SQLiteOpenFlags.SharedCache;

		public static string DatabasePath
		{
			get
			{
				var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				return Path.Combine(basePath, DatabaseFileName);
			}
		}
	}
}
