using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order {
  [SerializeField] private List<ItemOrder> itemOrders = new();
  private Dictionary<ItemData, int> _orderedItemToQuantity = null;
  private Dictionary<ItemData, int> OrderedItemToQuantity {
    get {
      if (_orderedItemToQuantity == null) {
        _orderedItemToQuantity = new();
        foreach (var itemOrder in itemOrders) {
          _orderedItemToQuantity[itemOrder.ItemData] = itemOrder.Quantity;
        }
      }
      return _orderedItemToQuantity;
    }
  }

  public bool IsSatisfiedBy(ItemContainer container) {
    Dictionary<ItemData, int> containedItemToQuantity = null;
    foreach (var containedItem in container.ContainedItems) {
      int itemCount = 1;
      if (containedItemToQuantity.TryGetValue(containedItem.Data, out int quantity)) {
        itemCount = quantity++;
      }
      containedItemToQuantity[containedItem.Data] = itemCount;
    }
    foreach (var order in itemOrders) {
      if (!containedItemToQuantity.TryGetValue(order.ItemData, out var quantity) || order.Quantity > quantity) {
        return false;
      }
    }
    return true;
  }

  [System.Serializable]
  private class ItemOrder {
    [field: SerializeField] public ItemData ItemData { get; set; }
    [field: SerializeField] public int Quantity { get; set; }
  }
}