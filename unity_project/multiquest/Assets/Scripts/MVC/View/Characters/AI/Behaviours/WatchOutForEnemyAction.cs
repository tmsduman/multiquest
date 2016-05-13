using System;
using System.Collections.Generic;
using TreeSharp;
using MVC.View.Characters.Data;
using MVC.Model.Character;
using UnityEngine;

namespace MVC.View.Characters.AI.Behaviours
{
	public class WatchOutForEnemyAction : AbstractAction
	{

		protected override RunStatus Run(object context){
			NPCRepresentation npcRepresentation = this.Unwrap (context);
			CharacterProxy proxy = npcRepresentation.GetFacade ().GetProxy<CharacterProxy> ();

			foreach (var item in proxy.Players) {
				if (Vector3.Distance (item.transform.position, npcRepresentation.transform.position) <= npcRepresentation.ViewRange) {
					return RunStatus.Success;
				}
			}
			//Debug.Log ("watch out for enemy");
			return RunStatus.Failure;
		}
	}
}

