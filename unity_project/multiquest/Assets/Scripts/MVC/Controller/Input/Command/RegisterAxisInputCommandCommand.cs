using MVC.Controller.Input.Notifications;

namespace MVC.Controller.Input.Command
{
    public class RegisterAxisInputCommandCommand : PureMVC.Patterns.SimpleCommand
    {
        private readonly InputModule module;

        public RegisterAxisInputCommandCommand(InputModule module)
        {
            this.module = module;
        }

        public override void Execute(PureMVC.Interfaces.INotification notification)
        {
            RegisterAxisInputCommandNotification n = (RegisterAxisInputCommandNotification)notification;
            this.module.RegisterAxisCommand(n.InputName, n.TriggerValue, n.Command);
        }
    }
}
