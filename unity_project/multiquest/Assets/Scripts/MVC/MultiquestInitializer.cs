using PureMVCImplementations;
using UnityEngine;

namespace MVC
{
    public class MultiquestInitializer : UnityUpdateHandler
    {
        private static bool initialized;

        public UnityFacade Facade
        {
            get { return UnityFacade.Instance; }
        }

        private void Awake()
        {
            Application.targetFrameRate = 30;


            if (initialized)
                return;
            initialized = true;

            this.InitializeProxies();
            this.InitializeModules();
            this.InitializeMediators();
        }

        private void InitializeProxies()
        {
            
        }

        private void InitializeModules()
        {
            
        }

        private void InitializeMediators()
        {
            
        }

        private void OnApplicationQuit()
        {
            
        }
    }
}
