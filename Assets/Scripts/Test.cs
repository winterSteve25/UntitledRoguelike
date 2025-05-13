using Combat;
using Combat.Deck;
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
    }

    private void Update()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        Debug.Log(CombatManager.Current.Me);
        Debug.Log(CombatManager.Current.TurnNumberSync);

        foreach (var u in CombatManager.Current.ActiveUnits)
        {
            Debug.Log($"{u} - {u.GridPositionSync}");
        }
    }
}