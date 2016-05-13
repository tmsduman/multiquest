using System;
using UnityEngine;

namespace MVC.View.Characters.MonoBehaviours
{
    public class PlayerRepresentation : CharacterRepresentation
    {
        public event Action Moved;

        private UnityEngine.Camera gameCamera;

        public void SetCamera(UnityEngine.Camera gameCamera)
        {
            this.gameCamera = gameCamera;
        }

        public override void Move(Data.RepresentationPossibleDirections direction)
        {
            Vector3 viewportPosition = Vector3.zero;
            switch (direction)
            {
                case MVC.View.Characters.Data.RepresentationPossibleDirections.Left:
                    viewportPosition = this.gameCamera.WorldToViewportPoint(
                        this.CachedTransform.position + new Vector3(-this.movementLength, 0, 0));
                    break;
                case MVC.View.Characters.Data.RepresentationPossibleDirections.Right:
                    viewportPosition = this.gameCamera.WorldToViewportPoint(
                        this.CachedTransform.position + new Vector3(this.movementLength, 0, 0));
                    break;
                case MVC.View.Characters.Data.RepresentationPossibleDirections.Up:
                    viewportPosition = this.gameCamera.WorldToViewportPoint(
                        this.CachedTransform.position + new Vector3(0, this.movementLength, 0));
                    break;
                case MVC.View.Characters.Data.RepresentationPossibleDirections.Down:
                    viewportPosition = this.gameCamera.WorldToViewportPoint(
                        this.CachedTransform.position + new Vector3(0, -this.movementLength, 0));
                    break;
            }

            if (viewportPosition.x <= 0 || viewportPosition.x >= 1 || viewportPosition.y <= 0 || viewportPosition.y >= 1)
                return;

            base.Move(direction);
        }

        protected override void OnMoved(Data.RepresentationPossibleDirections direction)
        {
            var tempEvent = this.Moved;
            if (tempEvent != null)
                tempEvent();
        }
    }
}
