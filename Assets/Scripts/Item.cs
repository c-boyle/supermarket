using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IHighlightable {
  [SerializeField] private ItemData data;
  [SerializeField] private Highlightable highlighting;
  [field: SerializeField] public ItemContainer ContainedBy { get; set; }
  [field: SerializeField] public ItemContainer Container { get; set; }
  public bool IsContainer { get => Container != null; }
  public ItemData Data { get => data; }
  public bool Highlighted { get => highlighting.Highlighted; set => highlighting.Highlighted = value; }
}
