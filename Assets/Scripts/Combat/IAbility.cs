using Cysharp.Threading.Tasks;

namespace Combat
{
    public interface IAbility
    {
        string Name { get; }
        int Cost { get; }
        
        /// <summary>
        /// Should wait for ability to finish before subtracting energy cost
        /// When this is true, the bool returned from Perform will determine if the action was successful
        /// And if the energy should be subtracted
        /// </summary>
        bool Blocking { get; }
        
        /// <summary>
        /// Performs the action
        /// </summary>
        /// <seealso cref="Blocking"/>
        UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector);
        
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