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
        private UnityEngine.Camera gameCamera;

        [SerializeField]
        private Transform cameraTransform;

        [SerializeField]
        private float moveTime;

        private CharacterProxy characterProxy;
        private List<PlayerRepresentation> registeredPlayers = new List<PlayerRepresentation>();

        private Vector4 borders;

        private void Start()
        {
            this.characterProxy = this.Facade.GetProxy<CharacterProxy>();
            this.characterProxy.PlayerAdded += player => this.AddPlayer(player);
            this.characterProxy.PlayerRemoved += player => this.RemovePlayer(player);

            foreach (var player in this.characterProxy.Players)
            {
                this.AddPlayer(player);
            }
        }

        public override void BeforeRegister()
        {
            this.RegisterNotification<MVC.View.Map.Notifications.MapLoadedNotification>(n =>
            {
                this.borders = Vector4.zero;

                Vector3 currentPos = this.cameraTransform.position;
                

                float interval = 0.1f;
                
                Ray ray;


                this.cameraTransform.position = new Vector3(10, 0, this.cameraTransform.position.z);
                this.borders.x = 10;
                // left
                while (true)
                {
                    this.cameraTransform.position -= new Vector3(interval, 0, 0);
                    ray = this.gameCamera.ViewportPointToRay(Vector3.zero);

                    bool mapHitted = false;
                    foreach (var hit in Physics.RaycastAll(ray))
                    {
                        if (hit.collider.gameObject.tag == "Map")
                        {
                            mapHitted = true;
                            break;
                        }
                    }

                    if (mapHitted)
                        this.borders.x -= interval;
                    else
                        break;
                }

                this.cameraTransform.position = new Vector3(-10, 0, this.cameraTransform.position.z);
                this.borders.y = -10;
                // right
                while (true)
                {
                    this.cameraTransform.position += new Vector3(interval, 0, 0);
                    ray = this.gameCamera.ViewportPointToRay(Vector3.one);

                    Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue, 100);

                    bool mapHitted = false;
                    foreach (var hit in Physics.RaycastAll(ray))
                    {
                        if (hit.collider.gameObject.tag == "Map")
                        {
                            mapHitted = true;
                            break;
                        }
                    }

                    if (mapHitted)
                        this.borders.y += interval;
                    else
                        break;
                }

                this.cameraTransform.position = new Vector3(-10, -10, this.cameraTransform.position.z);
                this.borders.w = -10;
                // top
                while (true)
                {
                    this.cameraTransform.position += new Vector3(0, interval, 0);
                    ray = this.gameCamera.ViewportPointToRay(Vector3.one);

                    bool mapHitted = false;
                    foreach (var hit in Physics.RaycastAll(ray))
                    {
                        if (hit.collider.gameObject.tag == "Map")
                        {
                            mapHitted = true;
                            break;
                        }
                    }

                    if (mapHitted)
                        this.borders.w += interval;
                    else
                        break;
                }

                this.cameraTransform.position = new Vector3(0, 10, this.cameraTransform.position.z);
                this.borders.z = 10;
                // bottom
                while (true)
                {
                    this.cameraTransform.position -= new Vector3(0, interval, 0);
                    ray = this.gameCamera.ViewportPointToRay(Vector3.zero);

                    bool mapHitted = false;
                    foreach (var hit in Physics.RaycastAll(ray))
                    {
                        if (hit.collider.gameObject.tag == "Map")
                        {
                            mapHitted = true;
                            break;
                        }
                    }

                    if (mapHitted)
                        this.borders.z -= interval;
                    else
                        break;
                }

                //Debug.Log("borders " + this.borders);

                this.cameraTransform.position = currentPos;

                this.HandlePlayerMoved();
            });
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
                    Mathf.Min(this.borders.y, Mathf.Max(this.borders.x, minValues.x + ((maxValues.x - minValues.x) * 0.5f))),
                    Mathf.Min(this.borders.w, Mathf.Max(this.borders.z, minValues.y + ((maxValues.y - minValues.y) * 0.5f))),
                    this.cameraTransform.position.z),
                this.moveTime);
        }
    }
}
