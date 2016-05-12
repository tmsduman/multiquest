namespace MVC.Controller.Input.Notifications
{
    public class RegisterInputCommandNotification : PureMVCImplementations.UnityNotification<RegisterInputCommandNotification>
    {
        public string InputName;
        public System.Action Command;
    }
}
