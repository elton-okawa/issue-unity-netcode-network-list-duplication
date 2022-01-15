using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

struct LobbyData : IEquatable<LobbyData> {
    public ulong ClientId;

    public LobbyData(ulong clientId) {
        ClientId = clientId;
    }

    public bool Equals(LobbyData other) {
        return ClientId == other.ClientId;
    }

    public override string ToString() {
        return $"LobbyData (clientId: {ClientId})";
    }
}

public class NetworkListController : NetworkBehaviour {

    [SerializeField] Transform displayParent;
    [SerializeField] GameObject valueDisplayPrefab;

    NetworkList<LobbyData> lobbyList;

    void Awake() {
        lobbyList = new NetworkList<LobbyData>();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (IsClient && !IsHost) {
            lobbyList.OnListChanged += HandleListChanged;
        }

        if (IsServer) {
            PopulateWithClientsConnected();

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    public override void OnDestroy() {
        base.OnDestroy();

        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        lobbyList.OnListChanged -= HandleListChanged;
    }
    
    void PopulateWithClientsConnected() {
        Debug.Log($"PopulateDisplayWithClientsConnected: '{NetworkManager.Singleton.ConnectedClientsIds.Count}' clients connected");
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            HandleClientConnected(clientId);
        }
    }

    void HandleClientConnected(ulong clientId) {
        Debug.Log($"ClientConnected: '{clientId}'");
        lobbyList.Add(new LobbyData(clientId));
        PopulateDisplay();
    }

    void HandleClientDisconnected(ulong clientId) {
        Debug.Log($"ClientDisconnected: '{clientId}'");
        for (int i = 0; i < lobbyList.Count; i++) {
            if (lobbyList[i].ClientId == clientId) {
                lobbyList.RemoveAt(i);
                break;
            }
        }
    }

    void HandleListChanged(NetworkListEvent<LobbyData> changeEvent) {
        Debug.Log($"ListChanged (eventType: {changeEvent.Type}, value: {changeEvent.Value}");
        PopulateDisplay();
    }

    void PrintNetworkListValues() {
        List<string> listValues = new List<string>();
        for(int i = 0; i < lobbyList.Count; i++) {
            listValues.Add(lobbyList[i].ClientId.ToString());
        }
        Debug.Log($"LobbyList values: {string.Join(", ", listValues)}");
    }

    void ClearExistingDisplay() {
        foreach (Transform child in displayParent) {
            Destroy(child.gameObject);
        }
    }

    void PopulateDisplay() {
        PrintNetworkListValues();
        ClearExistingDisplay();
        foreach (var data in lobbyList) {
            GameObject valueDisplay = Instantiate(valueDisplayPrefab);
            valueDisplay.GetComponentInChildren<Text>().text = data.ClientId.ToString();
            valueDisplay.transform.SetParent(displayParent);
        }
    }
}
