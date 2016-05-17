using MVC.View.Bases;
using PureMVCImplementations;
using UnityEngine;

namespace MVC.View.Map.MonoBehaviours
{
    public class MapExitRepresentation : MonoBehaviour, IInteractableRepresentation
    {
        private UnityFacade facade;

        public void SetFacade(UnityFacade facade)
        {
            this.facade = facade;
        }

        public void Interact(GameObject gameObject)
        {
            if(gameObject.GetComponent<MVC.View.Characters.MonoBehaviours.PlayerRepresentation>() != null)
            {
                if (this.facade != null)
                    this.facade.SendNotification(new MVC.View.Map.Notifications.LoadMapNotification());
            }
        }
    }
}
