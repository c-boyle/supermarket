using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectingItemContainer : ItemContainer {
  private readonly HashSet<ItemContainer> inColliderContainers = new();
  [SerializeField] private bool usedByPlayer = false;
  private ItemContainer mostRecentSelectedContainer = null;

  private void Update() {
    if (usedByPlayer && inColliderContainers.Count > 0) {
      var selectedContainer = GetSelectedContainer();
      if (mostRecentSelectedContainer != null && mostRecentSelectedContainer != selectedContainer) {
        mostRecentSelectedContainer.Highlighted = false;
      }
      selectedContainer.Highlighted = true;
      mostRecentSelectedContainer = selectedContainer;
    }
  }

  /* Right now this uses a primitive solution of checking which container is closest to this detecting item container */
  private ItemContainer GetSelectedContainer() {
    ItemContainer selectedContainer = null;
    float selectedContainerDistance = float.MaxValue;
    foreach (var container in inColliderContainers) {
      if (!Contains(container)) {
        var detectionPosition = transform.position;
        var containerPosition = container.transform.position;
        detectionPosition.y = 0f;
        containerPosition.y = 0f;
        var dist = Vector3.Distance(detectionPosition, containerPosition);
        if (dist < selectedContainerDistance) {
          selectedContainer = container;
          selectedContainerDistance = dist;
        }
      }
    }
    return selectedContainer;
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out ItemContainer container)) {
      inColliderContainers.Add(container);
      Debug.Log("ItemContainer " + container.name + " entered container range");
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.gameObject.TryGetComponent(out ItemContainer container)) {
      if (container == mostRecentSelectedContainer) {
        container.Highlighted = false;
      }
      inColliderContainers.Remove(container);
      Debug.Log("ItemContainer " + container.name + " exited container range");
    }
  }

  public void PickupItem() {
    var selectedContainer = GetSelectedContainer();
    if (selectedContainer != null) {
      TakeItem(selectedContainer);
    }
  }

  public void PutDownItem() {
    var selectedContainer = GetSelectedContainer();
    if (selectedContainer != null) {
      selectedContainer.TakeItem(this);
    }
  }
}
