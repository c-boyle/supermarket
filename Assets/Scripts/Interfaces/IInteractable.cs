using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for interactable game objects.
/// </summary>

public interface IInteractable : IHighlightable {
  public void InteractStart(PlayerInput player);
  public void InteractStop(PlayerInput player);
  public int Id { get; set; }
}
