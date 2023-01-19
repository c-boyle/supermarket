using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectingItemContainer : ItemContainer {
  private readonly HashSet<IInteractable> inColliderInteractables = new();
  private IInteractable mostRecentSelected = null;
  public Action OnSelectedChange { get; set; } = null;

  /* Right now this uses a primitive solution of checking which container is closest to this detecting item container */
  /* Gets called every frame right now, pretty inefficient as it stands */
  public IInteractable Selected {
    get {
      return Helpers.GetNearest(transform.position, inColliderInteractables, (interactable) => !Contains(interactable));
    }
  }

  private void Update() {
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
      selectedInteractable.Highlighted = true;

      mostRecentSelected = selectedInteractable;

    } else if (mostRecentSelected != null) {
      mostRecentSelected = null;
      OnSelectedChange?.Invoke();
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out IInteractable interactable)) {
      inColliderInteractables.Add(interactable);
      //Debug.Log("ItemContainer " + container.name + " entered container range");
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.gameObject.TryGetComponent(out IInteractable interactable)) {
      if (interactable == mostRecentSelected) {
        interactable.Highlighted = false;
      }
      inColliderInteractables.Remove(interactable);
      //Debug.Log("ItemContainer " + container.name + " exited container range");
    }
  }

  public void PickupItem() {
    if (Selected is ItemContainer container) {
      TakeItem(container);
    }
  }

  public void PutDownItem() {
    if (Selected is ItemContainer container) {
      container.TakeItem(this);
    }
  }
}
