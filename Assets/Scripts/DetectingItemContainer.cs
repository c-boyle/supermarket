using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// An item container that contains all items within a 3d space.
/// </summary>

public class DetectingItemContainer : ItemContainer {
  private readonly HashSet<ItemContainer> inColliderContainers = new();

  /// <summary>
  /// The list of items that are in the space owned by this item container.
  /// </summary>
  public override List<Item> ContainedItems {
    get {
      List<Item> containedItems = new();
      foreach (var container in inColliderContainers) {
        containedItems.AddRange(container.ContainedItems);
      }
      return containedItems;
    }
  }

  /* 
   * Right now this uses a primitive solution of checking which container is closest to this detecting item container.
   * It gets called roughly every frame right now, pretty inefficient as it stands.
   */ 
  public ItemContainer SelectedContainer {
    get {
      ItemContainer selected = null;
      do {
        selected = Helpers.GetNearest(transform.position, inColliderContainers);
        bool selectedFoundButInactive = selected != null && !selected.gameObject.activeInHierarchy;
        if (selectedFoundButInactive) {
          inColliderContainers.Remove(selected);
        }
      } while (selected != null && !inColliderContainers.Contains(selected));
      return selected;
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out ItemContainer container)) {
      inColliderContainers.Add(container);
      //Debug.Log("ItemContainer " + container.name + " entered container range");
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.gameObject.TryGetComponent(out ItemContainer container)) {
      inColliderContainers.Remove(container);
      //Debug.Log("ItemContainer " + container.name + " exited container range");
    }
  }
}
