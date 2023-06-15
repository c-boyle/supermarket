using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that handles the processing of one item into another (example: cookie dough into baked cookies).
/// </summary>

public class Processor : MonoBehaviour {
  [SerializeField] private ProcessorData processorData;
  private Coroutine processingRoutine;
  protected bool Processing { get => processingRoutine != null; }

  public void StartProcessing(ItemContainer container) {
    if (processingRoutine == null && container != null) {
      processingRoutine = StartCoroutine(Process(container));
      Debug.Log("Processor " + name + " started");
    }
  }

  public void StopProcessing() {
    if (processingRoutine != null) {
      StopCoroutine(processingRoutine);
      processingRoutine = null;
      Debug.Log("Processor " + name + " stopped");
    }
  }

  private IEnumerator Process(ItemContainer container) {
    Dictionary<Item, Item> beforeProcessToProcessed = new();
    // Check if all the items in the container can be processed
    foreach (var item in container.ContainedItems) {
      if (processorData.ItemDataToProcessedItem.TryGetValue(item.Data, out var processedItem)) {
        beforeProcessToProcessed[item] = processedItem;
      }
    }

    yield return new WaitForSeconds(processorData.RequiredTime);

    foreach (var kvp in beforeProcessToProcessed) {
      Item beforeItem = kvp.Key;
      Transform beforeTransform = beforeItem.transform;
      Item newItem = Instantiate(kvp.Value, beforeTransform.position, beforeTransform.rotation, beforeTransform.parent);
      newItem.ContainedBy = beforeItem.ContainedBy;
      newItem.Highlighted = beforeItem.Highlighted;
      int slot = beforeItem.ContainedBy.RemoveItem(beforeItem);
      newItem.ContainedBy.AddItem(newItem, slot);
      Destroy(beforeItem.gameObject);
    }

    processingRoutine = null;
    Debug.Log("Processor " + name + " complete");
  }
}
