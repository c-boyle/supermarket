using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A processor that automatically processes the items in its container (instead of being processed by a player manually).
/// </summary>

public class AutomaticProcessor : Processor, IInteractable {
  [field: SerializeField] public ItemContainer Container { get; private set; }
  [SerializeField] private Highlightable highlightable;
  [SerializeField] private Door door;
  [SerializeField] private Light activeLight;
  public bool Highlighted { get => highlightable.Highlighted; set => highlightable.Highlighted = value; }
  private bool firstRun = true;

  private Coroutine fadeRoutine = null;
  private readonly float fadeTime = 0.2f;
  private float originalLightRange;

  public int Id { get; set; } = -1;

  private void OnEnable() {
    if (Id == -1) {
      Id = PlayerInput.UnusedId;
      activeLight.enabled = true;
      originalLightRange = activeLight.range;
      activeLight.range = 0f;
    }
    PlayerInput.IdToInteractable[Id] = this;
  }

  private void OnDisable() {
    PlayerInput.IdToInteractable.Remove(Id);
  }

  public void InteractStart(PlayerInput player) {
    if (door != null) {
      door.IsOpen = false;
      if (firstRun) {
        door.OnOpen += () => StopProcessing();
        firstRun = false;
      }
    }
    if (!Processing) {
      StartProcessing(Container);
    } else {
      StopProcessing();
    }
  }

  public override void StartProcessing(ItemContainer container) {
    base.StartProcessing(container);
    if (fadeRoutine == null) {
      fadeRoutine = StartCoroutine(FadeInThenOffOnComplete());
    }
  }

  public override void StopProcessing() {
    base.StopProcessing();
    if (fadeRoutine != null) {
      StopCoroutine(fadeRoutine);
    }
    fadeRoutine = StartCoroutine(FadeLight(false));
  }

  public void InteractStop(PlayerInput player) {
    return;
  }

  private IEnumerator FadeInThenOffOnComplete() {
    yield return FadeLight(true);
    float timeUntilFadeout = processorData.RequiredTime - (2f * fadeTime);
    yield return new WaitForSeconds(timeUntilFadeout);
    yield return FadeLight(false);
  }

  private IEnumerator FadeLight(bool fadeIn) {
    float startTime = Time.time;
    float endRange = fadeIn ? originalLightRange : 0f;
    float stepSize = (endRange - activeLight.range) / fadeTime;
    while (Time.time - startTime < fadeTime && stepSize != 0f) {
      activeLight.range += stepSize * Time.deltaTime;
      yield return null;
    }
    if (!fadeIn) {
      fadeRoutine = null;
    }
  }
}
