using MVC.Controller.Input.Notifications;

namespace MVC.Controller.Input.Command
{
    public class RegisterInputCommandCommand : PureMVC.Patterns.SimpleCommand
    {
        private readonly InputModule module;

        public RegisterInputCommandCommand(InputModule module)
        {
            this.module = module;
        }

        public override void Execute(PureMVC.Interfaces.INotification notification)
        {
            RegisterInputCommandNotification n = (RegisterInputCommandNotification)notification;
            this.module.RegisterCommand(n.InputName, n.Command);
        }
    }
}
