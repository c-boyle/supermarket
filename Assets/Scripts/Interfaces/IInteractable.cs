using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
  public void InteractStart(PlayerInput player);
  public void InteractStop(PlayerInput player);
}
