using MVC.Model.Character;
using MVC.View.Characters.AI;
using MVC.View.Characters.MonoBehaviours;
using TreeSharp;
using UnityEngine;

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
            representation.CachedTransform.localPosition = new Vector3(1,1,0) * -2;
            representation.Init (this.Facade, this.manager);

            representation.Killed += this.EnemyKilled;

            this.characterProxy.AddEnemy(representation);
        }

        private void EnemyKilled(CharacterRepresentation representation)
        {
            representation.Killed -= this.EnemyKilled;
            this.characterProxy.RemoveEnemy(representation as NPCRepresentation);
        }
    }
}
