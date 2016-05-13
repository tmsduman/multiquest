using System;

namespace MVC.View.Characters.MonoBehaviours
{
    public class PlayerRepresentation : CharacterRepresentation
    {
        public event Action Moved;

        protected override void OnMoved(Data.RepresentationPossibleDirections direction)
        {
            var tempEvent = this.Moved;
            if (tempEvent != null)
                tempEvent();
        }
    }
}
