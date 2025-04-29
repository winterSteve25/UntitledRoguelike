using Cysharp.Threading.Tasks;

namespace Combat
{
    public interface IAbility
    {
        UniTaskVoid Perform(CombatManager combatManager);
    }
}