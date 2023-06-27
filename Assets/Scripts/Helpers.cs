using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// A class of general purpose helper functions.
/// </summary>

public static class Helpers {
  /// <summary>
  /// Gets the nearest interactable to the given position.
  /// </summary>
  public static T GetNearest<T>(Vector3 position, IEnumerable<T> targets, Predicate<T> inclusionCriteria = null) where T : IInteractable {
    T nearest = default;
    float nearestDist = float.MaxValue;
    foreach (var target in targets) {
      if (inclusionCriteria == null || inclusionCriteria(target)) {
        var detectionPosition = position;
        var containerPosition = (target as Component).transform.position;
        detectionPosition.y = 0f;
        containerPosition.y = 0f;
        var dist = Vector3.Distance(detectionPosition, containerPosition);
        if (dist < nearestDist) {
          nearest = target;
          nearestDist = dist;
        }
      }
    }
    return nearest;
  }

  // Cite: https://stackoverflow.com/questions/3804367/testing-for-equality-between-dictionaries-in-c-sharp
  public static bool DictionaryContentsEqual<TKey, TValue>(Dictionary<TKey, TValue> dic1, Dictionary<TKey, TValue> dic2) {
    return dic1.Count == dic2.Count && !dic1.Except(dic2).Any();
  }

  public static IEnumerator InvokeAfterDelay(Action action, float delaySeconds) {
    yield return new WaitForSeconds(delaySeconds);
    action?.Invoke();
  }
}
