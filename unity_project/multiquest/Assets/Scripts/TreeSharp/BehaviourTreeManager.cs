using System.Collections.Generic;
using UnityEngine;

namespace TreeSharp
{
    public class BehaviourTreeManager : MonoBehaviour
    {
        [SerializeField]
        private float updateInterval = 0.5f;

        private Dictionary<Composite, object> registeredBehaviours = new Dictionary<Composite, object>();
        private float currentDeltaTime;

        private void Update()
        {
            this.currentDeltaTime += Time.deltaTime;
            if (this.currentDeltaTime > this.updateInterval)
            {
                this.currentDeltaTime = 0;
                this.TickRegisteredBehaviours();
            }
        }

        private void TickRegisteredBehaviours()
        {
            foreach (var behaviour in this.registeredBehaviours)
            {
                this.TickBehaviour(behaviour.Key, behaviour.Value);
            }
        }

        private void TickBehaviour(Composite behaviour, object context)
        {
            behaviour.Tick(context);

            // If we're still running the current composite, then we don't need to reset, we want the tree to pick up
            // where it left off last tick.
            if (behaviour.LastStatus != RunStatus.Running)
            {
                // Reset the tree, and begin the execution all over...
                behaviour.Stop(context);
                behaviour.Start(context);
            }
        }

        public void RegisterBehaviour(Composite behaviour, object context, bool tick = false)
        {
            if (!this.registeredBehaviours.ContainsKey(behaviour))
            {
                behaviour.Start(context);
                this.registeredBehaviours.Add(behaviour, context);
                if (tick)
                {
                    this.TickBehaviour(behaviour, context);
                }
            }
        }

        public void UnregisterBehaviour(Composite behaviour, bool tick = false)
        {
            if (this.registeredBehaviours.ContainsKey(behaviour))
            {
                if (tick)
                {
                    this.TickBehaviour(behaviour, this.registeredBehaviours[behaviour]);
                }

                this.registeredBehaviours.Remove(behaviour);
            }
        }

        public void UpdateBehaviourContext(Composite behaviour, object context, bool tick = false)
        {
            if (this.registeredBehaviours.ContainsKey(behaviour))
            {
                behaviour.Stop(this.registeredBehaviours[behaviour]);
                this.registeredBehaviours[behaviour] = context;
                behaviour.Start(context);
                
                if (tick)
                {
                    this.TickBehaviour(behaviour, context);
                }
            }
        }
    }
}
