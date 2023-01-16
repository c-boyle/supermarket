using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
  [SerializeField] private ItemData data;
  [field: SerializeField] public Highlightable Highlighting { get; set; } = new();
  [field: SerializeField] public ItemContainer ContainedBy { get; set; }
  [field: SerializeField] public ItemContainer Container { get; set; }
  public bool IsContainer { get => Container != null; }
  public ItemData Data { get => data; }
}
