using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace MVC.Core.PureMVCImplementations
{
    public class UnityController : IController
    {
        private readonly IView view;
        private readonly object syncRoot = new object();
        private Dictionary<string, List<ICommand>> mappedCommands = new Dictionary<string,List<ICommand>>();

        public UnityController()
        {
            view = PureMVC.Core.View.Instance;
        }

        public void RegisterCommand(string notificationName, ICommand command)
        {
            lock (this.syncRoot)
            {
                if (!this.mappedCommands.ContainsKey(notificationName))
                {
                    // This call needs to be monitored carefully. Have to make sure that RegisterObserver
                    // doesn't call back into the controller, or a dead lock could happen.
                    this.view.RegisterObserver(notificationName, new Observer("executeCommand", this));
                    this.mappedCommands[notificationName] = new List<ICommand>() { command };
                }
                else
                    this.mappedCommands[notificationName].Add(command);
            }
        }

        public void ExecuteCommand(PureMVC.Interfaces.INotification notification)
        {
            lock (this.syncRoot)
            {
                if (!this.mappedCommands.ContainsKey(notification.Name))
                    return;
                for (int i = 0; i < this.mappedCommands[notification.Name].Count; i++)
                {
                    this.mappedCommands[notification.Name][i].Execute(notification);
                }
            }
        }

        public void RemoveCommand(string notificationName)
        {
            lock (this.syncRoot)
            {
                if (this.mappedCommands.ContainsKey(notificationName))
                {
                    // remove the observer

                    // This call needs to be monitored carefully. Have to make sure that RemoveObserver
                    // doesn't call back into the controller, or a dead lock could happen.
                    this.view.RemoveObserver(notificationName, this);
                    this.mappedCommands.Remove(notificationName);
                }
            }
        }

        public bool HasCommand(string notificationName)
        {
            lock (this.syncRoot)
            {
                return this.mappedCommands.ContainsKey(notificationName);
            }
        }

        public void RegisterCommand(string notificationName, System.Type commandType)
        {
            object commandInstance = System.Activator.CreateInstance(commandType);

			if (commandInstance is ICommand)
			{
                this.RegisterCommand(notificationName, commandInstance as ICommand);
			}
            
        }
    }
}
