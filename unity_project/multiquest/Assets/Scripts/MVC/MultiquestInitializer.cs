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

        public override void Update()
        {
            base.Update();
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }

        private void InitializeProxies()
        {
            this.Facade.RegisterProxy(new MVC.Model.Character.CharacterProxy());
        }

        private void InitializeModules()
        {
            new MVC.Controller.Input.InputModule(this);
        }

        private void InitializeMediators()
        {
            
        }

        private void OnApplicationQuit()
        {
            
        }
    }
}
