namespace Tasktrix.ViewModels
{
    public class ShellPageType : EventArgs
    {
        public PageType CurrentPage { get; private set; }

        public ShellPageType(PageType currentPage)
        {
            CurrentPage = currentPage;
        }
    }
}
