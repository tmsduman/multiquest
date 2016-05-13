using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSharp;
using MVC.View.Characters.AI.Behaviours;
using UnityEngine;
using MVC.View.Characters.MonoBehaviours;

namespace MVC.View.Characters.AI
{
	public class NPCRepresentation : CharacterRepresentation
	{
		[SerializeField]
		private BehaviourTreeManager manager;

		public NPCRepresentation ()
		{
		}

		public void Init(PureMVCImplementations.UnityFacade facade){
			facade.GetProxy<MVC.Model.Character.CharacterProxy> ();
		}

		private void Start()
		{
			manager.RegisterBehaviour(this.GetComposite(), null);
		}

		public Composite GetComposite()
		{
			return new PrioritySelector(new Sequence(new GoToEnemyAction(),new AttackAction()),new WalkAction ());
		}
	}

		

}

