using System.Collections;
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
        private int minEnemiesOnMap;

        [SerializeField]
        private int maxAdditionalEnemiesOnMap;

        private CharacterProxy characterProxy;

        private System.Random random;
        private int lastSpawnPointIndex;
        private List<Vector3> possibleSpawnPoints;

        protected override void Awake()
        {
            this.random = new System.Random();
            this.characterProxy = this.Facade.GetProxy<CharacterProxy>();
            base.Awake();
        }

        public override void BeforeRegister()
        {
            this.RegisterNotification<MVC.View.Map.Notifications.MapLoadedNotification>(n =>
            {
                this.possibleSpawnPoints = n.EnemySpawnPoints;
                this.lastSpawnPointIndex = 0;

                List<NPCRepresentation> tempNPCList = new List<NPCRepresentation>(this.characterProxy.Enemies);

                for (int i = 0; i < tempNPCList.Count; i++)
                {
                    tempNPCList[i].gameObject.SetActive(false);
                    this.RemoveRepresentation(tempNPCList[i]);
                }

                this.StartCoroutine(this.WaitToDestroyObsoleteEnemies(tempNPCList));

                this.CreateEnemies();
            });
        }

        private IEnumerator WaitToDestroyObsoleteEnemies(List<NPCRepresentation> players)
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < players.Count; i++)
            {
                DestroyImmediate(players[i].gameObject);
            }
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
            this.RemoveRepresentation(representation);

            if (this.characterProxy.Enemies.Count < this.minEnemiesOnMap)
                this.CreateEnemies();
        }

        private void RemoveRepresentation(CharacterRepresentation representation)
        {
            representation.Killed -= this.EnemyKilled;
            this.characterProxy.RemoveEnemy(representation as NPCRepresentation);
        }
    }
}
