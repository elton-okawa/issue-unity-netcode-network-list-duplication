
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimulatorToolsIssue {
    public class NetworkManagerGUI : MonoBehaviour {
        void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
                StartButtons();
            } else {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        void Start() {
            NetworkManager.Singleton.OnServerStarted += LoadListScene;
        }

        void OnDestroy() {
            if (NetworkManager.Singleton) {
                NetworkManager.Singleton.OnServerStarted -= LoadListScene;
            }
        }

        void LoadListScene() {
            NetworkManager.Singleton.SceneManager.LoadScene("ListScene", LoadSceneMode.Single);
        }

        static void StartButtons() {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels() {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);

            if (NetworkManager.Singleton.IsHost) {
                if (GUILayout.Button("ListScene")) {
                    Debug.Log("[NetworkManagerGUI] Loading ListScene");
                    NetworkManager.Singleton.SceneManager.LoadScene("ListScene", LoadSceneMode.Single);
                }
                if (GUILayout.Button("DummyScene")) {
                    Debug.Log("[NetworkManagerGUI] Loading DummyScene");
                    NetworkManager.Singleton.SceneManager.LoadScene("DummyScene", LoadSceneMode.Single);
                }
            }
        }
    }
}