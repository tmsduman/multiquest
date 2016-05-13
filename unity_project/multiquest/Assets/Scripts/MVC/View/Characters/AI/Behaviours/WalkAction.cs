using System;
using System.Collections.Generic;
using TreeSharp;
using MVC.View.Characters.Data;
using UnityEngine;

namespace MVC.View.Characters.AI.Behaviours
{
	public class WalkAction : AbstractAction
	{

		bool hasRotated = false;
		
		protected override RunStatus Run(object context){
			NPCRepresentation npc = this.Unwrap (context);

			List<RepresentationPossibleDirections> directions = new List<RepresentationPossibleDirections> ();
			Array uncastedDirections = Enum.GetValues (typeof(RepresentationPossibleDirections));
			foreach (var item in uncastedDirections) {
				directions.Add ((RepresentationPossibleDirections)item);
			}


			if (this.hasRotated) {
				npc.Move (npc.previousDirection);
				this.hasRotated = false;
			} else {
				RepresentationPossibleDirections currentDir = directions [UnityEngine.Random.Range (0, directions.Count - 1)];

				this.hasRotated = npc.previousDirection != currentDir;

				npc.Move (currentDir);
			}


//			this.Unwrap (context).Move (this.lastRandomDirection);
//
//
//			RepresentationPossibleDirections currentDir = directions [UnityEngine.Random.Range (0, directions.Count - 1)];
//
//			if (this.Unwrap (context).previousDirection == currentDir) {
//				
//			}
//
//			if (currentDir != this.lastRandomDirection && this.lastRandomDirection != null) {
//			}

//			this.lastRandomDirection = currentDir;

			

			return RunStatus.Success;
		}
	}
}

