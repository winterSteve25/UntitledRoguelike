using Combat;
using Deck;
using Levels;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
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
            var mp = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var gp = Level.Current.WorldToCell(mp);
            Debug.Log(gp);
        }
        
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        CombatManager.Current.NextTurn();
        Debug.Log(CombatManager.Current.Me);
    }
}