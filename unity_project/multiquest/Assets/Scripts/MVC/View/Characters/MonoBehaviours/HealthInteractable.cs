using MVC.View.Bases;
using UnityEngine;

namespace MVC.View.Characters.MonoBehaviours
{
    public class HealthInteractable : MonoBehaviour, IInteractableRepresentation
    {
        [SerializeField]
        private float startHealth;

        private float actualHealth;

        private void Awake()
        {
            this.actualHealth = this.startHealth;
        }

        public void Interact(GameObject interactGameObject)
        {
            Debug.Log("interact");
            WeaponRepresentation weapon = interactGameObject.GetComponent<WeaponRepresentation>();
            if(weapon != null)
            {
                this.actualHealth -= weapon.damagePerHit;
                if (this.actualHealth <= 0)
                    this.gameObject.SetActive(false);
            }
        }
    }
}
