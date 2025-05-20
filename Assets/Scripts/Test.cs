using System.Collections.Generic;
using Combat;
using Combat.Deck;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private List<UnitType> givens;
    [SerializeField] private Camera cam;
        
    private void Start()
    {
        inventoryUI.AddItemsAnywhere(givens);
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
        
        foreach (var u in CombatManager.Current.ActiveGadgets)
        {
            Debug.Log($"{u} - {u.GridPositionSync}");
        }
    }
}