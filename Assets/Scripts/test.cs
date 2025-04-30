using Combat;
using Deck;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class test : MonoBehaviour
{
    [FormerlySerializedAs("inventory")] [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private UnitType a;
    [SerializeField] private UnitType b;
    [SerializeField] private Camera cam;
        
    private void Start()
    {
        inventoryUI.AddItem(a, new Vector2Int(0, 0));
        inventoryUI.AddItem(b, new Vector2Int(2, 0));
        CombatManager.Current.NextTurn();
    }

    private void Update()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        CombatManager.Current.NextTurn();
    }
}