using Microcharts;
using SkiaSharp;
using Tasktrix.Database;
using Tasktrix.Models;

namespace Tasktrix.Views;

[XamlCompilation(XamlCompilationOptions.Skip)]
public partial class AnalysisPageView : ContentPage
{
	private int doneItems;
	private ChartEntry[] entries;
	private int notDone;
	private int totalItems;

	public AnalysisPageView()
	{
		InitializeComponent();
		GetTotalItems();
	}

	private async Task UpdateListView()
	{
		var database = await TasktrixDatabase.Instance;
		ListView.ItemsSource = await database.GetItemsAysnc();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await UpdateListView();
		GetTotalItems();
		GetDoneItems();
		await Task.Delay(100);
		PieChart();
		DonutChart();
		UpdateLabel();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		PieChartView.Chart = null;
		DonutChartView.Chart = null;
	}

	private void UpdateLabel()
	{
		var totalTodoItems = ListView.ItemsSource?.Cast<object>().Count() ?? 0;
		Todoitems.Text = $"{totalItems} Всего";

		var lowPriorityItems = ((IEnumerable<TaskObj>)ListView.ItemsSource).Count(item => item.Priority == "Низкий");
		Lowpriority.Text = $"{lowPriorityItems} Низкий";

		var mediumPriorityItems =
			((IEnumerable<TaskObj>)ListView.ItemsSource).Count(item => item.Priority == "Средний");
		Mediumpriority.Text = $"{mediumPriorityItems} Средний";


		var highPriorityItems = ((IEnumerable<TaskObj>)ListView.ItemsSource).Count(item => item.Priority == "Высокий");
		Highpriority.Text = $"{highPriorityItems} Высокий";
	}

	private void GetTotalItems()
	{
		totalItems = ListView.ItemsSource?.Cast<object>().Count() ?? 0;
	}

	private void GetDoneItems()
	{
		doneItems = ((IEnumerable<TaskObj>)ListView.ItemsSource).Count(item => item.Done);
		notDone = totalItems - doneItems;
	}

	private void PieChart()
	{
		var lowPriorityItems = ((IEnumerable<TaskObj>)ListView.ItemsSource).Count(item => item.Priority == "Низкий");
		var mediumPriorityItems =
			((IEnumerable<TaskObj>)ListView.ItemsSource).Count(item => item.Priority == "Средний");
		var highPriorityItems = ((IEnumerable<TaskObj>)ListView.ItemsSource).Count(item => item.Priority == "Высокий");

		entries = new[]
		{
			new ChartEntry(lowPriorityItems)
			{
				Label = "Низкий",
				Color = SKColor.Parse("#00ff00")
			},

			new ChartEntry(mediumPriorityItems)
			{
				Label = "Средний",
				Color = SKColor.Parse("#D5B60A")
			},

			new ChartEntry(highPriorityItems)
			{
				Label = "Высокий",
				Color = SKColor.Parse("#FFA500")
			}
		};
		if (entries.Any(entry => entry.Value > 0))
		{
			PieChartView.Chart = new RadarChart
			{
				Entries = entries,
				LabelTextSize = 40,
				IsAnimated = false,
				BackgroundColor = SKColor.Parse("#00FFFFFF")
			};
		}
		else
		{
			entries = new[]
			{
				new ChartEntry(1)
				{
					Label = "",
					ValueLabel = "0",
					Color = SKColor.Parse("#CCCCCC")
				}
			};

			PieChartView.Chart = new PieChart
			{
				Entries = entries,
				IsAnimated = true,
				LabelTextSize = 40,
				BackgroundColor = SKColor.Parse("#00FFFFFF"),
				LabelMode = LabelMode.None,
				GraphPosition = GraphPosition.Center,
				Margin = 0
			};
		}
	}

	private void DonutChart()
	{
		var labelColor = SKColor.Parse("#070808");


		if (doneItems + notDone > 0)
		{
			entries = new[]
			{
				new ChartEntry(doneItems)
				{
					Label = "Завершено",
					ValueLabel = doneItems.ToString(),
					Color = SKColor.Parse("#2c3e50"),
					ValueLabelColor = labelColor
				},

				new ChartEntry(notDone)
				{
					Label = "Не завершено",
					ValueLabel = notDone.ToString(),
					Color = SKColor.Parse("#ADD8E6"),
					ValueLabelColor = labelColor
				}
			};

			DonutChartView.Chart = new DonutChart
			{
				Entries = entries,
				IsAnimated = true,
				BackgroundColor = SKColor.Parse("#00FFFFFF"),
				LabelTextSize = 30,
				LabelColor = labelColor,
				GraphPosition = GraphPosition.Center
			};
		}
		else
		{
			entries = new[]
			{
				new ChartEntry(1)
				{
					Label = "Завершено",
					ValueLabel = "0",
					Color = SKColor.Parse("#CCCCCC"),
					ValueLabelColor = labelColor
				},

				new ChartEntry(1)
				{
					Label = "Не завершено",
					ValueLabel = "0",
					Color = SKColor.Parse("#CCCCCC"),
					ValueLabelColor = labelColor
				}
			};

			DonutChartView.Chart = new DonutChart
			{
				Entries = entries,
				IsAnimated = true,
				BackgroundColor = SKColor.Parse("#00FFFFFF"),
				LabelTextSize = 30,
				LabelColor = labelColor,
				GraphPosition = GraphPosition.Center
			};
		}
	}
}