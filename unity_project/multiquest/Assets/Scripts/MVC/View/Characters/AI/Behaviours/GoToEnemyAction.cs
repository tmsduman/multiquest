using TreeSharp;
using MVC.Model.Character;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;
using MVC.View.Characters.Data;

namespace MVC.View.Characters.AI.Behaviours
{
	public class GoToEnemyAction : AbstractAction
	{
		

		protected override RunStatus Run(object context){
			NPCRepresentation npcRepresentation = this.Unwrap (context);
			CharacterProxy proxy = npcRepresentation.GetFacade ().GetProxy<CharacterProxy> ();
			float closestDistance = float.MaxValue;
			CharacterRepresentation closestPlayer = null;
			foreach (var item in proxy.Players) {
				float currentDistance = Vector3.Distance (item.transform.position, npcRepresentation.transform.position);
				if (currentDistance < closestDistance) {
					closestDistance = currentDistance;
					closestPlayer = item;
				}
			}
			if (closestPlayer == null) {
				return RunStatus.Failure;
			}

			RepresentationPossibleDirections dirToMove = this.GetDirection (npcRepresentation.transform.position, closestPlayer.transform.position);

			if (npcRepresentation.IsBlockedDirection (dirToMove)) {

				if (!npcRepresentation.IsBlockedDirection (RepresentationPossibleDirections.Down)) {
					dirToMove = RepresentationPossibleDirections.Down;
				} else if (!npcRepresentation.IsBlockedDirection (RepresentationPossibleDirections.Left)) {
					dirToMove = RepresentationPossibleDirections.Left;
				} else if (!npcRepresentation.IsBlockedDirection (RepresentationPossibleDirections.Right)) {
					dirToMove = RepresentationPossibleDirections.Right;
				} else {
					dirToMove = RepresentationPossibleDirections.Down;
				} 
			}

			npcRepresentation.Move (dirToMove);


			return RunStatus.Success;
		}

		public RepresentationPossibleDirections GetDirection (Vector3 pointOfOrigin, Vector3 vectorToTest) {

			RepresentationPossibleDirections result = RepresentationPossibleDirections.Left;
			float shortestDistance = Mathf.Infinity;
			float distance = 0;

//			Vector3 vectorPosition = pointOfOrigin + vectorToTest;

			distance = Mathf.Abs (((pointOfOrigin + Vector3.up) - vectorToTest).magnitude);
			if (distance < shortestDistance)
			{
				shortestDistance = distance;
				result = RepresentationPossibleDirections.Up;
			}
			distance = Mathf.Abs (((pointOfOrigin + -Vector3.up) - vectorToTest).magnitude);
			if (distance < shortestDistance)
			{
				shortestDistance = distance;
				result = RepresentationPossibleDirections.Down;
			}
			distance = Mathf.Abs (((pointOfOrigin + Vector3.left) - vectorToTest).magnitude);
			if (distance < shortestDistance)
			{
				shortestDistance = distance;
				result = RepresentationPossibleDirections.Left;
			}
			distance = Mathf.Abs (((pointOfOrigin + Vector3.right) - vectorToTest).magnitude);
			if (distance < shortestDistance)
			{
				shortestDistance = distance;
				result = RepresentationPossibleDirections.Right;
			}

			return result;

		}
	}
}

