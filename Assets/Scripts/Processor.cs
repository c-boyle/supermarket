using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processor : MonoBehaviour {
  [SerializeField] private ProcessorData processorData;
  private Coroutine processingRoutine;
  protected bool Processing { get => processingRoutine != null; }

  public void StartProcessing(ItemContainer container) {
    if (processingRoutine == null && container != null) {
      processingRoutine = StartCoroutine(Process(container));
    }
  }

  public void StopProcessing() {
    if (processingRoutine != null) {
      StopCoroutine(processingRoutine);
      processingRoutine = null;
    }
  }

  private IEnumerator Process(ItemContainer container) {
    Dictionary<Item, Item> beforeProcessToProcessed = new();
    // Check if all the items in the container can be processed
    foreach (var item in container.ContainedItems) {
      if (!processorData.ItemDataToProcessedItem.TryGetValue(item.Data, out var processedItem)) {
        yield break;
      }
      beforeProcessToProcessed[item] = processedItem;
    }

    yield return new WaitForSeconds(processorData.RequiredTime);

    foreach (var kvp in beforeProcessToProcessed) {
      var beforeTransform = kvp.Key.transform;
      var newItem = Instantiate(kvp.Value, beforeTransform.position, beforeTransform.rotation, beforeTransform.parent);
      newItem.ContainedBy = kvp.Key.ContainedBy;
      newItem.Highlighted = kvp.Key.Highlighted;
      int slot = kvp.Key.ContainedBy.RemoveItem(kvp.Key);
      newItem.ContainedBy.AddItem(newItem, slot);
      Destroy(kvp.Key.gameObject);
    }

    processingRoutine = null;
  }
}
