using MVC.View.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MVC.View.Characters.MonoBehaviours
{
    public class WeaponRepresentation : MonoBehaviour
    {
        // TODO: attack

        public float damagePerHit;

        public void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("enter");
            IInteractableRepresentation interactable = collision.gameObject.GetComponent<IInteractableRepresentation>();
            if (interactable != null)
                interactable.Interact(this.gameObject);
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            //Debug.Log("exit");
        }
    }
}
