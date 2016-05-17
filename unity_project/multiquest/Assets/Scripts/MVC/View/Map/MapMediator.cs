using MVC.View.Map.Notifications;
using System.Collections.Generic;
using UnityEngine;
using MVC.View.Map.MonoBehaviours;
using MVC.View.Characters.MonoBehaviours;

namespace MVC.View.Map
{
    public class MapMediator : PureMVCImplementations.UnityMonoBehaviourMediator<MapMediator>
    {
        [SerializeField]
        private List<GameObject> possibleMaps;

        private System.Random random;

        protected override void Awake()
        {
            this.random = new System.Random();
            base.Awake();

            for (int i = 0; i < this.possibleMaps.Count; i++)
            {
                foreach (MapExitRepresentation mapExit in this.possibleMaps[i].GetComponentsInChildren<MapExitRepresentation>())
                {
                    mapExit.SetFacade(this.Facade);
                }
            }            
        }

        private void Start()
        {
            this.LoadRandomMap();
        }

        public override void BeforeRegister()
        {
            this.RegisterNotification<LoadMapNotification>(n => this.LoadRandomMap());
            this.RegisterNotification<MVC.View.Bases.Notifications.MapFinishedNotification>(n => this.LoadRandomMap());
        }

        private void LoadRandomMap()
        {
            int newMapId = this.random.Next(0, this.possibleMaps.Count);
            for (int i = 0; i < this.possibleMaps.Count; i++)
            {
                this.possibleMaps[i].SetActive(false);
            }

            this.possibleMaps[newMapId].SetActive(true);

            List<Vector3> enemySpawnPoints = new List<Vector3>();
            foreach (EnemySpawnPoint spawnPoint in this.possibleMaps[newMapId].GetComponentsInChildren<EnemySpawnPoint>())
            {
                if (!enemySpawnPoints.Contains(spawnPoint.transform.position))
                    enemySpawnPoints.Add(spawnPoint.transform.position);
            }

            this.SendNotification(new MapLoadedNotification()
            {
                EnemySpawnPoints = enemySpawnPoints
            });
        }
    }
}
