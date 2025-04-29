using Combat;
using Plugins.InventoryTetris;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    
    [SerializeField] private UnitType unitType;
    [SerializeField] private UnitType unitType2;

    private void Start()
    {
        inventory.AddItem(unitType);
        inventory.AddItem(unitType2);
    }
}