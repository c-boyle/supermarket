using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableItemContainer : ItemContainer {
   public bool beingHeld = false;
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
      if (!player.Hands.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds)) {
        player.Hands.OnTriggerExit(GetComponent<Collider>());
      }
    }
  }

  public override void InteractStop(PlayerInput player) {
    return;
  }
}
