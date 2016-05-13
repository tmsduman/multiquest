using TreeSharp;
using MVC.Model.Character;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;
namespace MVC.View.Characters.AI.Behaviours
{
	public class AttackAction : AbstractAction
	{
		

		protected override RunStatus Run(object context){
			NPCRepresentation npcRepresentation = this.Unwrap (context);
			CharacterProxy proxy = npcRepresentation.GetFacade ().GetProxy<CharacterProxy> ();
			foreach (var player in proxy.Players) {
				foreach (var blockedItem in npcRepresentation.blockedDirections) {
					if (blockedItem.Collider.GetComponent<CharacterRepresentation> () == player) {
						npcRepresentation.Attack (blockedItem.Direction);
						return RunStatus.Success;
					}
				}	
			}

			return RunStatus.Failure;
		}
	}
}

