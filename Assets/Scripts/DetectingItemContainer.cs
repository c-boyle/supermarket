using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectingItemContainer : ItemContainer {
  private readonly HashSet<ItemContainer> inColliderContainers = new();
  [SerializeField] private bool usedByPlayer = false;
  private ItemContainer mostRecentSelectedContainer = null;
  public Action OnSelectedContainerChange { get; set; } = null;

  /* Right now this uses a primitive solution of checking which container is closest to this detecting item container */
  public ItemContainer SelectedContainer {
    get {
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
  }

  private void Update() {
    if (usedByPlayer) {
      if (inColliderContainers.Count > 0) {
        var selectedContainer = SelectedContainer;
        if (mostRecentSelectedContainer != selectedContainer) {
          if (mostRecentSelectedContainer != null) {
            mostRecentSelectedContainer.Highlighted = false;
          }
          OnSelectedContainerChange?.Invoke();
        }

        selectedContainer.Highlighted = true;
        mostRecentSelectedContainer = selectedContainer;

      } else if (mostRecentSelectedContainer != null) {
        mostRecentSelectedContainer = null;
        OnSelectedContainerChange?.Invoke();
      }
    }
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
    var selectedContainer = SelectedContainer;
    if (selectedContainer != null) {
      TakeItem(selectedContainer);
    }
  }

  public void PutDownItem() {
    var selectedContainer = SelectedContainer;
    if (selectedContainer != null) {
      selectedContainer.TakeItem(this);
    }
  }
}
