using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticProcessor : Processor, IInteractable {
  [SerializeField] private ItemContainer container;

  public void InteractStart(PlayerInput player) {
    StartProcessing(container);
  }

  public void InteractStop(PlayerInput player) {
    StopProcessing();
  }
}
