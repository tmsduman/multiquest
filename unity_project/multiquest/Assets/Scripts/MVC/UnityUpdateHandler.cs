namespace MVC
{
    public abstract class UnityUpdateHandler : UnityEngine.MonoBehaviour
    {
        public event System.Action<float> UpdateAction;
        public event System.Action<float> FixedUpdateAction;

        public virtual void Update()
        {
            var tempEvent = this.UpdateAction;
            if (tempEvent != null)
                tempEvent(UnityEngine.Time.smoothDeltaTime);
        }

        public virtual void FixedUpdate()
        {
            var tempEvent = this.FixedUpdateAction;
            if (tempEvent != null)
                tempEvent(UnityEngine.Time.fixedDeltaTime);
        }
    }
}
