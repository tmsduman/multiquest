using MVC.Controller.Input.Notifications;
using MVC.Model.Character;
using MVC.View.Characters.MonoBehaviours;
using MVC.View.Characters.AI;
using UnityEngine;
using TreeSharp;

namespace MVC.View.Characters
{
	public class EnemyMediator : PureMVCImplementations.UnityMonoBehaviourMediator<EnemyMediator>
	{
		[SerializeField]
		private Transform characterParent;

		[SerializeField]
		private NPCRepresentation npcPrefab;

		[SerializeField]
		private BehaviourTreeManager manager;

		private CharacterProxy characterProxy;


		private void Start()
		{
			this.characterProxy = this.Facade.GetProxy<CharacterProxy>();
			for (int i = 0; i < 1; i++)
			{
				this.CreateNewNPC(this.npcPrefab, i);
			}
		}



		private void CreateNewNPC(NPCRepresentation representationPrefab, int id)
		{
			NPCRepresentation representation = Instantiate<NPCRepresentation>(representationPrefab);
			representation.CachedTransform.SetParent(this.characterParent);
			representation.CachedTransform.localScale = Vector3.one;
			representation.CachedTransform.localPosition = Vector3.one * 200;
			representation.Init (this.Facade, this.manager);


			this.characterProxy.AddPlayer(representation);
		}
	}
}
