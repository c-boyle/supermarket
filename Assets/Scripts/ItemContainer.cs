using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour, IHighlightable {
  [SerializeField] private AcceptedItemsData acceptedItemsData;
  [SerializeField] private List<ContainerSlot> containerSlots = new();
  [SerializeField] private Highlightable highlighting;
  private int selectedContainerSlotIndex = -1;

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

  public bool Highlighted {
    get => highlighting != null ? highlighting.Highlighted : false;
    set {
      bool hasMultipleSlotsAndAnItem = containerSlots.Count > 1 && ContainedCount > 0;
      if (Highlighted != value && hasMultipleSlotsAndAnItem) {
        if (value) {
          selectedContainerSlotIndex = FirstOccupiedSlotIndex;
        }
        if (selectedContainerSlotIndex != -1) {
          containerSlots[selectedContainerSlotIndex].ContainedItem.Highlighted = value;
        }
      }
      highlighting.Highlighted = value;
    }
  }

  private int FirstOccupiedSlotIndex {
    get {
      for (int i = 0; i < containerSlots.Count; i++) {
        if (containerSlots[i].ContainedItem != null) {
          return i;
        }
      }
      return -1;
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
    if (containerSlots.Count > 1 && selectedContainerSlotIndex == -1) {
      selectedContainerSlotIndex = slot;
      containerSlots[selectedContainerSlotIndex].ContainedItem.Highlighted = Highlighted;
    }
  }

  public void TakeItem(ItemContainer itemContainer) {
    var itemToTake = itemContainer.containerSlots.Count > 1 ? itemContainer.containerSlots[itemContainer.selectedContainerSlotIndex].ContainedItem : itemContainer.containerSlots[0].ContainedItem;
    AddItem(itemToTake);
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
    if (containerSlots[slot].ContainedItem != null) {
      containerSlots[slot].ContainedItem.Highlighted = false;
    }
    if (selectedContainerSlotIndex == slot) {
      selectedContainerSlotIndex = -1;
    }
    containerSlots[slot].ContainedItem.ContainedBy = null;
    containerSlots[slot].ContainedItem = null;
  }

  [System.Serializable]
  private class ContainerSlot {
    [field: SerializeField] public Item ContainedItem { get; set; }
    [field: SerializeField] public Transform ContainmentPosition { get; set; }
  }
}
