using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class joinLobby : MonoBehaviour
{
    private string ipAddress = "127.0.0.1";
    [SerializeField] private InputField inputIP;

    private void Start()
    {
        inputIP.text = ipAddress;
    }

    public void Host()
    {
        if(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            NetworkManager.Singleton.StartHost();
            Destroy(gameObject);
        }
    }

    public void Client()
    {
        if(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, (ushort)7777);
            NetworkManager.Singleton.StartClient();
            Destroy(gameObject);
        }
    }

    public void ChangeIP()
    {
        ipAddress = inputIP.text;
    }
}
