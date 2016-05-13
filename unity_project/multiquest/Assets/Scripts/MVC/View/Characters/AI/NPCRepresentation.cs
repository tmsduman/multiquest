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
		
		private BehaviourTreeManager manager;

		private PureMVCImplementations.UnityFacade facade;

		public NPCRepresentation ()
		{
		}

		public void Init(PureMVCImplementations.UnityFacade facade,BehaviourTreeManager manager){
			this.facade = facade;
			this.manager = manager;
		}

		private void Start()
		{
			manager.RegisterBehaviour(this.GetComposite(), this.facade);
		}

		public Composite GetComposite()
		{
			return new PrioritySelector(new Sequence(new GoToEnemyAction(),new AttackAction()),new WalkAction ());
		}
	}

		

}

