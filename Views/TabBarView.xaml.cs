using SimpleToolkit.Core;
using Tasktrix.Models;
using Tasktrix.ViewModels;

namespace Tasktrix.Views;

public partial class TabBarView : ContentView
{
	public static readonly BindableProperty ShellItemsProperty =
		BindableProperty.Create(nameof(ShellItems), typeof(IEnumerable<BaseShellItem>), typeof(TabBarView),
			propertyChanged: ShellItemsPropertyChanged);

	private TabBarItem selectedItem;

	public TabBarView()
	{
		InitializeComponent();

		absoluteLayout.SizeChanged += AbsoluteLayoutSizeChanged;
	}

	private double itemHeight => 50;
	private double defaultItemWidth => 60;
	private double selectedItemWidth => 160;
	private double absoluteLayoutWidth => Math.Min(absoluteLayout.Width, absoluteLayout.MaximumWidthRequest);
	private double itemsSpacing => 12;

	public IEnumerable<BaseShellItem> ShellItems
	{
		get => (IEnumerable<BaseShellItem>)GetValue(ShellItemsProperty);
		set => SetValue(ShellItemsProperty, value);
	}

	public event Action<object, ShellPageType> CurrentPageSelectionChanged;

	private void UpdateControl()
	{
		if (!absoluteLayout.Children.Any())
			return;

		var totalItemsWidth = (absoluteLayout.Children.Count - 1) * defaultItemWidth + selectedItemWidth;
		var left = (absoluteLayoutWidth - totalItemsWidth) / 2;

		for (var i = 0; i < absoluteLayout.Children.Count; i++)
		{
			var itemView = absoluteLayout.Children[i] as Border;
			var width = itemView.BindingContext == selectedItem ? selectedItemWidth : defaultItemWidth;
			var rect = new Rect(left, 0, width, itemHeight);

			AbsoluteLayout.SetLayoutBounds(itemView, rect);
			itemView.Layout(rect);

			left += width;
			left += itemsSpacing;

			UpdateContentButton(itemView.Content as ContentButton);
		}
	}

	private void AnimateSelection(object oldSelected, object newSelected)
	{
		var animation = new Animation();

		var needsToBeChanged = false;

		var totalItemsWidth = (absoluteLayout.Children.Count - 1) * defaultItemWidth + selectedItemWidth;
		var left = (absoluteLayoutWidth - totalItemsWidth) / 2;

		for (var i = 0; i < absoluteLayout.Children.Count; i++)
		{
			var itemView = absoluteLayout.Children[i] as Border;

			if (itemView.BindingContext == newSelected || itemView.BindingContext == oldSelected)
			{
				needsToBeChanged = !needsToBeChanged;

				var currentLeftSelected = itemView.X;
				var newLeftSelected = left;
				var currentWidthSelected = itemView.Width;

				animation.Add(0, 1, new Animation(v =>
				{
					var l = currentLeftSelected + (newLeftSelected - currentLeftSelected) * v;
					var w = currentWidthSelected +
					        ((itemView.BindingContext == newSelected ? selectedItemWidth : defaultItemWidth) -
					         currentWidthSelected) * v;
					var rect = new Rect(l, 0, w, itemHeight);

					if (itemView.BindingContext == newSelected) UpdateContentButton(itemView.Content as ContentButton);

					AbsoluteLayout.SetLayoutBounds(itemView, rect);
					itemView.Layout(rect);
				}, finished: () =>
				{
					if (itemView.BindingContext != newSelected) UpdateContentButton(itemView.Content as ContentButton);
				}));

				left += itemView.BindingContext == newSelected ? selectedItemWidth : defaultItemWidth;
				left += itemsSpacing;
				continue;
			}

			if (!needsToBeChanged)
			{
				left += defaultItemWidth + itemsSpacing;
				continue;
			}

			var currentLeft = itemView.X;
			var newLeft = left;
			var currentWidth = itemView.Width;

			animation.Add(0, 1, new Animation(v =>
			{
				var l = currentLeft + (newLeft - currentLeft) * v;
				var w = currentWidth + (defaultItemWidth - currentWidth) * v;
				var rect = new Rect(l, 0, w, itemHeight);

				AbsoluteLayout.SetLayoutBounds(itemView, rect);
				itemView.Layout(rect);
			}));

			left += defaultItemWidth + itemsSpacing;
		}

		animation.Commit(this, "Animation", finished: (v, b) => { UpdateControl(); });
	}

	private void UpdateContentButton(ContentButton button)
	{
		var item = button.BindingContext as TabBarItem;
		var grid = button.Content as Grid;
		var icon = grid.Children[0] as Icon;
		var labelContentView = grid.Children[1] as ContentView;
		var label = labelContentView.Content as Label;

		if (item == selectedItem)
		{
			button.Background = item.SecondarySelectionColor;
			icon.TintColor = item.PrimarySelectionColor;
			label.IsVisible = true;
		}
		else
		{
			Application.Current.Resources.TryGetValue("DefaultGray", out var defaultColor);

			button.Background = Colors.Transparent;
			icon.TintColor = defaultColor as Color;
			label.IsVisible = false;
		}
	}

	private void ButtonClicked(object sender, EventArgs e)
	{
		var button = sender as ContentButton;
		var oldSelectedItem = selectedItem;

		selectedItem = button.BindingContext as TabBarItem;

		if (oldSelectedItem != selectedItem)
			AnimateSelection(oldSelectedItem, selectedItem);

		CurrentPageSelectionChanged?.Invoke(this, new ShellPageType(selectedItem.PageType));
	}

	private void AbsoluteLayoutSizeChanged(object sender, EventArgs e)
	{
		UpdateControl();
	}

	private static void ShellItemsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var tabBar = bindable as TabBarView;
		if (newValue is not IEnumerable<BaseShellItem> shellItems)
			return;

		var tabBarItems = shellItems
			.Select(s => new TabBarItem(
				s.Route,
				s.Title,
				ShellProperties.GetIconSource(s),
				ShellProperties.GetPageType(s),
				ShellProperties.GetPrimarySelectionColor(s),
				ShellProperties.GetSecondarySelectionColor(s)))
			.ToList();

		BindableLayout.SetItemsSource(tabBar.absoluteLayout, tabBarItems);

		tabBar.selectedItem = tabBarItems.FirstOrDefault();

		tabBar.UpdateControl();
	}

	public record TabBarItem(
		string Route,
		string Title,
		ImageSource IconSource,
		PageType PageType,
		Color PrimarySelectionColor,
		Color SecondarySelectionColor);
}