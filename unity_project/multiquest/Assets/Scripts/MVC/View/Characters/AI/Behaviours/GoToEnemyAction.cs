using TreeSharp;
using MVC.Model.Character;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;
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


			return RunStatus.Success;
		}
	}
}

