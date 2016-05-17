using System;
using System.Collections.Generic;
using MVC.View.Characters.AI;
using MVC.View.Characters.MonoBehaviours;

namespace MVC.Model.Character
{
    public class CharacterProxy : MVC.Core.PureMVCImplementations.UnityProxy<CharacterProxy>
    {
        public event Action<PlayerRepresentation> PlayerAdded;
        public event Action<PlayerRepresentation> PlayerRemoved;
        public event Action<NPCRepresentation> EnemyAdded;
        public event Action<NPCRepresentation> EnemyRemoved;

        public readonly List<PlayerRepresentation> Players = new List<PlayerRepresentation>();
        public readonly List<NPCRepresentation> Enemies = new List<NPCRepresentation>();

        public void AddPlayer(PlayerRepresentation player)
        {
            if (!this.Players.Contains(player))
            {
                this.Players.Add(player);
                var tempEvent = this.PlayerAdded;
                if (tempEvent != null)
                    tempEvent(player);
            }
        }

        public void RemovePlayer(PlayerRepresentation player)
        {
            if (this.Players.Contains(player))
            { 
                this.Players.Remove(player);
                var tempEvent = this.PlayerRemoved;
                if (tempEvent != null)
                    tempEvent(player);
            }
        }

        public void AddEnemy(NPCRepresentation enemy)
        {
            if (!this.Enemies.Contains(enemy))
            { 
                this.Enemies.Add(enemy);
                var tempEvent = this.EnemyAdded;
                if (tempEvent != null)
                    tempEvent(enemy);
            }
        }

        public void RemoveEnemy(NPCRepresentation enemy)
        {
            if (this.Enemies.Contains(enemy))
            { 
                this.Enemies.Remove(enemy);
                var tempEvent = this.EnemyRemoved;
                if (tempEvent != null)
                    tempEvent(enemy);
            }
        }
    }
}
