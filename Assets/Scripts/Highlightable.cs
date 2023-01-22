using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlightable : MonoBehaviour, IHighlightable {
  [SerializeField] private List<Renderer> renderersToHighlight = new();

  private bool _highlighted = false;
  public bool Highlighted {
    get {
      return _highlighted;
    }
    set {
      SetHighlight(value);
      _highlighted = value;
    }
  }

  private void SetHighlight(bool highlight) {
    if (highlight != Highlighted) {
      foreach (var renderer in renderersToHighlight) {
        var material = renderer.material;
        if (highlight) {
          material.EnableKeyword("_EMISSION");
          material.SetColor("_EmissionColor", Color.white);
        } else {
          material.DisableKeyword("_EMISSION");
        }
      }
    }
  }
}
