using System.Collections.Generic;
using System.Linq;
using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Levels
{
    public abstract class LevelAI : MonoBehaviour
    {
        protected List<Unit> Units;
        protected CombatManager CombatMan;
        
        private void Start()
        {
            CombatMan = CombatManager.Current;
            CombatMan.OnTurnChanged += CurrentOnOnTurnChanged;
        }
        
        public void Init(List<Unit> units)
        {
            Units = units;
        }

        private void CurrentOnOnTurnChanged(int turnNumber, bool isFriendly)
        {
            if (isFriendly) return;
            DecideMove();
            CombatMan.NextTurn();
        }

        protected void TryUseAbility<T>(Unit unit, IAreaSelector areaSelector)
        {
            var f = unit.Abilities.FirstOrDefault(x => x is T);
            if (f == null) return;
            f.Perform(CombatMan, unit, areaSelector).Forget();
        }

        protected abstract void DecideMove();
    }
}