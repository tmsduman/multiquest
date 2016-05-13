using System;
using System.Collections.Generic;
using System.Linq;
using MVC.Controller.Input.Command;
using MVC.Controller.Input.Notifications;

namespace MVC.Controller.Input
{
    public class InputModule : PureMVCImplementations.UnityModule
    {
        private struct AxisData
        {
            public Action Command;
            public float TriggerValue;
        }

        private Dictionary<string, List<Action>> registeredCommands = new Dictionary<string,List<Action>>();
        private Dictionary<string, List<AxisData>> registeredAxisCommands = new Dictionary<string, List<AxisData>>();

        public InputModule(UnityUpdateHandler updateHandler)
        {
            this.RegisterCommand(RegisterInputCommandNotification.ID, new RegisterInputCommandCommand(this));
            this.RegisterCommand(RegisterAxisInputCommandNotification.ID, new RegisterAxisInputCommandCommand(this));
            this.RegisterCommand(UnregisterInputCommandNotification.ID, new UnregisterInputCommandCommand(this));
            this.RegisterCommand(UnregisterAxisInputCommandNotification.ID, new UnregisterAxisInputCommandCommand(this));

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

        public void UnregisterCommand(string inputName)
        {
            if (this.registeredCommands.ContainsKey(inputName))
            {
                this.registeredCommands.Remove(inputName);
            }
        }

        public void RegisterAxisCommand(string inputName, float triggerValue, Action command)
        {
            if (!this.registeredAxisCommands.ContainsKey(inputName))
                this.registeredAxisCommands.Add(inputName, new List<AxisData>() { new AxisData() { Command = command, TriggerValue = triggerValue } });
            else if (!this.registeredAxisCommands[inputName].Any(e => e.Command == command))
                this.registeredAxisCommands[inputName].Add(new AxisData() { Command = command, TriggerValue = triggerValue });
        }

        public void UnregisterAxisCommand(string inputName)
        {
            if (this.registeredAxisCommands.ContainsKey(inputName))
            {
                this.registeredAxisCommands.Remove(inputName);
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

            foreach (var input in this.registeredAxisCommands)
            {
                for (int i = 0; i < input.Value.Count; i++)
                {
                    if (UnityEngine.Input.GetAxis(input.Key) == input.Value[i].TriggerValue)
                    {
                        var tempEvent = input.Value[i].Command;
                        if (tempEvent != null)
                            tempEvent();
                    }
                }
            }
        }
    }
}
