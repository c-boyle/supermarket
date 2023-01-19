using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticProcessor : Processor, IInteractable {
  [SerializeField] private ItemContainer container;
  [SerializeField] private Highlightable highlightable;
  public bool Highlighted { get => highlightable.Highlighted; set => highlightable.Highlighted = value; }

  public void InteractStart(PlayerInput player) {
    if (Processing) {
      StartProcessing(container);
    } else {
      StopProcessing();
    }
  }

  public void InteractStop(PlayerInput player) {
    return;
  }
}