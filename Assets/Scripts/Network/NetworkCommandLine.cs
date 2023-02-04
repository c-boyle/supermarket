using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;


// Citation: https://docs-multiplayer.unity3d.com/netcode/current/tutorials/helloworld/index.html
public class NetworkCommandLine : MonoBehaviour {
  [SerializeField] private NetworkManager netManager;
  [SerializeField] private UnityTransport transport;

  void Start() {
    if (Application.isEditor) return;

    var args = GetCommandlineArgs();

    if (args.TryGetValue("-mode", out string mode)) {

      if (args.TryGetValue("-ip", out string ipAddress)) {
        transport.ConnectionData.Address = ipAddress;
      }
      if (args.TryGetValue("-port", out string port)) {
        transport.ConnectionData.Port = ushort.Parse(port);
      }

      switch (mode) {
        case "server":
          netManager.StartServer();
          break;
        case "host":
          netManager.StartHost();
          break;
        case "client":
          netManager.StartClient();
          break;
      }
      Debug.Log("Using address:" + transport.ConnectionData.Address + " and port:" + transport.ConnectionData.Port);
    }

  }

  private Dictionary<string, string> GetCommandlineArgs() {
    Dictionary<string, string> argDictionary = new Dictionary<string, string>();

    var args = System.Environment.GetCommandLineArgs();

    for (int i = 0; i < args.Length; ++i) {
      var arg = args[i].ToLower();
      if (arg.StartsWith("-")) {
        var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
        value = (value?.StartsWith("-") ?? false) ? null : value;

        argDictionary.Add(arg, value);
      }
    }
    return argDictionary;
  }
}