using System;
using System.Collections.Generic;
using MVC.Controller.Input.Command;
using MVC.Controller.Input.Notifications;

namespace MVC.Controller.Input
{
    public class InputModule : PureMVCImplementations.UnityModule
    {
        private Dictionary<string, List<Action>> registeredCommands = new Dictionary<string,List<Action>>();

        public InputModule(UnityUpdateHandler updateHandler)
        {
            this.RegisterCommand(RegisterInputCommandNotification.ID, new RegisterInputCommandCommand(this));
            this.RegisterCommand(UnregisterInputCommandNotification.ID, new UnregisterInputCommandCommand(this));

            updateHandler.FixedUpdateAction += delta =>
            {
                this.ExecuteCommands();
            };
        }

        public void RegisterCommand(string inputName, Action command)
        {
            if (!this.registeredCommands.ContainsKey(inputName))
                this.registeredCommands.Add(inputName, new List<Action>() { command });
            else if (!this.registeredCommands[inputName].Contains(command))
                this.registeredCommands[inputName].Add(command);
        }

        public void UnregisterCommand(string inputName, Action command)
        {
            if (this.registeredCommands.ContainsKey(inputName))
            {
                if (this.registeredCommands[inputName].Contains(command))
                    this.registeredCommands[inputName].Remove(command);
            }
        }

        private void ExecuteCommands()
        {
            foreach (var input in this.registeredCommands)
            {
                if (UnityEngine.Input.GetButton(input.Key))
                {
                    for (int i = 0; i < input.Value.Count; i++)
                    {
                        var tempEvent = input.Value[i];
                        if (tempEvent != null)
                            tempEvent();
                    }
                }
            }
        }
    }
}
