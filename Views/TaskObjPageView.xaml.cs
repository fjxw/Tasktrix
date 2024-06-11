using Tasktrix.Models;
using Tasktrix.Database;

namespace Tasktrix.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TaskObjPageView
{
	public TaskObjPageView()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateToolbarTitle();
	}


	private void UpdateToolbarTitle()
	{
		if (string.IsNullOrWhiteSpace(NameField.Text))
		{
			itemTitle.FormattedText = new FormattedString();
			itemTitle.FormattedText.Spans.Add(new Span
				{ Text = "Создание новой задачи", FontAttributes = FontAttributes.Bold });
		}
		else
		{
			itemTitle.FormattedText = new FormattedString();
			itemTitle.FormattedText.Spans.Add(
				new Span { Text = "Редактирование", FontAttributes = FontAttributes.Bold });
		}
	}

	private bool isSaved;

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		Saving();
	}

	private async void Saving()
	{
		isSaved = true;
		if (string.IsNullOrWhiteSpace(NameField.Text) || string.IsNullOrWhiteSpace(DescField.Text))
		{
			isSaved = false;
			HapticFeedback.Perform(HapticFeedbackType.LongPress);
			await DisplayAlert("Невозможно сохранить задачу", "Введите название и описание", "OK");
			return;
		}

		var todoItem = (TaskObj)BindingContext;
		todoItem.Name = NameField.Text;
		todoItem.Notes = DescField.Text;
		todoItem.Date = DateTime.Now;

		HapticFeedback.Perform();
		var database = await TasktrixDatabase.Instance;
		await database.SaveItemAsync(todoItem);
		await Shell.Current.GoToAsync("..");
	}


	public async void Close(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(NameField.Text) || string.IsNullOrWhiteSpace(DescField.Text))
		{
			await Shell.Current.GoToAsync("..");
		}
		else
		{
			var Confirmed = await DisplayAlert("Задача не сохранена", "Хотите сохранить?",
				"Сохранить", "Закрыть");
			if (Confirmed)
				Saving();
			else
				await Shell.Current.GoToAsync("..");
		}
	}
}