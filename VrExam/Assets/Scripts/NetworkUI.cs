using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    public void StartHost()
    {
        Debug.Log("Host Started");
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        Debug.Log("Client Started");
        NetworkManager.Singleton.StartClient();
    }
}