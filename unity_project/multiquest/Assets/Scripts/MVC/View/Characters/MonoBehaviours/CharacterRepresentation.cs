using DG.Tweening;
using MVC.View.Characters.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        private float size;

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

        private struct DirectionBlockedData
        {
            public RepresentationMovementDirections Direction;
            public Collider2D Collider;
        }

        private List<DirectionBlockedData> blockedDirections = new List<DirectionBlockedData>();
        private List<Collider2D> wasStayRemoveBefore = new List<Collider2D>();

        private void Awake()
        {
            
        }

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
                if (!this.blockedDirections.Any(e => e.Direction == direction))
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
            }

            this.StartCoroutine(this.WaitForNextAction());
        }

        private IEnumerator WaitForNextAction()
        {
            yield return new WaitForSeconds(this.movementTime);
            this.nextActionPossible = true;
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (this.wasStayRemoveBefore.Contains(collision.collider))
                this.wasStayRemoveBefore.Remove(collision.collider);
            
            if (Mathf.Abs(collision.contacts[0].point.x - collision.contacts[1].point.x) < 1
                && Mathf.Abs(collision.contacts[0].point.y - collision.contacts[1].point.y) < 1)
                return;

            if (collision.contacts[0].point.x == collision.contacts[1].point.x)
            {
                if (collision.transform.position.x > this.CachedTransform.position.x)
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationMovementDirections.Right,
                        Collider = collision.collider
                    });
                else
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationMovementDirections.Left,
                        Collider = collision.collider
                    });
            }
            else
            {
                if (collision.transform.position.y > this.CachedTransform.position.y)
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationMovementDirections.Up,
                        Collider = collision.collider
                    });
                else
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationMovementDirections.Down,
                        Collider = collision.collider
                    });
            }
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            if (Mathf.Abs(collision.contacts[0].point.x - collision.contacts[1].point.x) < 1
                && Mathf.Abs(collision.contacts[0].point.y - collision.contacts[1].point.y) < 1)
            {
                foreach (var item in this.blockedDirections.Where(e => e.Collider == collision.collider).ToArray())
	            {
		            this.blockedDirections.Remove(item);
	            }

                if(!this.wasStayRemoveBefore.Contains(collision.collider))
                    this.wasStayRemoveBefore.Add(collision.collider);

            }
            else if (this.wasStayRemoveBefore.Contains(collision.collider))
            {
                this.OnCollisionEnter2D(collision);
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (this.wasStayRemoveBefore.Contains(collision.collider))
                this.wasStayRemoveBefore.Remove(collision.collider);
            
            foreach (var item in this.blockedDirections.Where(e => e.Collider == collision.collider).ToArray())
	        {
		        this.blockedDirections.Remove(item);
	        }
        }
    }
}
