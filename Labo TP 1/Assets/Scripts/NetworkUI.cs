using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 180, 160));

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Iniciar Host", GUILayout.Height(35))) 
                NetworkManager.Singleton.StartHost();

            if (GUILayout.Button("Iniciar Server", GUILayout.Height(35))) 
                NetworkManager.Singleton.StartServer();

            if (GUILayout.Button("Unirse como Cliente", GUILayout.Height(35))) 
                NetworkManager.Singleton.StartClient();
        }
        else
        {
            GUILayout.Label("Conectado como: " + (NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Cliente"));
            
            if (GUILayout.Button("Desconectar", GUILayout.Height(35))) 
                NetworkManager.Singleton.Shutdown();
        }

        GUILayout.EndArea();
    }
}
