using TreeSharp;

namespace MVC.View.Characters.AI.Behaviours
{
	public abstract class AbstractAction : Action
	{
		public AbstractAction () : base((ActionSucceedDelegate)null)
		{

		}

		protected NPCRepresentation Unwrap(object context){
			return (NPCRepresentation)context;
		}

	
	}
}

