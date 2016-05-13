using System.Collections.Generic;
using MVC.Controller.Input.Notifications;
using MVC.Model.Character;
using MVC.View.Characters.Data;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;

namespace MVC.View.Characters
{
    public class CharacterMediator : PureMVCImplementations.UnityMonoBehaviourMediator<CharacterMediator>
    {
        [SerializeField]
        private Transform characterParent;

        [SerializeField]
        private PlayerRepresentation characterPrefab;

        private CharacterProxy characterProxy;

        protected override void Awake()
        {
            base.Awake();
            
        }

        private void Start()
        {
            this.characterProxy = this.Facade.GetProxy<CharacterProxy>();

            for (int i = 0; i < 2; i++)
            {
                this.CreateNewCharacter(this.characterPrefab, i);
            }
        }

        private void CreateNewCharacter(PlayerRepresentation representationPrefab, int id)
        {
            PlayerRepresentation representation = Instantiate<PlayerRepresentation>(representationPrefab);
            representation.CachedTransform.SetParent(this.characterParent);
            representation.CachedTransform.localScale = Vector3.one;
            representation.CachedTransform.localPosition = new Vector3(1,1,0) * id * 2;

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Left" + id,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Left);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Right" + id,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Right);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Up" + id,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Up);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Down" + id,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Down);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Attack" + id,
                Command = () =>
                {
                    representation.Attack();
                }
            });

            this.characterProxy.AddPlayer(representation);
        }
    }
}
