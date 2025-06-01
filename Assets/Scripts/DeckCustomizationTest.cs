using System.Collections.Generic;
using Combat.Deck;
using UnityEngine;

public class DeckCustomizationTest : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<ItemType> items;

    private void Start()
    {
        inventory.Init(new Vector2Int(4, 2));
        inventoryUI.Init(new Vector2Int(4, 2), inventory);
    }
}