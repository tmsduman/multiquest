using MVC.Controller.Input.Notifications;

namespace MVC.Controller.Input.Command
{
    public class UnregisterAxisInputCommandCommand : PureMVC.Patterns.SimpleCommand
    {
        private readonly InputModule module;

        public UnregisterAxisInputCommandCommand(InputModule module)
        {
            this.module = module;
        }

        public override void Execute(PureMVC.Interfaces.INotification notification)
        {
            UnregisterAxisInputCommandNotification n = (UnregisterAxisInputCommandNotification)notification;
            this.module.UnregisterAxisCommand(n.InputName);
        }
    }
}
