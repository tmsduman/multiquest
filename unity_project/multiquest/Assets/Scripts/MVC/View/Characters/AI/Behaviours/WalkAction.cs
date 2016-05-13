﻿using TreeSharp;

namespace MVC.View.Characters.AI.Behaviours
{
	public class WalkAction : Action
	{
		public WalkAction () : base((ActionSucceedDelegate)null)
		{
			
		}

		protected override RunStatus Run(object context){
			UnityEngine.Debug.Log ("walk around");
			return RunStatus.Success;
		}
	}
}

