using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processor : MonoBehaviour {
  [SerializeField] private ProcessorData processorData;
  [SerializeField] private ItemContainer container;
  private Coroutine processingRoutine;

  public void StartProcessing() {
    if (processingRoutine == null) {
      processingRoutine = StartCoroutine(Process());
    }
  }

  public void StopProcessing() {
    if (processingRoutine != null) {
      StopCoroutine(processingRoutine);
    }
  }

  private IEnumerator Process() {
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
      Instantiate(kvp.Value, kvp.Key.transform.position, kvp.Key.transform.rotation, kvp.Key.transform);
      Destroy(kvp.Key.gameObject);
    }

    processingRoutine = null;
  }
}
