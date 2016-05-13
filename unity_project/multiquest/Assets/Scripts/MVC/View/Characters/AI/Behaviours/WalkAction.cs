using System;
using System.Collections.Generic;
using TreeSharp;
using MVC.View.Characters.Data;

namespace MVC.View.Characters.AI.Behaviours
{
	public class WalkAction : AbstractAction
	{
		
		protected override RunStatus Run(object context){
			List<RepresentationPossibleDirections> directions = new List<RepresentationPossibleDirections> ();
			Array uncastedDirections = Enum.GetValues (typeof(RepresentationPossibleDirections));
			foreach (var item in uncastedDirections) {
				directions.Add ((RepresentationPossibleDirections)item);
			}

			this.Unwrap (context).Move (directions[0]);
			return RunStatus.Success;
		}
	}
}

