using MVC.Controller.Input.Notifications;

namespace MVC.Controller.Input.Command
{
    public class UnregisterInputCommandCommand : PureMVC.Patterns.SimpleCommand
    {
        private readonly InputModule module;

        public UnregisterInputCommandCommand(InputModule module)
        {
            this.module = module;
        }

        public override void Execute(PureMVC.Interfaces.INotification notification)
        {
            UnregisterInputCommandNotification n = (UnregisterInputCommandNotification)notification;
            this.module.UnregisterCommand(n.InputName);
        }
    }
}
