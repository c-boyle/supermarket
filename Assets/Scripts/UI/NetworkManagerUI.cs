using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour {
  [SerializeField] private Button serverButton;
  [SerializeField] private Button hostButton;
  [SerializeField] private Button clientButton;
  private bool doShow = false;

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
    serverButton.gameObject.SetActive(doShow);
    hostButton.gameObject.SetActive(doShow);
    clientButton.gameObject.SetActive(doShow);
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.P)) {
      doShow = !doShow;
      serverButton.gameObject.SetActive(doShow);
      hostButton.gameObject.SetActive(doShow);
      clientButton.gameObject.SetActive(doShow);
    }
  }
}
