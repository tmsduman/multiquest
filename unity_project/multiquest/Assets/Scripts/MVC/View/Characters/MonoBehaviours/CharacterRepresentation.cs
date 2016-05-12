using MVC.View.Characters.Data;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace MVC.View.Characters.MonoBehaviours
{
    public class CharacterRepresentation : MonoBehaviour
    {
        #region CachedTransform
        private Transform localCachedTransform;
        internal Transform CachedTransform
        {
            get
            {
                if (this.localCachedTransform == null)
                    this.localCachedTransform = this.transform;
                return this.localCachedTransform;
            }
        }
        #endregion

        [SerializeField]
        private float movementLength;

        [SerializeField]
        private float movementTime;

        [SerializeField]
        private Image characterImage;

        [SerializeField]
        private Animator moveAnimator;

        private Data.RepresentationMovementDirections previousDirection;

        private bool nextActionPossible = true;

        public void Move(RepresentationMovementDirections direction)
        {
            if (!this.nextActionPossible)
                return;

            this.nextActionPossible = false;

            if (direction != this.previousDirection)
            {
                this.previousDirection = direction;

                switch (direction)
                {
                    case RepresentationMovementDirections.Left:
                        this.moveAnimator.SetTrigger("IdleLeft");
                        break;
                    case RepresentationMovementDirections.Right:
                        this.moveAnimator.SetTrigger("IdleRight");
                        break;
                    case RepresentationMovementDirections.Up:
                        this.moveAnimator.SetTrigger("IdleUp");
                        break;
                    case RepresentationMovementDirections.Down:
                        this.moveAnimator.SetTrigger("IdleDown");
                        break;
                }
            }
            else
            {
                switch (direction)
                {
                    case RepresentationMovementDirections.Left:
                        this.CachedTransform.DOLocalMoveX(this.CachedTransform.localPosition.x - this.movementLength, this.movementTime);
                        this.moveAnimator.SetTrigger("MoveLeft");
                        break;
                    case RepresentationMovementDirections.Right:
                        this.CachedTransform.DOLocalMoveX(this.CachedTransform.localPosition.x + this.movementLength, this.movementTime);
                        this.moveAnimator.SetTrigger("MoveRight");
                        break;
                    case RepresentationMovementDirections.Up:
                        this.CachedTransform.DOLocalMoveY(this.CachedTransform.localPosition.y + this.movementLength, this.movementTime);
                        this.moveAnimator.SetTrigger("MoveUp");
                        break;
                    case RepresentationMovementDirections.Down:
                        this.CachedTransform.DOLocalMoveY(this.CachedTransform.localPosition.y - this.movementLength, this.movementTime);
                        this.moveAnimator.SetTrigger("MoveDown");
                        break;
                }
            }

            this.StartCoroutine(this.WaitForNextAction());
        }

        private IEnumerator WaitForNextAction()
        {
            yield return new WaitForSeconds(this.movementTime);
            this.nextActionPossible = true;
        }
    }
}
