using Cysharp.Threading.Tasks;

namespace Combat
{
    public interface IAbility
    {
        string Name { get; }
        int Cost { get; }
        
        UniTaskVoid Perform(CombatManager combatManager, Unit unit);
        
        protected static async UniTask UntilNextTurn(CombatManager combatManager)
        {
            await UniTask.WaitUntil(combatManager.TurnNumber, n => n != combatManager.TurnNumber);
        }

        protected static async UniTask UntilNextFriendlyTurn(CombatManager combatManager)
        {
            var friendliness = combatManager.FriendlyTurn;
            await UniTask.WaitUntil(combatManager.TurnNumber, n => n != combatManager.TurnNumber && combatManager.FriendlyTurn == friendliness);
        }
    }
}