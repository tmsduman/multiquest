namespace MVC.Controller.Input.Notifications
{
    public class RegisterAxisInputCommandNotification : PureMVCImplementations.UnityNotification<RegisterAxisInputCommandNotification>
    {
        public string InputName;
        public System.Action Command;
        public float TriggerValue;
    }
}
