using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat.UI
{
    public class AbilitiesButtonGroup : MonoBehaviour
    {
        [SerializeField] private RectTransform btnsGroup;
        [SerializeField] private AbilityButton abilityBtnPrefab;

        public List<(AbilityButton, int)> List { get; private set; }
        private bool _interactable;

        private void Start()
        {
            List = new List<(AbilityButton, int)>();
        }

        public void Clear()
        {
            foreach (var btn in List)
            {
                Destroy(btn.Item1.gameObject);
            }

            List.Clear();
        }

        public void AddAbility(CombatManager combatManager, Unit unit, IAbility ability, ScrollUI scrollUI,
            IAreaSelector areaSelector)
        {
            // TODO: Use pools
            var btn = Instantiate(abilityBtnPrefab, btnsGroup);
            btn.Interactable = unit.Interactable
                               && combatManager.AmIFriendly == unit.Friendly
                               && combatManager.Me.Energy >= ability.Cost
                               && combatManager.MyTurn;
            btn.Init(() =>
            {
                if (ability.Blocking)
                {
                    UniTask.Void(async () =>
                    {
                        var successful = await ability.Perform(combatManager, unit, areaSelector);
                        if (!successful) return;
                        combatManager.Me.Energy -= ability.Cost;
                    });
                }
                else
                {
                    combatManager.Me.Energy -= ability.Cost;
                    ability.Perform(combatManager, unit, areaSelector)
                        .Forget();
                }
            }, ability, scrollUI);

            List.Add((btn, ability.Cost));
        }
    }
}