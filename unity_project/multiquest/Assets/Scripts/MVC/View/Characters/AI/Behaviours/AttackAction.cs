using TreeSharp;

namespace MVC.View.Characters.AI.Behaviours
{
	public class AttackAction : AbstractAction
	{
		

		protected override RunStatus Run(object context){
			UnityEngine.Debug.Log ("attack");
			return RunStatus.Failure;
		}
	}
}

