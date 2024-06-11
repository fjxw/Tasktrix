using Tasktrix.Views;
using Microsoft.Maui.Controls;

namespace Tasktrix;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}