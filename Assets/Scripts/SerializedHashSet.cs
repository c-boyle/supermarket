using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedHashSet<T> : ISerializationCallbackReceiver {
  [SerializeField] private List<T> list = new();

  private HashSet<T> _hashSet = new();
  public HashSet<T> HashSet {
    get {
      if (changesOccured) {
        _hashSet.Clear();
        List<int> indicesToRemove = new();
        for (int i = 0; i < list.Count; i++) {
          bool isDuplicate = !_hashSet.Add(list[i]);
          if (isDuplicate) {
            indicesToRemove.Add(i);
          }
        }
        foreach (var i in indicesToRemove) {
          list.RemoveAt(i);
        }
      }
      return _hashSet;
    }
  }
  private bool changesOccured = false;

  public void OnBeforeSerialize() {
    return;
  }

  public void OnAfterDeserialize() {
    changesOccured = true;
  }
}
