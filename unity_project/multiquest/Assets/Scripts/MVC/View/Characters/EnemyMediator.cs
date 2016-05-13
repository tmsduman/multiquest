using MVC.Controller.Input.Notifications;
using MVC.Model.Character;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;

namespace MVC.View.Characters
{
    public class EnemyMediator : PureMVCImplementations.UnityMonoBehaviourMediator<EnemyMediator>
    {
        [SerializeField]
        private Transform enemyParent;

        [SerializeField]
        private CharacterRepresentation enemyPrefab;

        private CharacterProxy characterProxy;

        protected override void Awake()
        {
            base.Awake();
            
        }

        private void Start()
        {
            this.characterProxy = this.Facade.GetProxy<CharacterProxy>();

            for (int i = 0; i < 1; i++)
            {
                this.CreateNewEnemy(this.enemyPrefab, i);
            }
        }

        private void CreateNewEnemy(CharacterRepresentation representationPrefab, int id)
        {
            CharacterRepresentation representation = Instantiate<CharacterRepresentation>(representationPrefab);
            representation.CachedTransform.SetParent(this.enemyParent);
            representation.CachedTransform.localScale = Vector3.one;
            representation.CachedTransform.localPosition = Vector3.one * -200;

            this.characterProxy.AddEnemy(representation);
        }
    }
}
