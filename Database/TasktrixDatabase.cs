using SQLite;
using Tasktrix.Models;
using Tasktrix.ViewModels;

namespace Tasktrix.Database;

public class TasktrixDatabase
{
	private static SQLiteAsyncConnection Database;

	public static readonly TaskAwaiter<TasktrixDatabase> Instance =
		new(async () =>
		{
			var instance = new TasktrixDatabase();
			var result = await Database.CreateTableAsync<TaskObj>();
			return instance;
		});

	public TasktrixDatabase()
	{
		Database = new SQLiteAsyncConnection(InitData.DatabasePath, InitData.Flags);
	}

	public Task<List<TaskObj>> GetItemsAysnc()
	{
		return Database.Table<TaskObj>().ToListAsync();
	}

	public Task<List<TaskObj>> GetItemsNotDoneAysnc()
	{
		return Database.QueryAsync<TaskObj>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
	}

	public Task<List<TaskObj>> GetItemsDoneAsync()
	{
		return Database.QueryAsync<TaskObj>("SELECT * FROM [TodoItem] WHERE [Done] = 1");
	}

	public Task<List<TaskObj>> GetItemsByPriorityAsync(string priority)
	{
		return Database.QueryAsync<TaskObj>("SELECT * FROM [TodoItem] WHERE [Priority] = '" + priority + "'");
	}

	public Task<List<TaskObj>> GetItemsByDateAsync(DateTime date)
	{
		return Database.QueryAsync<TaskObj>("SELECT * FROM [TodoItem] WHERE [Date] = '" + date + "'");
	}

	public Task<List<TaskObj>> GetItemsByDateRangeAsync(DateTime startDate, DateTime endDate)
	{
		return Database.QueryAsync<TaskObj>("SELECT * FROM [TodoItem] WHERE [Date] BETWEEN '" + startDate + "' AND '" +
		                                     endDate + "'");
	}

	public Task<int> CountItemsWithAttachmentAsync(bool hasAttachment)
	{
		return Database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [TodoItem] WHERE [HasAttachment] = '" +
		                                        hasAttachment + "'");
	}


	public Task<TaskObj> GetItemAsync(int id)
	{
		return Database.Table<TaskObj>().Where(i => i.ID == id).FirstOrDefaultAsync();
	}

	public Task<List<TaskObj>> RefreshDataAsync()
	{
		return Database.QueryAsync<TaskObj>("SELECT * FROM [TodoItem]");
	}

	public Task<int> SaveItemAsync(TaskObj item)
	{
		if (item.ID != 0)
			return Database.UpdateAsync(item);
		return Database.InsertAsync(item);
	}

	public Task<int> DeleteItemAsync(TaskObj item)
	{
		return Database.DeleteAsync(item);
	}
}