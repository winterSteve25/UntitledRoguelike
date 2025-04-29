using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.Slime
{
    public class SplitAbility : MonoBehaviour, IAbility
    {
        public async UniTaskVoid Perform(CombatManager combatManager)
        {
            await UniTask.DelayFrame(1);
        }
    }
}