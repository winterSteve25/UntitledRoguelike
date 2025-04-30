using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class CombatInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text energyText;
        [SerializeField] private Button nextTurnButton;

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
            CombatManager.Current.NextTurn();
        }
        
        public void IsNextTurn(int turnNumber, bool friendlyTurn)
        {
            if (friendlyTurn)
            {
                turnText.text = "Your Turn";
                energyText.gameObject.SetActive(true);
                nextTurnButton.gameObject.SetActive(true);
            }
            else
            {
                turnText.text = "Opponent Turn";
                energyText.gameObject.SetActive(false);
                nextTurnButton.gameObject.SetActive(false);
            }
        }

        public void UpdateEnergy(int playerEnergy, int maxEnergy)
        {
            energyText.text = $"Energy: {playerEnergy}/{maxEnergy}";
        }
    }
}