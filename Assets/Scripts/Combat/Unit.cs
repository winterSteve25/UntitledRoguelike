using UnityEngine;

namespace Combat
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private GameObject abilitiesParent;
        [SerializeField] private GameObject passivesParent;
        
        public Vector2Int GridPosition { get; private set; }
        public UnitType Type { get; private set; }
        public bool Friendly { get; private set; }
        
        private IAbility[] _abilities;
        private IPassive[] _passives;

        public void Init(UnitType unitType, Vector2Int position, Vector2 worldPosition, bool friendly)
        {
            _abilities = abilitiesParent.GetComponents<IAbility>();
            _passives = passivesParent.GetComponents<IPassive>();
            
            Type = unitType;
            GridPosition = position;
            Friendly = friendly;
            
            transform.position = worldPosition;
        }
    }
}