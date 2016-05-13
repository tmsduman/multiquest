using System.Collections.Generic;
using MVC.Model.Character;
using MVC.View.Characters.MonoBehaviours;
using UnityEngine;
using DG.Tweening;

namespace MVC.View.Camera
{
    public class GameCameraMediator : PureMVCImplementations.UnityMonoBehaviourMediator<GameCameraMediator>
    {
        [SerializeField]
        private Transform cameraTransform;

        [SerializeField]
        private float moveTime;

        private CharacterProxy characterProxy;
        private List<PlayerRepresentation> registeredPlayers = new List<PlayerRepresentation>();

        private void Start()
        {
            this.characterProxy = this.Facade.GetProxy<CharacterProxy>();
            this.characterProxy.PlayerAdded += player => this.AddPlayer(player);
            this.characterProxy.PlayerRemoved += player => this.RemovePlayer(player);

            foreach (var player in this.characterProxy.Players)
            {
                this.AddPlayer(player);
            }

            this.HandlePlayerMoved();
        }

        private void AddPlayer(PlayerRepresentation player)
        {
            if (!this.registeredPlayers.Contains(player))
            {
                player.Moved += this.HandlePlayerMoved;
                this.registeredPlayers.Add(player);
                this.HandlePlayerMoved();
            }
        }

        private void RemovePlayer(PlayerRepresentation player)
        {
            if (this.registeredPlayers.Contains(player))
            {
                player.Moved -= this.HandlePlayerMoved;
                this.registeredPlayers.Remove(player);
                this.HandlePlayerMoved();
            }
        }

        private void HandlePlayerMoved()
        {
            //return;

            if (this.registeredPlayers.Count == 0)
                return;

            Vector3 minValues = this.registeredPlayers[0].CachedTransform.position;
            Vector3 maxValues = this.registeredPlayers[0].CachedTransform.position;

            for (int i = 1; i < this.registeredPlayers.Count; i++)
            {
                if (minValues.x > this.registeredPlayers[i].CachedTransform.position.x)
                    minValues.x = this.registeredPlayers[i].CachedTransform.position.x;

                if (minValues.y > this.registeredPlayers[i].CachedTransform.position.y)
                    minValues.y = this.registeredPlayers[i].CachedTransform.position.y;

                if (maxValues.x < this.registeredPlayers[i].CachedTransform.position.x)
                    maxValues.x = this.registeredPlayers[i].CachedTransform.position.x;

                if (maxValues.y < this.registeredPlayers[i].CachedTransform.position.y)
                    maxValues.y = this.registeredPlayers[i].CachedTransform.position.y;
            }
            
            this.cameraTransform.DOMove(
                new Vector3(
                    minValues.x + ((maxValues.x - minValues.x) * 0.5f),
                    minValues.y + ((maxValues.y - minValues.y) * 0.5f),
                    this.cameraTransform.position.z),
                this.moveTime);
        }
    }
}
