using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Processor : NetworkBehaviour {
  [SerializeField] private ProcessorData processorData;
  private Coroutine processingRoutine;
  protected bool Processing { get => processingRoutine != null; }

  public void StartProcessing(ItemContainer container) {
    if (processingRoutine == null && container != null) {
      processingRoutine = StartCoroutine(Process(container));
      Debug.Log("Processor " + name + " Started At Time: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:ff"));
    }
  }

  public void StopProcessing() {
    if (processingRoutine != null) {
      StopCoroutine(processingRoutine);
      processingRoutine = null;
      Debug.Log("Processor " + name + " Stopped At Time: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:ff"));
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
      var beforeItem = kvp.Key;
      var beforeTransform = beforeItem.transform;
      var newItem = Instantiate(kvp.Value, beforeTransform.position, beforeTransform.rotation, beforeTransform.parent);
      newItem.ContainedBy = beforeItem.ContainedBy;
      newItem.Highlighted = beforeItem.Highlighted;
      int slot = beforeItem.ContainedBy.RemoveItem(beforeItem);
      newItem.ContainedBy.AddItem(newItem, slot);
      Destroy(beforeItem.gameObject);
    }

    processingRoutine = null;
    Debug.Log("Processor " + name + " Complete At Time: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:ff"));
  }
}
