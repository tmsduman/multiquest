using TreeSharp;

namespace MVC.View.Characters.AI.Behaviours
{
	public class AttackAction : Action
	{
		public AttackAction () : base((ActionSucceedDelegate)null)
		{

		}

		protected override RunStatus Run(object context){
			UnityEngine.Debug.Log ("attack");
			return RunStatus.Success;
		}
	}
}

