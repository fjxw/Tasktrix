using Tasktrix.Models;
using Tasktrix.Database;
namespace Tasktrix.Views;

public partial class TaskPageView
{
	public TaskPageView()
	{
		InitializeComponent();
		todolistlist = listView;
	}

	public ListView todolistlist;

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await UpdateListView();
		GetDoneItems();
		await UpdateListView();
		await IsEmptyList();
	}


	public bool EmptyList { get; set; }

	private async Task IsEmptyList()
	{
		var database = await TasktrixDatabase.Instance;
		var items = await database.GetItemsAysnc();
		if (items.Count == 0)
		{
			VStack.IsVisible = true;
			listView.IsVisible = false;
			Console.WriteLine("Список пуст");
		}
		else
		{
			listView.IsVisible = true;
			VStack.IsVisible = false;
			Console.WriteLine("Список не пуст");
		}
	}


	private async Task UpdateListView()
	{
		var database = await TasktrixDatabase.Instance;
		listView.ItemsSource = await database.GetItemsAysnc();

		Console.WriteLine("Обновление успешно");
	}

	private async void OnItemAdded(object sender, EventArgs e)
	{
		HapticFeedback.Perform();

		await Navigation.PushAsync(new TaskObjPageView
		{
			BindingContext = new TaskObj()
		});
	}


	private async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		if (e.SelectedItem != null)
			await Navigation.PushAsync(new TaskObjPageView
			{
				BindingContext = e.SelectedItem as TaskObj
			});
	}


	private void GetDoneItems()
	{
		var doneItems = ((IEnumerable<TaskObj>)listView.ItemsSource).Count(item => item.Done);
		var total = listView.ItemsSource?.Cast<object>().Count() ?? 0;
		var notDone = total - doneItems;

		Console.WriteLine(doneItems);
		Console.WriteLine(notDone);
		Console.WriteLine(total);
	}

	private void OnCheckBoxChecked(object sender, EventArgs e)
	{
		var checkBox = (CheckBox)sender;
		var todoItem = checkBox.BindingContext as TaskObj;
		if (todoItem != null) todoItem.IsSelected = checkBox.IsChecked;
	}

	private void OnCheckBoxUnchecked(object sender, EventArgs e)
	{
		var checkBox = (CheckBox)sender;
		var todoItem = checkBox.BindingContext as TaskObj;
		if (todoItem != null) todoItem.IsSelected = checkBox.IsChecked;
	}

	private async void DeleteSelectedItems(object sender, EventArgs e)
	{
		var selectedItems = listView.ItemsSource?.Cast<TaskObj>().Where(item => item.IsSelected).ToList();

		if (!selectedItems.Any())
		{
			await DisplayAlert("Не выбрано задач", "Выберите задачи для удаления", "OK");
		}
		else
		{
			var Confirmed = await DisplayAlert("Удаление задач", "Вы точно хотите удалить выбранные задачи?", "Да",
				"Нет");

			if (Confirmed)
			{
				var database = await TasktrixDatabase.Instance;
				foreach (var item in selectedItems) await database.DeleteItemAsync(item);


				await IsEmptyList();
				await UpdateListView();
			}
		}
	}


	private bool sortByDateAscending = false;


	public async void OpenSortMenu(object sender, EventArgs e)
	{
		var sortbydate = "Недавние";
		var sortbypriority = "Ниже приоритет";
		var clearsorting = "Очистить";


		var action = await Application.Current.MainPage.DisplayActionSheet("Сортировка", "Отмена", null, sortbydate,
			sortbypriority, clearsorting);

		if (action != null && action.Equals(sortbydate))
		{
			var sortedItems = ((IEnumerable<TaskObj>)listView.ItemsSource).OrderByDescending(item => item.Date);
			listView.ItemsSource = sortedItems.ToList();
		}
		else if (action != null && action.Equals(sortbypriority))
		{
			var sortedItems = ((IEnumerable<TaskObj>)listView.ItemsSource).OrderBy(item => item.Priority);
			listView.ItemsSource = sortedItems.ToList();
		}
		else if (action != null && action.Equals(clearsorting))
		{
			listView.ItemsSource = ((IEnumerable<TaskObj>)listView.ItemsSource).ToList();
			await UpdateListView();
		}
	}


	private async void MarkSelectedItemsComplete(object sender, EventArgs e)
	{
		var selectedItems = listView.ItemsSource?.Cast<TaskObj>().Where(item => item.IsSelected).ToList();

		if (!selectedItems.Any())
		{
			await DisplayAlert("Не выбрано задачи", "Необходимо выбрать задачу", "OK");
		}
		else
		{
			foreach (var item in selectedItems)
			{
				item.Done = true;
				item.IsSelected = false;
			}

			var database = await TasktrixDatabase.Instance;
			foreach (var item in selectedItems) await database.SaveItemAsync(item);

			await UpdateListView();
		}
	}

	private async void SetSelectedItemPriority(object sender, EventArgs e)
	{
		var selectedItems = listView.ItemsSource?.Cast<TaskObj>().Where(item => item.IsSelected).ToList();

		if (!selectedItems.Any())
		{
			await DisplayAlert("Не выбрано задач", "Для указания приоритета задачи необходимо ее выбрать", "OK");
		}
		else
		{
			var action = await Application.Current.MainPage.DisplayActionSheet("Установить приоритет", "Отмена",
				null, "Низкий", "Средний", "Высокий");

			if (action != null)
			{
				foreach (var item in selectedItems)
				{
					item.Priority = action;
					item.IsSelected = false;
				}

				var database = await TasktrixDatabase.Instance;
				foreach (var item in selectedItems) await database.SaveItemAsync(item);

				await UpdateListView();
			}
		}
	}

	private async void RefreshView_Refreshing(object sender, EventArgs e)
	{
		listView.IsRefreshing = true;
		await UpdateListView();
		await Task.Delay(1000);
		listView.IsRefreshing = false;
	}
}