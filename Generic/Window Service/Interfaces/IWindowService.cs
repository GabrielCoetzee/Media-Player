namespace Generic.WindowInterfaces
{
    interface IWindowService
    {
        void ShowWindow(object dataContext = null);
        void ShowWindowModal(object dataContext = null);
    }
}
