using System.Collections.Generic;
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

        [SerializeField]
        private List<Vector3> possibleSpawnPoints;

        [SerializeField]
        private int minEnemiesOnMap;

        [SerializeField]
        private int maxAdditionalEnemiesOnMap;

        private CharacterProxy characterProxy;

        private System.Random random;
        private int lastSpawnPointIndex;

        private void Start()
        {
            //return;

            this.random = new System.Random();
            this.characterProxy = this.Facade.GetProxy<CharacterProxy>();
            this.CreateEnemies();
        }

        private void CreateEnemies()
        {
            int offset = this.random.Next(0, this.maxAdditionalEnemiesOnMap + 1);
            for (int i = this.characterProxy.Enemies.Count; i < this.minEnemiesOnMap + offset; i++)
            {
                this.CreateNewNPC(this.npcPrefab);
            }
        }

        private void CreateNewNPC(NPCRepresentation representationPrefab)
        {
            NPCRepresentation representation = Instantiate<NPCRepresentation>(representationPrefab);
            representation.CachedTransform.SetParent(this.characterParent);
            representation.CachedTransform.localScale = Vector3.one;

            this.lastSpawnPointIndex = (this.lastSpawnPointIndex + 1) % this.possibleSpawnPoints.Count;

            representation.CachedTransform.localPosition = this.possibleSpawnPoints[this.lastSpawnPointIndex];
            representation.Init (this.Facade, this.manager);

            representation.Killed += this.EnemyKilled;

            this.characterProxy.AddEnemy(representation);
        }

        private void EnemyKilled(CharacterRepresentation representation)
        {
            representation.Killed -= this.EnemyKilled;
            this.characterProxy.RemoveEnemy(representation as NPCRepresentation);

            if (this.characterProxy.Enemies.Count < this.minEnemiesOnMap)
                this.CreateEnemies();
        }
    }
}
