using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour {
  [SerializeField] private AcceptedItemsData acceptedItemsData;
  [SerializeField] private List<ContainerSlot> containerSlots = new();
  [field: SerializeField] public Highlightable Highlighting { get; set; } = new();

  public int ContainedCount {
    get {
      int count = 0;
      foreach (ContainerSlot slot in containerSlots) {
        if (slot.ContainedItem != null) {
          count++;
        }
      }
      return count;
    }
  }

  public bool AddItem(Item item) {
    if (AcceptsItem(item)) {
      for (int i = 0; i < containerSlots.Count; i++) {
        if (containerSlots[i].ContainedItem == null) {
          AddItem(item, i);
          return true;
        } else if (containerSlots[i].ContainedItem.IsContainer && containerSlots[i].ContainedItem.Container.AddItem(item)) {
          return true;
        }
      }
    }
    return false;
  }

  public void AddItem(Item item, int slot) {
    if (!AcceptsItem(item)) {
      return;
    }
    item.ContainedBy.RemoveItem(item);
    containerSlots[slot].ContainedItem = item;
    item.ContainedBy = this;
    item.transform.SetParent(containerSlots[slot].ContainmentPosition, false);
  }

  public void TakeItem(ItemContainer itemContainer, int slot = 0) {
    AddItem(itemContainer.containerSlots[slot].ContainedItem);
  }

  protected bool Contains(ItemContainer itemContainer) {
    foreach (var slot in containerSlots) {
      bool slotContainsItem = slot.ContainedItem != null && (slot.ContainedItem.Container == itemContainer || (slot.ContainedItem.IsContainer && slot.ContainedItem.Container.Contains(itemContainer)));
      if (slotContainsItem) {
        return true;
      }
    }
    return false;
  }

  private bool AcceptsItem(Item item) {
    return item != null && acceptedItemsData.AcceptedItems.Contains(item.Data);
  }

  private int RemoveItem(Item item) {
    for (int i = 0; i < containerSlots.Count; i++) {
      if (containerSlots[i].ContainedItem == item) {
        RemoveItem(i);
        return i;
      }
    }
    return -1;
  }

  private void RemoveItem(int slot) {
    containerSlots[slot].ContainedItem.ContainedBy = null;
    containerSlots[slot].ContainedItem = null;
  }

  [System.Serializable]
  private class ContainerSlot {
    [field: SerializeField] public Item ContainedItem { get; set; }
    [field: SerializeField] public Transform ContainmentPosition { get; set; }
  }
}
