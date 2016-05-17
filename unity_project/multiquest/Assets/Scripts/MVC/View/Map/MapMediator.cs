using MVC.View.Map.Notifications;
using System.Collections.Generic;
using UnityEngine;

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

            this.LoadRandomMap();
        }

        public override void BeforeRegister()
        {
            this.RegisterNotification<LoadMapNotification>(n => this.LoadRandomMap());
        }

        private void LoadRandomMap()
        {
            int newMapId = this.random.Next(0, this.possibleMaps.Count);
            for (int i = 0; i < this.possibleMaps.Count; i++)
            {
                this.possibleMaps[i].SetActive(false);
            }

            this.possibleMaps[newMapId].SetActive(true);
        }
    }
}
