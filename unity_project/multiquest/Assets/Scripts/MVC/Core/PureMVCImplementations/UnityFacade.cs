using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace PureMVCImplementations
{
    public class UnityFacade : Facade
    {
        public new static UnityFacade Instance
        {
            get
            {
                if (cachedInstance == null)
                {
                    lock (m_staticSyncRoot)
                    {
                        if (cachedInstance == null)
                        {
                            cachedInstance = new UnityFacade();
                        }
                    }
                }

                return cachedInstance;
            }
        }

        private static UnityFacade cachedInstance;

        protected UnityFacade() 
        {
            this.InitializeFacade();
        }

        protected override void InitializeController()
        {
            if (m_controller != null)
                return;
            m_controller = new MVC.Core.PureMVCImplementations.UnityController();
        }

        /// <summary>
        /// Send an <c>INotification</c>
        /// </summary>
        /// <param name="notification">The notification to send</param>
        /// <remarks>Keeps us from having to construct new notification instances in our implementation code</remarks>
        /// <remarks>This method is thread safe and needs to be thread safe in all implementations.</remarks>
        public virtual void SendNotification(INotification notification)
        {
            this.NotifyObservers(notification);
        }

        public T GetProxy<T>() where T : IProxy
        {
            string proxyName = typeof(T).Name;
            if (this.HasProxy(proxyName))
                return (T)this.RetrieveProxy(proxyName);
            return default(T);
        }

        public bool HasProxy<T>() where T : IProxy
        {
            return this.HasProxy(typeof(T).Name);
        }

        public void RegisterCommand(string notificationName, ICommand command)
        {
            ((MVC.Core.PureMVCImplementations.UnityController) m_controller).RegisterCommand(notificationName, command);
        }
    }
}
