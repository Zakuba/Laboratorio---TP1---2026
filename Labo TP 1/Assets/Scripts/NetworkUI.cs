using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El objeto que contiene los botones 'Crear partida' y 'Unirse a partida' (por ejemplo, un panel/objeto vacío que sea padre de ambos botones).")]
    [SerializeField] private GameObject panelMenu;

    [Header("Escena de juego")]
    [Tooltip("Nombre EXACTO de la escena del nivel, tal como figura en Build Settings.")]
    [SerializeField] private string nombreEscenaNivel = "Nivel1";

    // Llamar desde el botón "Crear partida"
    public void OnClickHost()
    {
        NetworkManager.Singleton.StartHost();

        // Solo el servidor/host decide a qué escena se pasa; Netcode se
        // encarga de sincronizar automáticamente a todos los clientes
        // conectados hacia esa misma escena.
        NetworkManager.Singleton.SceneManager.LoadScene(
            nombreEscenaNivel,
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );

        OcultarMenu();
    }

    // Llamar desde el botón "Unirse a partida"
    public void OnClickClient()
    {
        NetworkManager.Singleton.StartClient();
        OcultarMenu();
    }

    private void OcultarMenu()
    {
        if (panelMenu != null)
        {
            panelMenu.SetActive(false);
        }
    }
}