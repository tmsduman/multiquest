using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

namespace PureMVCImplementations
{
    public abstract class UnityMediator<T> : IMediator
    {
        private readonly Dictionary<Type, Delegate> notificationMap = 
            new Dictionary<Type, Delegate>();
        
        private object viewComponent;

        public UnityMediator()
        {
            this.BeforeRegister();
        }

        public UnityFacade Facade
        {
            get { return UnityFacade.Instance; }
        }

        public string MediatorName
        {
            get { return typeof(T).Name; }
        }

        public object ViewComponent
        {
            get
            {
                return this.viewComponent;
            }
            set
            {
                this.viewComponent = value;
            }
        }

        /// <summary>
        /// List the <c>INotification</c> names this <c>Mediator</c> is interested in being notified of
        /// </summary>
        /// <returns>The list of <c>INotification</c> names </returns>
        public virtual IList<string> ListNotificationInterests()
        {
            List<string> list = new List<string>();
            foreach (var item in this.notificationMap)
            {
                list.Add(item.Key.Name);
            }
            return list;
        }

        /// <summary>
        /// Handle <c>INotification</c>s
        /// </summary>
        /// <param name="notification">The <c>INotification</c> instance to handle</param>
        /// <remarks>
        ///     <para>
        ///        Typically this will be handled in a switch statement, with one 'case' entry per <c>INotification</c> the <c>Mediator</c> is interested in. 
        ///     </para>
        /// </remarks>
        public virtual void HandleNotification(INotification notification)
        {
            foreach (var mapping in this.notificationMap)
            {
                if (notification.GetType().Equals(mapping.Key))
                {
                    var tempEvent = mapping.Value;
                    if (tempEvent != null)
                        tempEvent.DynamicInvoke(notification);
                    break;
                }
            }
        }

        /// <summary>
        /// Called by the View when the Mediator is registered
        /// </summary>
        public virtual void OnRegister()
        {
        }

        /// <summary>
        /// Called by the View when the Mediator is removed
        /// </summary>
        public virtual void OnRemove()
        {
        }

        public void SendNotification(INotification notification)
        {
            this.Facade.SendNotification(notification);
        }

        public virtual void BeforeRegister()
        {
        }

        public void RegisterNotification<N>(Action<N> executeMethod) where N : INotification
        {
            Type notificationType = typeof(N);
            if (this.notificationMap.ContainsKey(notificationType))
            {
                return;
            }

            this.notificationMap.Add(notificationType, executeMethod);
        }
    }
}
