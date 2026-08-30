using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Asigna aquí el punto donde nacerán")]
    [SerializeField] private Transform spawnPoint;

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadedCompleted;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadedCompleted;
        }
    }

    private void OnSceneLoadedCompleted(string sceneName, LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        // Solo el Host/Servidor reubica a todos los jugadores conectados al terminar la carga
        if (NetworkManager.Singleton.IsServer)
        {
            if (spawnPoint == null) spawnPoint = transform;

            foreach (var clientId in clientsCompleted)
            {
                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
                {
                    if (client.PlayerObject != null)
                    {
                        CharacterController cc = client.PlayerObject.GetComponent<CharacterController>();
                        if (cc != null) cc.enabled = false;

                        // Pequeño desplazamiento por jugador para que no se superpongan
                        Vector3 posicionSpawn = spawnPoint.position + new Vector3(clientId * 1.5f, 0.5f, 0f);
                        client.PlayerObject.transform.position = posicionSpawn;
                        client.PlayerObject.transform.rotation = spawnPoint.rotation;

                        if (cc != null) cc.enabled = true;
                    }
                }
            }
        }
    }
}