namespace MVC.Core.PureMVCImplementations
{
    public class UnityProxy<T> : PureMVC.Interfaces.IProxy
    {
        public string ProxyName
        {
            get { return typeof(T).Name; }
        }

        private object localData;
        public object Data
        {
            get
            {
                return this.localData;
            }
            set
            {
                this.localData = value;
            }
        }

        public virtual void OnRegister()
        {
        }

        public virtual void OnRemove()
        {
        }
    }
}
