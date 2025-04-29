using Plugins.InventoryTetris;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Unit Type", menuName = "Combat/Unit Type")]
    public class UnitType : ScriptableObject, IItemData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField] public Unit Prefab { get; private set; }

        public Sprite Icon => Prefab.GetComponent<SpriteRenderer>().sprite;
        public Color BackgroundColor => Color.white;
    }
}