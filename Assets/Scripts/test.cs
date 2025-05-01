using Combat;
using Deck;
using Levels;
using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            var clickedTile = Level.Current.WorldToCell(worldPos);
            Debug.Log(clickedTile);
        }
        
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        CombatManager.Current.NextTurn();
        
        foreach (var unit in CombatManager.Current.ActiveUnits)
        {
            Debug.Log($"{unit.GridPosition} - {unit}");
        }
    }
}