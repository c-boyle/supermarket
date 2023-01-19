using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour, IHighlightable, IInteractable {
  [SerializeField] private AcceptedItemsData acceptedItemsData;
  [SerializeField] private List<ContainerSlot> containerSlots = new();
  [SerializeField] private Highlightable highlighting;
  private int selectedContainerSlotIndex = -1;

  private bool _highlightSelectedItemEnabled = true;
  public bool HighlightSelectedItemEnabled { 
    get => _highlightSelectedItemEnabled; 
    set {
      bool flippedTrue = _highlightSelectedItemEnabled != value && value;
      _highlightSelectedItemEnabled = value;
      if (flippedTrue) {
        SelectItem(true);
      }
      if (!value && selectedContainerSlotIndex != -1) {
        containerSlots[selectedContainerSlotIndex].ContainedItem.Highlighted = false;
      }
    } 
  }

  public List<Item> ContainedItems {
    get {
      List<Item> containedItems = new();
      foreach (var slot in containerSlots) {
        if (slot.ContainedItem != null) {
          containedItems.Add(slot.ContainedItem);
        }
      }
      return containedItems;
    }
  }

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
      if (Highlighted != value) {
        SelectItem(value);
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

  public virtual void InteractStart(PlayerInput player) {
    player.Processor.StartProcessing(this);
  }

  public virtual void InteractStop(PlayerInput player) {
    player.Processor.StopProcessing();
  }

  public void MoveSelectionUp() {
    MoveSelection(1);
  }

  public void MoveSelectionDown() {
    MoveSelection(-1);
  }

  private void MoveSelection(int step) {
    if (Highlighted && HighlightSelectedItemEnabled && containerSlots.Count > 1 && ContainedCount > 1) {
      var newSelection = selectedContainerSlotIndex;
      do {
        newSelection += step;
        newSelection %= containerSlots.Count;
        if (newSelection < 0) {
          newSelection = containerSlots.Count - 1;
        }
      } while (newSelection != selectedContainerSlotIndex && containerSlots[newSelection].ContainedItem == null);

      if (newSelection != selectedContainerSlotIndex) {
        containerSlots[selectedContainerSlotIndex].ContainedItem.Highlighted = false;
      }
      containerSlots[newSelection].ContainedItem.Highlighted = true;
      selectedContainerSlotIndex = newSelection;
    }
  }

  private void SelectItem(bool highlight) {
    bool hasMultipleSlotsAndAnItem = HighlightSelectedItemEnabled && containerSlots.Count > 1 && ContainedCount > 0;
    if (hasMultipleSlotsAndAnItem) {
      if (highlight && selectedContainerSlotIndex == -1) {
        selectedContainerSlotIndex = FirstOccupiedSlotIndex;
      }
      if (selectedContainerSlotIndex != -1) {
        containerSlots[selectedContainerSlotIndex].ContainedItem.Highlighted = highlight;
      }
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
    Item itemToTake = null;
    if (itemContainer.containerSlots.Count == 1) {
      itemToTake = itemContainer.containerSlots[0].ContainedItem;
    } else if (itemContainer.selectedContainerSlotIndex != -1) {
      itemToTake = itemContainer.containerSlots[itemContainer.selectedContainerSlotIndex].ContainedItem;
    }
    AddItem(itemToTake);
  }

  /*
  protected bool Contains(ItemContainer itemContainer) {
    return Contains((slot) => slot.ContainedItem != null && (slot.ContainedItem.Container == itemContainer || (slot.ContainedItem.IsContainer && slot.ContainedItem.Container.Contains(itemContainer))));
  }

  protected bool Contains(Processor processor) {
    return Contains((slot) => slot.ContainedItem != null && (slot.ContainedItem.Processor == processor || (slot.ContainedItem.IsContainer && slot.ContainedItem.Container.Contains(processor))));
  }

  private bool Contains(Predicate<ContainerSlot> containsCriteria) {
    foreach (var slot in containerSlots) {
      if (containsCriteria(slot)) {
        return true;
      }
    }
    return false;
  }
  */

  private bool AcceptsItem(Item item) {
    return item != null && acceptedItemsData.AcceptedItems.Contains(item.Data);
  }

  public int RemoveItem(Item item) {
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
