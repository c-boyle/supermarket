using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A game item in the world.
/// Note: an item will always be contained by an item container.
/// </summary>

public class Item : MonoBehaviour, IHighlightable {
  [SerializeField] private ItemData data;
  [SerializeField] private Highlightable highlighting;
  /// <summary>
  /// The item container that contains this item.
  /// </summary>
  [field: SerializeField] public ItemContainer ContainedBy { get; set; }
  /// <summary>
  /// If this item is itself a container (like a tray), then this will be that container. Null otherwise.
  /// </summary>
  [field: SerializeField] public ItemContainer Container { get; set; }
  [field: SerializeField] public Processor Processor { get; set; }
  public bool IsContainer { get => Container != null; }
  public bool IsProcessor { get => Processor != null; }
  public ItemData Data { get => data; }
  public bool Highlighted { get => highlighting.Highlighted; set => highlighting.Highlighted = value; }
}
