using System.Collections;
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
            WeaponRepresentation weapon = interactGameObject.GetComponent<WeaponRepresentation>();
            if (weapon != null)
            {
                this.actualHealth -= weapon.damagePerHit;
                if (this.actualHealth <= 0)
                {
                    this.transform.position += Vector3.one * 100000;
                    this.StartCoroutine(this.WaitToHide());
                }
            }
        }

        public void Kill()
        {
            this.transform.position += Vector3.one * 100000;
            this.StartCoroutine(this.WaitToHide());
        }

        private IEnumerator WaitToHide()
        {
            yield return new WaitForSeconds(1);
            this.gameObject.SetActive(false);
        }
    }
}
