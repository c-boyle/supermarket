using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectingItemContainer : ItemContainer {
  private readonly HashSet<ItemContainer> inColliderContainers = new();

  /* Right now this uses a primitive solution of checking which container is closest to this detecting item container */
  /* Gets called every frame right now, pretty inefficient as it stands */
  public ItemContainer SelectedContainer {
    get {
      return Helpers.GetNearest(transform.position, inColliderContainers);
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
