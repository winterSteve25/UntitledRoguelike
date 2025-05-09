using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.UI
{
    public class CombatInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private Button nextTurnButton;
        [SerializeField] private Image[] energies;

        private void OnEnable()
        {
            CombatManager.Current.OnTurnChanged += IsNextTurn;
        }

        private void OnDisable()
        {
            if (CombatManager.Current == null) return;
            CombatManager.Current.OnTurnChanged -= IsNextTurn;
        }

        public void NextTurn()
        {
            CombatManager.Current.NextTurnRpc();
        }
        
        public void IsNextTurn(int turnNumber, bool friendly)
        {
            if (friendly == CombatManager.Current.AmIFriendly)
            {
                turnText.text = "Your Turn";
                nextTurnButton.gameObject.SetActive(true);
            }
            else
            {
                turnText.text = "Opponent Turn";
                nextTurnButton.gameObject.SetActive(false);
            }
        }

        public void UpdateEnergy(int playerEnergy)
        {
            foreach (var energy in energies)
            {
                energy.gameObject.SetActive(false);
            }

            for (var i = 0; i < playerEnergy; i++)
            {
                energies[i].gameObject.SetActive(true);
            }
        }
    }
}