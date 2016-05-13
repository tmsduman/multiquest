using TreeSharp;

namespace MVC.View.Characters.AI.Behaviours
{
	public class GoToEnemyAction : Action
	{
		public GoToEnemyAction () : base((ActionSucceedDelegate)null)
		{

		}

		protected override RunStatus Run(object context){
			UnityEngine.Debug.Log ("go to enemy");
			return RunStatus.Success;
		}
	}
}

