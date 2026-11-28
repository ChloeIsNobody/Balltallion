using Balltallion.Players;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using Balltallion.UI.Components.PagedGrid;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class DraftingManager : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO player1;
        [SerializeField] private LocalPlayerSO player2;
        
        [SerializeField, Scene] private string nextSceneToLoad;

        private void Awake()
        {
            player1.teamLoadout.ResetLoadoutPointer();
            player2.teamLoadout.ResetLoadoutPointer();
        }
    }
}