using SimpleToolkit.Core;
using Tasktrix.ViewModels;

namespace Tasktrix;

public partial class AppShell
{
	public AppShell()
	{
		InitializeComponent();
		Loaded += AppShellLoaded;
	}

	private static void AppShellLoaded(object sender, EventArgs e)
	{
		var shell = sender as AppShell;
		if (shell != null)
			shell.Window.SubscribeToSafeAreaChanges(safeArea =>
			{
				shell.RootContainer.Padding = new Thickness(safeArea.Left, 0, safeArea.Right, safeArea.Bottom);
			});
	}

	private void TabBarViewCurrentPageChanged(object sender, ShellPageType e)
	{
		Microsoft.Maui.Controls.Shell.Current.GoToAsync("///" + e.CurrentPage);
	}
}

public enum PageType
{
	TaskPageView = 0,
	AnalysisPageView = 1
}