using PureMVC.Interfaces;

namespace PureMVCImplementations
{
    public class UnityModule
    {
        public UnityFacade Facade
        {
            get { return UnityFacade.Instance; }
        }

        public void SendNotification(INotification notification)
        {
            this.Facade.SendNotification(notification);
        }

        public bool HasCommand(string notificationName)
        {
            return this.Facade.HasCommand(notificationName);
        }

        public void RegisterCommand(string notificationName, ICommand command)
        {
            this.Facade.RegisterCommand(notificationName, command);
        }

        public void RemoveCommand(string notificationName)
        {
            this.Facade.RemoveCommand(notificationName);
        }
    }
}
