using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special item container class that can be pushed around by characters.
/// </summary>

public class PushableItemContainer : ItemContainer {
   private bool beingHeld = false;
   private Transform originalParent = null;

  public override void InteractStart(PlayerInput player) {
    beingHeld = !beingHeld;
    if (beingHeld) {
      player.Movement.Stop();
      player.Movement.RotationEnabled = false;
      player.Hands.PickUpAndDropEnabled = false;

      originalParent = transform.parent;
      transform.SetParent(player.transform, true);
      transform.rotation = Quaternion.identity;
    } else {
      transform.SetParent(originalParent, true);

      player.Movement.RotationEnabled = true;
      player.Hands.PickUpAndDropEnabled = true;
      player.Hands.OnTriggerExit(GetComponent<Collider>());
    }
  }

  public override void InteractStop(PlayerInput player) {
    return;
  }
}
