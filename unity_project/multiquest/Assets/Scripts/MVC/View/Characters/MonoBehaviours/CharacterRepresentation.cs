﻿using DG.Tweening;
using MVC.View.Characters.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

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

        public event Action<CharacterRepresentation> Killed;

        [SerializeField]
        private float size;

        [SerializeField]
        protected float movementLength;

        [SerializeField]
        private float movementTime;

        [SerializeField]
        private Image characterImage;

        [SerializeField]
        private Animator moveAnimator;

        [SerializeField]
        private float attackTime;

        public Data.RepresentationPossibleDirections previousDirection;

        private bool nextActionPossible = true;

        public int InputId;

        public struct DirectionBlockedData
        {
            public RepresentationPossibleDirections Direction;
            public Collider2D Collider;
        }

        public List<DirectionBlockedData> blockedDirections = new List<DirectionBlockedData>();
        private List<Collider2D> wasStayRemoveBefore = new List<Collider2D>();

        protected virtual void OnDisable()
        {
            var tempEvent = this.Killed;
            if (tempEvent != null)
                tempEvent(this);
        }

        #region move

        public virtual void Move(RepresentationPossibleDirections direction)
        {
            if (!this.nextActionPossible)
                return;
            this.nextActionPossible = false;

            if (direction != this.previousDirection)
            {
                this.previousDirection = direction;
                this.moveAnimator.SetTrigger("Idle" + direction);
            }
            else
            {
                if (!this.blockedDirections.Any(e => e.Direction == direction))
                {
                    this.moveAnimator.SetTrigger("Move" + direction);
                    switch (direction)
                    {
                        case RepresentationPossibleDirections.Left:
                            this.CachedTransform.DOLocalMoveX(this.CachedTransform.localPosition.x - this.movementLength, this.movementTime);
                            break;
                        case RepresentationPossibleDirections.Right:
                            this.CachedTransform.DOLocalMoveX(this.CachedTransform.localPosition.x + this.movementLength, this.movementTime);
                            break;
                        case RepresentationPossibleDirections.Up:
                            this.CachedTransform.DOLocalMoveY(this.CachedTransform.localPosition.y + this.movementLength, this.movementTime);
                            break;
                        case RepresentationPossibleDirections.Down:
                            this.CachedTransform.DOLocalMoveY(this.CachedTransform.localPosition.y - this.movementLength, this.movementTime);
                            break;
                    }

                    this.OnMoved(direction);
                }
            }

            this.StartCoroutine(this.WaitForNextAction(this.movementTime));
        }

        protected virtual void OnMoved(RepresentationPossibleDirections direction)
        {
        }

        private IEnumerator WaitForNextAction(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            this.nextActionPossible = true;
        }

        #endregion

        #region collision

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.gameObject.GetComponent<WeaponRepresentation>() != null)
                return;

            if (this.wasStayRemoveBefore.Contains(collision.collider))
                this.wasStayRemoveBefore.Remove(collision.collider);

            if (Mathf.Abs(collision.contacts[0].point.x - collision.contacts[1].point.x) < 0.1f
                && Mathf.Abs(collision.contacts[0].point.y - collision.contacts[1].point.y) < 0.1f)
            {
                if (!this.wasStayRemoveBefore.Contains(collision.collider))
                    this.wasStayRemoveBefore.Add(collision.collider);
                return;
            }

            if (this.blockedDirections.Any(e => e.Collider == collision.collider))
                return;

            if (collision.contacts[0].point.x == collision.contacts[1].point.x)
            {
                if (collision.transform.position.x > this.CachedTransform.position.x)
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationPossibleDirections.Right,
                        Collider = collision.collider
                    });
                else
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationPossibleDirections.Left,
                        Collider = collision.collider
                    });
            }
            else
            {
                if (collision.transform.position.y > this.CachedTransform.position.y)
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationPossibleDirections.Up,
                        Collider = collision.collider
                    });
                else
                    this.blockedDirections.Add(new DirectionBlockedData()
                    {
                        Direction = RepresentationPossibleDirections.Down,
                        Collider = collision.collider
                    });
            }

            //Debug.Log("add " + collision.collider.gameObject.name + " " + this.gameObject.name);
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<WeaponRepresentation>() != null)
                return;

            if (Mathf.Abs(collision.contacts[0].point.x - collision.contacts[1].point.x) < 0.1f
                && Mathf.Abs(collision.contacts[0].point.y - collision.contacts[1].point.y) < 0.1f)
            {
                foreach (var item in this.blockedDirections.Where(e => e.Collider == collision.collider).ToArray())
                {
                    //Debug.Log("remove stay " + collision.collider.gameObject.name + " " + this.gameObject.name);
                    this.blockedDirections.Remove(item);
                }

                if (!this.wasStayRemoveBefore.Contains(collision.collider))
                    this.wasStayRemoveBefore.Add(collision.collider);
            }
            else if (this.wasStayRemoveBefore.Contains(collision.collider))
            {
                this.OnCollisionEnter2D(collision);
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.gameObject.GetComponent<WeaponRepresentation>() != null)
                return;

            if (this.wasStayRemoveBefore.Contains(collision.collider))
                this.wasStayRemoveBefore.Remove(collision.collider);
            
            foreach (var item in this.blockedDirections.Where(e => e.Collider == collision.collider).ToArray())
            {
                //Debug.Log("remove exit " + collision.collider.gameObject.name + " " + this.gameObject.name);
                this.blockedDirections.Remove(item);
            }
        }

        #endregion

        public void Attack()
        {
            this.Attack(this.previousDirection);
        }

        public virtual void Attack(RepresentationPossibleDirections direction)
        {
            if (!this.nextActionPossible)
                return;
            this.nextActionPossible = false;

            this.moveAnimator.SetTrigger("Attack" + direction);

            this.StartCoroutine(this.WaitForNextAction(this.attackTime));
        }

        public bool IsBlockedDirection (RepresentationPossibleDirections dir) {
            foreach (DirectionBlockedData item in this.blockedDirections) {
                if (dir == item.Direction) {
                    return true;
                }
            }

            return false;
        }
    }
}
