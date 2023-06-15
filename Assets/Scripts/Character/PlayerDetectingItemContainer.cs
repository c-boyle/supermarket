using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// A special kind of item container intended to be used by player characters to detect when items
/// are within their reach, and pick up or put down said items.
/// </summary>

public class PlayerDetectingItemContainer : ItemContainer {
  private readonly HashSet<IInteractable> inColliderInteractables = new();
  private IInteractable mostRecentSelected = null;
  public Action OnSelectedChange { get; set; } = null;
  public bool PickUpAndDropEnabled { get; set; } = true;
  public bool HighlightingEnabled { get; set; } = true;

  /* 
   * Right now this uses a primitive solution of checking which container is closest to this detecting item container.
   * It gets called roughly every frame right now, pretty inefficient as it stands.
   */ 
  public IInteractable Selected {
    get {
      return Helpers.GetNearest(transform.position, inColliderInteractables);
    }
  }

  protected override void OnEnable() {
    return;
  }

  protected override void OnDisable() {
    return;
  }

  private void Update() {
    if (!HighlightingEnabled) {
      return;
    }
    if (inColliderInteractables.Count > 0) {
      var selectedInteractable = Selected;
      if (mostRecentSelected != selectedInteractable) {
        if (mostRecentSelected != null) {
          mostRecentSelected.Highlighted = false;
        }
        OnSelectedChange?.Invoke();
      }

      if (selectedInteractable is ItemContainer container) {
        container.HighlightSelectedItemEnabled = ContainedCount == 0;
      }
      selectedInteractable.Highlighted = PickUpAndDropEnabled;

      mostRecentSelected = selectedInteractable;

    } else if (mostRecentSelected != null) {
      mostRecentSelected = null;
      OnSelectedChange?.Invoke();
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out IInteractable interactable)) {
      inColliderInteractables.Add(interactable);
      // Debug.Log("IInteractable " + other.name + " entered container range");
    }
  }

  public void OnTriggerExit(Collider other) {
    if (other.gameObject.TryGetComponent(out IInteractable interactable)) {
      if (interactable == mostRecentSelected) {
        interactable.Highlighted = false;
      }
      inColliderInteractables.Remove(interactable);
      // Debug.Log("IInteractable " + other.name + " exited container range");
    }
  }

  public void PickupItem(IInteractable selected) {
    if (PickUpAndDropEnabled) {
      ItemContainer selectedContainer = null;
      if (selected is ItemContainer container) {
        selectedContainer = container;
      } else if (selected is AutomaticProcessor processor) {
        selectedContainer = processor.Container;
      }
      if (selectedContainer != null) {
        TakeItem(selectedContainer);
      }
    }
  }

  public void PutDownItem(IInteractable selected) {
    if (PickUpAndDropEnabled) {
      ItemContainer selectedContainer = null;
      if (selected is ItemContainer container) {
        selectedContainer = container;
      } else if (selected is AutomaticProcessor processor) {
        selectedContainer = processor.Container;
      }
      if (selectedContainer != null) {
        selectedContainer.TakeItem(this);
      }
    }
  }
}
