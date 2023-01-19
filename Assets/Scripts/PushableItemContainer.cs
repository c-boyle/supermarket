using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableItemContainer : ItemContainer {
   private bool beingHeld = false;
   private Transform originalParent = null;

  public override void InteractStart(PlayerInput player) {
    beingHeld = !beingHeld;
    if (beingHeld) {
      originalParent = transform.parent;
      transform.SetParent(player.transform, true);
    } else {
      transform.SetParent(originalParent, true);
    }
  }

  public override void InteractStop(PlayerInput player) {
    return;
  }
}
