using Deck;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Unit Type", menuName = "Combat/Unit Type")]
    public class UnitType : ScriptableObject, IItem
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField] public Unit Prefab { get; private set; }

        public bool OnlyUseOnGrid => true;
        public Sprite Sprite => Prefab.GetComponent<SpriteRenderer>().sprite;
    }
}