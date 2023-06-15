using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A temporary script used for testing network operations.
/// </summary>

public class NetworkManagerUI : MonoBehaviour {
  [SerializeField] private Button serverButton;
  [SerializeField] private Button hostButton;
  [SerializeField] private Button clientButton;
  private bool show = false;

  private void Awake() {
    serverButton.onClick.AddListener(() => {
      NetworkManager.Singleton.StartServer();
    });
    hostButton.onClick.AddListener(() => {
      NetworkManager.Singleton.StartHost();
    });
    clientButton.onClick.AddListener(() => {
      NetworkManager.Singleton.StartClient();
    });
    serverButton.gameObject.SetActive(show);
    hostButton.gameObject.SetActive(show);
    clientButton.gameObject.SetActive(show);
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.P)) {
      show = !show;
      serverButton.gameObject.SetActive(show);
      hostButton.gameObject.SetActive(show);
      clientButton.gameObject.SetActive(show);
    }
  }
}
