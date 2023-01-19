using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectingItemContainer : MonoBehaviour {


  private void Update() {
    if (highlightSelectedContainer) {
      if (inColliderContainers.Count > 0) {
        var selectedContainer = SelectedContainer;
        if (mostRecentSelectedContainer != selectedContainer) {
          if (mostRecentSelectedContainer != null) {
            mostRecentSelectedContainer.Highlighted = false;
          }
          OnSelectedContainerChange?.Invoke();
        }

        selectedContainer.HighlightSelectedItemEnabled = ContainedCount == 0;
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
      //Debug.Log("ItemContainer " + container.name + " entered container range");
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.gameObject.TryGetComponent(out ItemContainer container)) {
      if (container == mostRecentSelectedContainer) {
        container.Highlighted = false;
      }
      inColliderContainers.Remove(container);
      //Debug.Log("ItemContainer " + container.name + " exited container range");
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
