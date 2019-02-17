namespace MediaPlayer.Generic.Window_Service.Interfaces
{
    interface IWindowService
    {
        void ShowWindow(object dataContext = null);
        void ShowWindowModal(object dataContext = null);
    }
}
