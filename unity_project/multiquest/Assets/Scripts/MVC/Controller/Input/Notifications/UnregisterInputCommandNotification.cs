namespace MVC.Controller.Input.Notifications
{
    public class UnregisterInputCommandNotification : PureMVCImplementations.UnityNotification<UnregisterInputCommandNotification>
    {
        public string InputName;
        public System.Action Command;
    }
}
