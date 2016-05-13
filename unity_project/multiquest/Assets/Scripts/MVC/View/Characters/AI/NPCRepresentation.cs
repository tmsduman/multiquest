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

		public float ViewRange = 10;
		
		private BehaviourTreeManager manager;
		private Composite behaviour;
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
			this.behaviour = this.GetComposite ();
			manager.RegisterBehaviour(this.behaviour, this);
		}

		private void OnDisable(){
			manager.UnregisterBehaviour (this.behaviour);
		}

		public Composite GetComposite()
		{
			return new PrioritySelector(new Sequence(new WatchOutForEnemyAction (), new PrioritySelector(new AttackAction(),new GoToEnemyAction()),new WalkAction ()));
		}

		public PureMVCImplementations.UnityFacade GetFacade(){
			return this.facade;
		}
	}

		

}

