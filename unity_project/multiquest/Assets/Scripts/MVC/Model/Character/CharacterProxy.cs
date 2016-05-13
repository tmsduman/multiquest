using System.Collections.Generic;
using MVC.View.Characters.MonoBehaviours;

namespace MVC.Model.Character
{
    public class CharacterProxy : MVC.Core.PureMVCImplementations.UnityProxy<CharacterProxy>
    {
        public readonly List<CharacterRepresentation> Players = new List<CharacterRepresentation>();

        public void AddPlayer(CharacterRepresentation player)
        {
            if (!this.Players.Contains(player))
                this.Players.Add(player);
        }

        public void RemovePlayer(CharacterRepresentation player)
        {
            if (this.Players.Contains(player))
                this.Players.Remove(player);
        }
    }
}
