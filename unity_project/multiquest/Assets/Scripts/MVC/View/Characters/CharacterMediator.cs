using System.Collections;
using System.Collections.Generic;
using MVC.Controller.Input.Notifications;
using MVC.Model.Character;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVC.View.Characters
{
    public class CharacterMediator : PureMVCImplementations.UnityMonoBehaviourMediator<CharacterMediator>
    {
        [SerializeField]
        private Transform characterParent;

        [SerializeField]
        private PlayerRepresentation characterPrefab;

        [SerializeField]
        private UnityEngine.Camera gameCamera;

        private CharacterProxy characterProxy;

        private void Start()
        {
            this.characterProxy = this.Facade.GetProxy<CharacterProxy>();

            this.CreateAllCharacter();
        }

        public override void BeforeRegister()
        {
            this.RegisterNotification<MVC.View.Bases.Notifications.MapFinishedNotification>(n => 
            {
                List<PlayerRepresentation> tempPlayerList = new List<PlayerRepresentation>(this.characterProxy.Players);

                for (int i = 0; i < tempPlayerList.Count; i++)
                {
                    
                    this.PlayerKilled(tempPlayerList[i]);
                    tempPlayerList[i].gameObject.SetActive(false);
                }

                this.StartCoroutine(this.WaitToDestroyObsoletePlayers(tempPlayerList));
            });
        }

        private IEnumerator WaitToDestroyObsoletePlayers(List<PlayerRepresentation> players)
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < players.Count; i++)
            {
                DestroyImmediate(players[i].gameObject);
            }
        }

        private void CreateAllCharacter()
        {
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
            representation.CachedTransform.localPosition = new Vector3(1,0,0) * id;

            representation.SetCamera(this.gameCamera);
            representation.Killed += this.PlayerKilled;

            representation.InputId = id;

            #region game pad

            this.SendNotification(new RegisterAxisInputCommandNotification()
            {
                InputName = (id + 1) + "_X axis",
                TriggerValue = -1,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Left);
                }
            });

            this.SendNotification(new RegisterAxisInputCommandNotification()
            {
                InputName = (id + 1) + "_X axis",
                TriggerValue = 1,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Right);
                }
            });

            this.SendNotification(new RegisterAxisInputCommandNotification()
            {
                InputName = (id + 1) + "_Y axis",
                TriggerValue = -1,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Down);
                }
            });

            this.SendNotification(new RegisterAxisInputCommandNotification()
            {
                InputName = (id + 1) + "_Y axis",
                TriggerValue = 1,
                Command = () =>
                {
                    representation.Move(Data.RepresentationPossibleDirections.Up);
                }
            });

            this.SendNotification(new RegisterInputCommandNotification()
            {
                InputName = "joystick " + (id + 1) + " button 0",
                Command = () =>
                {
                    representation.Attack();
                }
            });

            #endregion

            #region keyboard

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

            #endregion

            this.characterProxy.AddPlayer(representation);
        }

        private void PlayerKilled(CharacterRepresentation representation)
        {
            representation.Killed -= this.PlayerKilled;

            this.characterProxy.RemovePlayer(representation as PlayerRepresentation);

            this.SendNotification(new UnregisterInputCommandNotification() { InputName = "Left" + representation.InputId });
            this.SendNotification(new UnregisterInputCommandNotification() { InputName = "Right" + representation.InputId });
            this.SendNotification(new UnregisterInputCommandNotification() { InputName = "Up" + representation.InputId });
            this.SendNotification(new UnregisterInputCommandNotification() { InputName = "Down" + representation.InputId });

            if (this.characterProxy.Players.Count <= 0)
            {
                //this.SendNotification(new MVC.View.Bases.Notifications.MapFinishedNotification());
                this.CreateAllCharacter();
            }
        }

        private void OnGUI()
        {
            //GUILayout.BeginVertical();
            //for (int i = 0;i < 20; i++) 
            //{

            //    if (Input.GetKeyDown("joystick 1 button " + i))
            //    {
            //        GUILayout.Label("joystick 1 button " + i);
            //    }

            //     if (Input.GetAxis("1_X axis " + i) != 0)
            //     {
            //         GUILayout.Label("1_X axis " + i);
            //     }
                 
            //}
            //GUILayout.EndVertical();
        }
    }
}
