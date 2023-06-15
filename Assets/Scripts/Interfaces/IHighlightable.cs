using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for game objects that can be highlighted.
/// </summary>

public interface IHighlightable {
  // The highlighting and un-highlighting of the object should be handled in the setter of this property.
  public bool Highlighted { get; set; }
}
