using MVC.Controller.Input.Notifications;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;

namespace MVC.View.Characters
{
    public class CharacterMediator : PureMVCImplementations.UnityMonoBehaviourMediator<CharacterMediator>
    {
        [SerializeField]
        private Transform characterParent;

        [SerializeField]
        private CharacterRepresentation characterPrefab;

        private void Start()
        {
            for (int i = 0; i < 1; i++)
            {
                this.CreateNewCharacter(this.characterPrefab, i);
            }
        }

        private void CreateNewCharacter(CharacterRepresentation representationPrefab, int id)
        {
            CharacterRepresentation representation = Instantiate<CharacterRepresentation>(representationPrefab);
            representation.CachedTransform.SetParent(this.characterParent);
            representation.CachedTransform.localScale = Vector3.one;
            representation.CachedTransform.localPosition = Vector3.zero;

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Left",
                Command = () =>
                {
                    representation.Move(Data.RepresentationMovementDirections.Left);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Right",
                Command = () =>
                {
                    representation.Move(Data.RepresentationMovementDirections.Right);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Up",
                Command = () =>
                {
                    representation.Move(Data.RepresentationMovementDirections.Up);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "Down",
                Command = () =>
                {
                    representation.Move(Data.RepresentationMovementDirections.Down);
                }
            });
        }
    }
}
