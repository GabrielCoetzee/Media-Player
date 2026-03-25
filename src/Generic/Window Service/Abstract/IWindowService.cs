namespace Generic.Abstract
{
    interface IWindowService
    {
        void ShowWindow(object dataContext = null);
        void ShowWindowModal(object dataContext = null);
    }
}
