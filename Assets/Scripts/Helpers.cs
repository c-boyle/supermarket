using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Helpers {
  public static T GetNearest<T>(Vector3 position, IEnumerable<T> targets, Predicate<T> inclusionCriteria = null) where T : Component, IInteractable {
    T nearest = null;
    float nearestDist = float.MaxValue;
    foreach (var target in targets) {
      if (inclusionCriteria == null || inclusionCriteria(target)) {
        var detectionPosition = position;
        var containerPosition = target.transform.position;
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
}
