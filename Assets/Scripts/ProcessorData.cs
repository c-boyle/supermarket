using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data class that represents what a processor can process and what it gets processed into.
/// </summary>

[CreateAssetMenu(fileName = nameof(ProcessorData), menuName = "ScriptableObjects/" + nameof(ProcessorData))]
public class ProcessorData : ScriptableObject {
  [SerializeField] private List<ItemDataToProcessedItemKVP> _itemDataToProcessedItemList = new();
  [field: SerializeField] public float RequiredTime { get; private set; }

  private Dictionary<ItemData, Item> _itemDataToProcessedItem = null;
  public Dictionary<ItemData, Item> ItemDataToProcessedItem { 
    get {
      if (_itemDataToProcessedItem == null) {
        _itemDataToProcessedItem = new();
        foreach (var kvp in _itemDataToProcessedItemList) {
          _itemDataToProcessedItem[kvp.ItemData] = kvp.Item;
        }
      }
      return _itemDataToProcessedItem;
    }
  }

  private void OnValidate() {
    _itemDataToProcessedItem = null;
  }

  [System.Serializable]
  private class ItemDataToProcessedItemKVP {
    [field: SerializeField] public ItemData ItemData { get; set; }
    [field: SerializeField] public Item Item { get; set; }
  }
}
